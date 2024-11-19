using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Game.Editor
{

    public class DropdownSearch : DropdownSearch<string>
    {
        public DropdownSearch(List<string> rItemList, AdvancedDropdownState state, char rSplitChar) : base(rItemList, state, rSplitChar)
        {
        }
    }
    public class DropdownSearch<T> : AdvancedDropdown
    {
        public class Node : AdvancedDropdownItem
        {
            public T Data = default;
            public Dictionary<string, Node> ChildDict = new Dictionary<string, Node>();

            public Node(string rName, T rData) : base(rName)
            {
                this.Data = rData;
            }
            public bool TryGetChild(string rName, out Node rNode)
            {
                foreach (var rChildren in this.children)
                {
                    if (rChildren.name == rName)
                    {
                        rNode = (Node)rChildren;
                        return true;
                    }
                }
                rNode = null;
                return false;
            }
        }

        public Action<T> OnItemChanged;
        private List<T> mItemList;
        private AdvancedDropdownItem mRoot;

        public void CreateNode(string rNodePath, Node rRootNode, T rItem, char rSplitChar = '/')
        {
            if (string.IsNullOrEmpty(rNodePath)) return;
            var rNodePaths = rNodePath.Split(rSplitChar);
            Node rCurNode = rRootNode;
            for (int i = 0; i < rNodePaths.Length; i++)
            {
                var bIsEnd = rNodePaths.Length - 1 == i;
                var rNodeName = rNodePaths[i];
                if (!rCurNode.TryGetChild(rNodeName, out var rNode))
                {
                    rNode = new Node(rNodeName, bIsEnd ? rItem : default);
                    rCurNode.AddChild(rNode);
                }
                rCurNode = rNode;
            }
            return;
        }
        public Node GenerateRootNode(List<T> rItemList, char rSplitChar)
        {
            var rRootNode = new Node("Search", default);
            for (int i = 0; i < rItemList.Count; i++)
            {
                this.CreateNode(rItemList[i].ToString(), rRootNode, rItemList[i], rSplitChar);
            }
            return rRootNode;
        }
        public DropdownSearch(List<T> rItemList, AdvancedDropdownState state, char rSplitChar = '/') : base(state)
        {
            this.mItemList = rItemList;
            this.mRoot = this.GenerateRootNode(rItemList, rSplitChar);
        }
        public void SetMinSize(Vector2 rSize)
        {
            this.minimumSize = rSize;
        }
        protected override AdvancedDropdownItem BuildRoot()
        {
            return this.mRoot;
        }
        protected override void ItemSelected(AdvancedDropdownItem rAdvancedDropdownItem)
        {
            if (rAdvancedDropdownItem is Node rNode)
            {
                foreach (var item in this.mItemList)
                {
                    if (rNode.Data.Equals(item))
                    {
                        this.OnItemChanged?.Invoke(item);
                        break;
                    }
                }
            }
        }
        public static void Draw(T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged, char rSplitChar, params GUILayoutOption[] rOptions)
        {
            if (rItemList == null)
            {
                rItemList = new List<T>();
            }
            var rButtonName = "None";
            if (rItemList.Contains(rSelectItem))
            {
                rButtonName = rSelectItem.ToString();
            }
            else
            {
                if (rSelectItem == null)
                {
                    if (rItemList.Count != 0)
                    {
                        rButtonName = rItemList[0].ToString();
                        rOnItemChanged?.Invoke(rItemList[0]);
                    }
                    else
                    {
                        rSelectItem = default;
                        rOnItemChanged?.Invoke(rSelectItem);
                    }
                }
            }
            var rButtonRect = GUILayoutUtility.GetRect(new GUIContent(rButtonName), EditorStyles.popup, rOptions);
            if (GUI.Button(rButtonRect, rButtonName, EditorStyles.popup))
            {
                var rDropdownSearch = new DropdownSearch<T>(rItemList, new AdvancedDropdownState(), rSplitChar);
                rDropdownSearch.OnItemChanged = rOnItemChanged;
                rDropdownSearch.Show(rButtonRect);
            }
        }

        public static void Draw(T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged, params GUILayoutOption[] rOptions)
        {
            Draw(rSelectItem, rItemList, rOnItemChanged, '/', rOptions);
        }
        public static void DrawBox(string rTitle, T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged, char rSplitChar, params GUILayoutOption[] rOptions)
        {
            using (var _ = new EditorGUILayout.VerticalScope("Box", rOptions))
            {
                EditorGUILayout.LabelField(rTitle, rOptions);
                Draw(rSelectItem, rItemList, rOnItemChanged, rSplitChar, rOptions);
            }
        }
        public static void DrawBox(string rTitle, T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged, char rSplitChar, float fWidth)
        {
            using (var _ = new EditorGUILayout.VerticalScope("Box", GUILayout.Width(fWidth)))
            {
                EditorGUILayout.LabelField(rTitle, GUILayout.Width(fWidth));
                Draw(rSelectItem, rItemList, rOnItemChanged, rSplitChar, GUILayout.Width(fWidth));
            }
        }
        public static void DrawBox(string rTitle, T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged, float fWidth)
        {
            DrawBox(rTitle, rSelectItem, rItemList, rOnItemChanged, '/', fWidth);
        }
        public static void DrawBox(string rTitle, T rSelectItem, List<T> rItemList, Action<T> rOnItemChanged)
        {
            DrawBox(rTitle, rSelectItem, rItemList, rOnItemChanged, '/', 100);
        }
    }
}
