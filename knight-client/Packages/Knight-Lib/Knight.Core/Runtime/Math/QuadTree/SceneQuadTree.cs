//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core.Math
{
    public class SceneQuadTreeNode
    {
        public SceneQuadTreeNode    Parent;
        public SceneQuadTreeNode[]  Children;

        public int                  ID;                 // 节点ID
        public List<SceneObject>    NodeList;           // 存放场景节点
        public float                Length;             // 节点的边长
        public float                LooseFactor;        // 节点的松散系数
        public int                  Depth;              // 节点的深度
        public Vector3              Center;             // 节点的包围盒
        public int                  SubtreeObjCount;    // 以该节点为根节点的子树的游戏物体的个数
        public Rect                 AABB;               // AABB Rect
    }

    public class SceneQuadTree
    {
        public Vector3[]            QuadDirs = new Vector3[] 
        {
            new Vector3(-1, 0, -1),         // 00       xz
            new Vector3( 1, 0, -1),         // 01
            new Vector3(-1, 0,  1),         // 10
            new Vector3( 1, 0,  1)          // 11
        };

        public Color[]              DepthColors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.black,
            Color.white,
            Color.yellow,
            Color.cyan
        };

        public int                  MaxDepth;
        public float                LooseFactor;
        public SceneQuadTreeNode    TreeRoot;
        public Vector3              Center;
        public float                Length;

        public void Initialize(int nMaxDepth, float fLooseFactor, Vector3 rCenter, float fLength)
        {
            this.MaxDepth = nMaxDepth;
            this.LooseFactor = fLooseFactor;
            this.Center = rCenter;
            this.Length = fLength;

            this.Build(null, 0, ref this.TreeRoot, 0, this.Center, this.Length);
        }

        public void Build(SceneQuadTreeNode rParentNode, int nIndex, ref SceneQuadTreeNode rQuadNode, int nDepth, Vector3 rCenter, float fLength)
        {
            rQuadNode = new SceneQuadTreeNode();
            rQuadNode.ID = nIndex;
            rQuadNode.Parent = rParentNode;
            rQuadNode.Children = new SceneQuadTreeNode[4];
            rQuadNode.NodeList = new List<SceneObject>();
            rQuadNode.Center = rCenter;
            rQuadNode.Length = fLength;
            rQuadNode.LooseFactor = this.LooseFactor;
            rQuadNode.Depth = nDepth;
            rQuadNode.SubtreeObjCount = 0;

            var fLooseLength = fLength;
            rQuadNode.AABB = new Rect(rCenter.x - fLooseLength / 2.0f, rCenter.z - fLooseLength / 2.0f, fLooseLength, fLooseLength);

            if (nDepth == this.MaxDepth - 1) return;

            for (int i = 0; i < 4; i++)
            {
                var rChildCenter = rCenter + this.QuadDirs[i] * fLength * 0.25f;
                this.Build(rQuadNode, i, ref rQuadNode.Children[i], nDepth + 1, rChildCenter, fLength * 0.5f);
            }
        }

        public void ClearAllNodes()
        {
            this.ClearAllNodes(this.TreeRoot);
        }

        private void ClearAllNodes(SceneQuadTreeNode rTreeNode)
        {
            if (rTreeNode == null) return;

            rTreeNode.SubtreeObjCount = 0;
            rTreeNode.NodeList.Clear();
            for (int i = 0; i < rTreeNode.Children.Length; i++)
            {
                this.ClearAllNodes(rTreeNode.Children[i]);
            }
        }

        public SceneQuadTreeNode AddNode(SceneObject rGo)
        {
            // 计算四叉树的深度层次
            var rBoxCollider = rGo.GameObject.GetComponentInChildren<BoxCollider>();
            var rBounds = rBoxCollider.bounds;
            var fObjRadius = Mathf.Max(rBounds.size.x, rBounds.size.z);
            var nCurDepth = Mathf.FloorToInt(Mathf.Log(this.LooseFactor * this.Length / fObjRadius, 2.0f) - 1);
            nCurDepth = Mathf.Clamp(0, this.MaxDepth - 1, nCurDepth);
            
            var nTempDepth = 0;
            SceneQuadTreeNode rCurNode = this.TreeRoot;
            while(nTempDepth < nCurDepth)
            {
                var nIndex = ((rGo.GameObject.transform.position.x > rCurNode.Center.x) ? 1 : 0) |
                             ((rGo.GameObject.transform.position.z > rCurNode.Center.z) ? 2 : 0);
                rCurNode = rCurNode.Children[nIndex];
                nTempDepth++;
            }
            rCurNode.NodeList.Add(rGo);
            rCurNode.SubtreeObjCount++;

            // 增加子节点的个数
            var rParentNode = rCurNode.Parent;
            while (rParentNode != null)
            {
                rParentNode.SubtreeObjCount++;
                rParentNode = rParentNode.Parent;
            }
            return rCurNode;
        }

        public void GetAllIntersectSceneGos(SceneObject rSceneGo, List<SceneQuadTreeNode> rQuadTreeNodes)
        {
            rQuadTreeNodes.Clear();
            this.GetAllIntersectSceneGos(rSceneGo, rQuadTreeNodes, this.TreeRoot);
        }

        private void GetAllIntersectSceneGos(SceneObject rSceneGo, List<SceneQuadTreeNode> rQaudTreeNodes, SceneQuadTreeNode rQuadNode)
        {
            if (rQuadNode == null) return;
            if (!rSceneGo.BoxCollider)
            {
                return;
            }
            var rRect = new Rect(
                rSceneGo.BoxCollider.bounds.center.x - rSceneGo.BoxCollider.bounds.size.x / 2.0f, 
                rSceneGo.BoxCollider.bounds.center.z - rSceneGo.BoxCollider.bounds.size.z / 2.0f, 
                rSceneGo.BoxCollider.bounds.size.x, 
                rSceneGo.BoxCollider.bounds.size.z);

            if (!rQuadNode.AABB.Overlaps(rRect)) return;

            if (rQuadNode.NodeList.Count > 0)
                rQaudTreeNodes.Add(rQuadNode);

            for (int i = 0; i < rQuadNode.Children.Length; i++)
            {
                this.GetAllIntersectSceneGos(rSceneGo, rQaudTreeNodes, rQuadNode.Children[i]);
            }
        }

        public void UpdateNode(SceneObject rSceneGo)
        {
            var rBoxCollider = rSceneGo.GameObject.GetComponentInChildren<BoxCollider>();
            var rBounds = rBoxCollider.bounds;
            var fObjRadius = Mathf.Max(rBounds.size.x, rBounds.size.z);
            var nCurDepth = Mathf.FloorToInt(Mathf.Log(this.LooseFactor * this.Length / fObjRadius, 2.0f) - 1);
            nCurDepth = Mathf.Clamp(0, this.MaxDepth - 1, nCurDepth);
            
            var nTempDepth = 0;
            SceneQuadTreeNode rCurNode = this.TreeRoot;
            while (nTempDepth < nCurDepth)
            {
                var nIndex = ((rBounds.center.x > rCurNode.Center.x) ? 1 : 0) |
                             ((rBounds.center.z > rCurNode.Center.z) ? 2 : 0);
                rCurNode = rCurNode.Children[nIndex];
                nTempDepth++;
            }
            if (rSceneGo.QuadNode == rCurNode) return;
            
            // 替换
            rSceneGo.QuadNode.NodeList.Remove(rSceneGo);
            rSceneGo.QuadNode.SubtreeObjCount--;
            var rParentNode = rSceneGo.QuadNode.Parent;
            while (rParentNode != null)
            {
                rParentNode.SubtreeObjCount--;
                rParentNode = rParentNode.Parent;
            }

            rSceneGo.QuadNode = rCurNode;
            rSceneGo.QuadNode.NodeList.Add(rSceneGo);
            rSceneGo.QuadNode.SubtreeObjCount++;
            rParentNode = rSceneGo.QuadNode.Parent;
            while (rParentNode != null)
            {
                rParentNode.SubtreeObjCount++;
                rParentNode = rParentNode.Parent;
            }
        }
        
        public void DebugDraw(SceneQuadTreeNode rQuadNode)
        {
            if (rQuadNode == null || rQuadNode.SubtreeObjCount == 0) return;

            var rColor = new Color(this.DepthColors[rQuadNode.Depth].r, this.DepthColors[rQuadNode.Depth].g, this.DepthColors[rQuadNode.Depth].b, 1f);
            Gizmos.color = this.DepthColors[rQuadNode.Depth];
            Gizmos.DrawWireCube(rQuadNode.Center, new Vector3(rQuadNode.Length, 1, rQuadNode.Length));
            for (int i = 0; i < rQuadNode.Children.Length; i++)
            {
                this.DebugDraw(rQuadNode.Children[i]);
            }
        }
    }
}
