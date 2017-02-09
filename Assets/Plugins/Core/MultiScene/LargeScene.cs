//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Core.MultiScene
{
    /// <summary>
    /// 大场景的数据结构
    /// </summary>
    public class LargeScene : ScriptableObject
    {
        /// <summary>
        /// 场景名
        /// </summary>
        public string   ID;
        /// <summary>
        /// 场景的行号
        /// </summary>
        public int      SceneRow;
        /// <summary>
        /// 场景的列号
        /// </summary>
        public int      SceneCol;
        /// <summary>
        /// 场景的大小
        /// </summary>
        public Vector3  Size;
        /// <summary>
        /// 场景中地形块的行数
        /// </summary>
        public int      TileRows;
        /// <summary>
        /// 场景中地形块的列数
        /// </summary>
        public int      TileCols;
        /// <summary>
        /// 大场景的偏移值
        /// </summary>
        public Vector2  Offset;
        /// <summary>
        /// 相邻的场景，相邻的没有场景了为""
        /// </summary>
        public string   LeftNeighbor;
        public string   RightNeighbor;
        public string   TopNeighbor;
        public string   BottomNeighbor;
    }
}