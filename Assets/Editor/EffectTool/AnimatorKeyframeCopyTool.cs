using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core;

namespace Framework.Editor
{
    public class AnimatorKeyframeCopyTool : UnityEditor.Editor
    {
        public class GameObjectNode
        {
            public string               Path;
            public GameObject           NodeGo;

            public GameObjectNode(GameObject rNodeGo)
            {
                this.NodeGo = rNodeGo;
                this.Path = UtilTool.GetTransformPath(rNodeGo.transform);
            }
        }

        public class SelectedNode
        {
            public GameObjectNode       Node;
            public List<GameObjectNode> ChildNodes;
        }

        private static SelectedNode     mSelectedNode;
        private static SelectedNode     mCopyNode;
        private static AnimatorClipInfo mAnimatorClipInfo;

        [MenuItem("Tools/Effect/KeyFrameCopyTool Copy %#c")]
        public static void CopyKeyFrames()
        {
            GameObject rSelectGo = Selection.activeGameObject;
            if (rSelectGo == null) return;

            Animator rAnimator = rSelectGo.GetComponentInParent<Animator>();
            if (rAnimator == null) return;

            var rAnimationClips = rAnimator.GetCurrentAnimatorClipInfo(0);
            if (rAnimationClips == null || rAnimationClips.Length == 0) return;
            mAnimatorClipInfo = rAnimationClips[0];

            mSelectedNode = CreateNode(rSelectGo);
        }

        [MenuItem("Tools/Effect/KeyFrameCopyTool Paste %#v")]
        public static void PasteKeyFrames()
        {
            // 复制Node GameObject
            mCopyNode = CopyNode(mSelectedNode);

            //Debug.LogError(mSelectedNode.Node.Path);
            //for (int i = 0; i < mSelectedNode.ChildNodes.Count; i++)
            //{
            //    Debug.LogError(mSelectedNode.ChildNodes[i].Path);
            //}

            //Debug.LogError(mCopyNode.Node.Path);
            //for (int i = 0; i < mCopyNode.ChildNodes.Count; i++)
            //{
            //    Debug.LogError(mCopyNode.ChildNodes[i].Path);
            //}
        }

        private static SelectedNode CreateNode(GameObject rSelectedGo)
        {
            var rSelectedNode = new SelectedNode();
            rSelectedNode.Node = new GameObjectNode(rSelectedGo);
            rSelectedNode.ChildNodes = new List<GameObjectNode>();

            Transform[] rChildTransforms = rSelectedGo.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < rChildTransforms.Length; i++)
            {
                // 过滤掉自己
                if (rChildTransforms[i] == rSelectedGo.transform) continue;

                var rChildNode = new GameObjectNode(rChildTransforms[i].gameObject);
                rSelectedNode.ChildNodes.Add(rChildNode);
            }
            return rSelectedNode;
        }

        private static SelectedNode CopyNode(SelectedNode rNode)
        {
            SelectedNode rCopyNode = new SelectedNode();
            var rNewGo = GameObject.Instantiate(rNode.Node.NodeGo);
            rNewGo.transform.parent = rNode.Node.NodeGo.transform.parent;
            rNewGo.name = rNode.Node.NodeGo.name + "_1";
            rCopyNode.Node = new GameObjectNode(rNewGo);
            rCopyNode.ChildNodes = new List<GameObjectNode>();

            Transform[] rChildTransforms = rNewGo.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < rChildTransforms.Length; i++)
            {
                // 过滤掉自己
                if (rChildTransforms[i] == rNewGo.transform) continue;

                var rChildNode = new GameObjectNode(rChildTransforms[i].gameObject);
                rCopyNode.ChildNodes.Add(rChildNode);
            }
            return rCopyNode;
        }
    }
}