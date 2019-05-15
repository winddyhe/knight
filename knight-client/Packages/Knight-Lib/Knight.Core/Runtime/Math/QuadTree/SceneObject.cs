//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core.Math
{
    public class SceneObject
    {
        public int                  __id;
        public SceneQuadTreeNode    QuadNode;
        public GameObject           GameObject;
        public BoxCollider          BoxCollider;
    }
}
