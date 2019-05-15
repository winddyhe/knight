//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core.Math
{
    public class SceneQuadTreeDebugger : MonoBehaviour
    {
        private static SceneQuadTreeDebugger    __instance;
        public  static SceneQuadTreeDebugger    Instance { get { return __instance; } }
        
        public SceneQuadTree                    QuadTree;

        private void Awake()
        {
            __instance = this;
        }

        private void OnDrawGizmos()
        {
            if (this.QuadTree == null || this.QuadTree.TreeRoot == null) return;
            this.QuadTree.DebugDraw(this.QuadTree.TreeRoot);
        }
    }
}
