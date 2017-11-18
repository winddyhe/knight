//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core;

namespace Framework.Editor
{
    public class AnimatorKeyframeCopyTool : UnityEditor.Editor
    {
        public class AnimCurve
        {
            public EditorCurveBinding   CurveBinding;
            public AnimationCurve       Curve;
        }

        public class GameObjectNode
        {
            public string               Path;
            public GameObject           NodeGo;
            public List<AnimCurve>      Curves;

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
        private static AnimationClip    mAnimatorClip;
        private static Animator         mAnimator;

        [MenuItem("Tools/Effect/KeyFrameCopyTool Copy %#c")]
        public static void CopyKeyFrames()
        {
            GameObject rSelectGo = Selection.activeGameObject;
            if (rSelectGo == null) return;

            mAnimator = rSelectGo.GetComponentInParent<Animator>();
            if (mAnimator == null) return;
            
            var rAnimationClips = mAnimator.runtimeAnimatorController.animationClips;
            if (rAnimationClips == null || rAnimationClips.Length == 0) return;
            mAnimatorClip = rAnimationClips[0];
            
            mSelectedNode = CreateNode(rSelectGo);
        }

        [MenuItem("Tools/Effect/KeyFrameCopyTool Paste %#v")]
        public static void PasteKeyFrames()
        {
            if (mSelectedNode == null || mSelectedNode.Node == null) return;

            // 复制Node GameObject
            mCopyNode = CopyNode(mSelectedNode);

            // 设置新复制的节点的对应的Animation Curve
            for (int i = 0; i < mCopyNode.Node.Curves.Count; i++)
            {
                AnimationUtility.SetEditorCurve(mAnimatorClip, mCopyNode.Node.Curves[i].CurveBinding, mCopyNode.Node.Curves[i].Curve);
            }
            for (int i = 0; i < mCopyNode.ChildNodes.Count; i++)
            {
                for (int j = 0; j < mCopyNode.ChildNodes[i].Curves.Count; j++)
                {
                    AnimationUtility.SetEditorCurve(mAnimatorClip, mCopyNode.ChildNodes[i].Curves[j].CurveBinding, mCopyNode.ChildNodes[i].Curves[j].Curve);
                }
            }

            mSelectedNode = null;
            mCopyNode = null;
            mAnimator = null;
            mAnimatorClip = null;
        }

        private static SelectedNode CreateNode(GameObject rSelectedGo)
        {
            var rSelectedNode = new SelectedNode();
            rSelectedNode.Node = new GameObjectNode(rSelectedGo);
            rSelectedNode.Node.Curves = GetCurves(rSelectedGo);

            rSelectedNode.ChildNodes = new List<GameObjectNode>();
            Transform[] rChildTransforms = rSelectedGo.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < rChildTransforms.Length; i++)
            {
                // 过滤掉自己
                if (rChildTransforms[i] == rSelectedGo.transform) continue;

                var rChildNode = new GameObjectNode(rChildTransforms[i].gameObject);
                rChildNode.Curves = GetCurves(rChildTransforms[i].gameObject);
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
            rCopyNode.Node.Curves = CopyCurves(rCopyNode.Node, rNode.Node.Curves);
            rCopyNode.ChildNodes = new List<GameObjectNode>();

            Transform[] rChildTransforms = rNewGo.GetComponentsInChildren<Transform>(true);
            int k = 0;
            for (int i = 0; i < rChildTransforms.Length; i++)
            {
                // 过滤掉自己
                if (rChildTransforms[i] == rNewGo.transform) continue;

                var rChildNode = new GameObjectNode(rChildTransforms[i].gameObject);
                rChildNode.Curves = CopyCurves(rChildNode, rNode.ChildNodes[k++].Curves);
                rCopyNode.ChildNodes.Add(rChildNode);
            }
            return rCopyNode;
        }

        private static List<AnimCurve> GetCurves(GameObject rTargetGo)
        {
            List<AnimCurve> rAnimCurves = new List<AnimCurve>();
            var rCurveBindings = AnimationUtility.GetAnimatableBindings(rTargetGo, rTargetGo.transform.root.gameObject);
            for (int i = 0; i < rCurveBindings.Length; i++)
            {
                var rAnimCurve = AnimationUtility.GetEditorCurve(mAnimatorClip, rCurveBindings[i]);
                if (rAnimCurve != null)
                    rAnimCurves.Add(new AnimCurve() { Curve = rAnimCurve, CurveBinding = rCurveBindings[i] });
            }
            return rAnimCurves;
        }
        
        private static List<AnimCurve> CopyCurves(GameObjectNode rNewNode, List<AnimCurve> rCurves)
        {
            List<AnimCurve> rNewCurves = new List<AnimCurve>();
            var rNewCurveBindings = new List<EditorCurveBinding>(AnimationUtility.GetAnimatableBindings(rNewNode.NodeGo, rNewNode.NodeGo.transform.root.gameObject));

            for (int i = 0; i < rCurves.Count; i++)
            {
                var rOldCurveBinding = rCurves[i].CurveBinding;
                var rNewCurveBinding = rNewCurveBindings.Find((rItem) => { return rItem.propertyName.Equals(rOldCurveBinding.propertyName); });
                AnimCurve rNewCurve = new AnimCurve() { Curve = rCurves[i].Curve, CurveBinding = rNewCurveBinding };
                rNewCurves.Add(rNewCurve);
            }
            return rNewCurves;
        }
    }
}