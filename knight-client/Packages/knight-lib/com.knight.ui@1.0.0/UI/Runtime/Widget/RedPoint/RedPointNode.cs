using System;
using System.Collections.Generic;

namespace Knight.Framework.UI
{
    public class RedPointNode
    {
        public string NodeName;
        public int Number;      // 自己的数量
        public int TotalNumber; // 总和数量 所有子节点的数量加上自己的数量

        public RedPointNode Parent;
        public Dictionary<string, RedPointNode> ChildrenDict;

        public Action<int> OnRedPointChanged;

        public RedPointNode(string rNodeName, RedPointNode rParent)
            : this(rNodeName, rParent, 0)
        {
        }

        public RedPointNode(string rNodeName, RedPointNode rParent, int nNumber)
        {
            this.NodeName = rNodeName;
            this.Parent = rParent;
            this.Number = nNumber;
            this.ChildrenDict = new Dictionary<string, RedPointNode>();
        }

        public void SetNumber(int nNumber)
        {
            var nOriginalNumber = this.Number;
            this.Number = nNumber;
            var nDeltaNumber = nNumber - nOriginalNumber;
            nDeltaNumber = Math.Max(nDeltaNumber, 0);
            if (nDeltaNumber == 0) return;

            this.TotalNumber += nDeltaNumber;
            RedPointManager.Instance.SetNodeDirty(this);

            // 更新父节点的差量
            var rParentNode = this.Parent;
            while (rParentNode != null)
            {
                rParentNode.TotalNumber += nDeltaNumber;
                RedPointManager.Instance.SetNodeDirty(rParentNode);
                rParentNode = rParentNode.Parent;
            }
        }
    }
}
