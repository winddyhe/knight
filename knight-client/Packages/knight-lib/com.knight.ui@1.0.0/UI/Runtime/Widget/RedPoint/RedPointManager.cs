using System.Collections.Generic;
using Knight.Core;

namespace Knight.Framework.UI
{
    public class RedPointManager : TSingleton<RedPointManager>
    {
        private RedPointNode mRootNode;
        private HashSet<RedPointNode> mDirtyNodes;

        private RedPointManager()
        {
        }

        public void Initialize()
        {
            this.mRootNode = new RedPointNode("Root", null);
            this.mDirtyNodes = new HashSet<RedPointNode>();
        }

        public RedPointNode AddRedPoint(string rRedPointPath)
        {
            var rRedPointNodeNames = rRedPointPath.Split('/');
            return this.FindOrBuildNode(rRedPointNodeNames);
        }

        public void SetNodeDirty(RedPointNode rNode)
        {
            this.mDirtyNodes.Add(rNode);
        }

        public void Update()
        {
            if (this.mDirtyNodes == null || this.mDirtyNodes.Count == 0) return;

            foreach (var rDirtyNode in this.mDirtyNodes)
            {
                rDirtyNode?.OnRedPointChanged(rDirtyNode.TotalNumber);
            }
            this.mDirtyNodes.Clear();
        }

        /// <summary>
        /// 查找红点节点, UI界面层使用
        /// </summary>
        public RedPointNode SearchRedPoint(string rRedPointPath)
        {
            if (string.IsNullOrEmpty(rRedPointPath)) return null;
            if (this.mRootNode == null) return null;

            var rRedPointNodeNames = rRedPointPath.Split('/');
            var rCurNode = this.mRootNode;
            for (int i = 0; i < rRedPointNodeNames.Length; i++)
            {
                var rNodeName = rRedPointNodeNames[i];
                if (rCurNode.ChildrenDict.TryGetValue(rNodeName, out var rNode))
                {
                    rCurNode = rNode;
                }
                else
                {
                    return null;
                }
            }
            return rCurNode;
        }

        private RedPointNode FindOrBuildNode(string[] rRedPointNodeNames)
        {
            var rCurNode = this.mRootNode;
            for (int i = 0; i < rRedPointNodeNames.Length; i++)
            {
                var rNodeName = rRedPointNodeNames[i];
                if (this.mRootNode.ChildrenDict.TryGetValue(rNodeName, out var rNode))
                {
                    rCurNode = rNode;
                }
                else
                {
                    var rNewNode = new RedPointNode(rNodeName, rCurNode);
                    rCurNode.ChildrenDict.Add(rNodeName, rNewNode);
                    rCurNode = rNewNode;
                }
            }
            return rCurNode;
        }

    }
}
