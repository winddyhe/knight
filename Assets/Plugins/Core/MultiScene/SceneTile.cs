//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Core.MultiScene
{
    /// <summary>
    /// 场景分块数据
    /// </summary>
    public class SceneTile : ScriptableObject
    {
        /// <summary>
        /// 场景块的ID
        /// </summary>
        public int      ID;
        /// <summary>
        /// 场景块的行号
        /// </summary>
        public int      Row;
        /// <summary>
        /// 场景块的列号
        /// </summary>
        public int      Col;
        /// <summary>
        /// 场景块对应的场景名
        /// </summary>
        public string   ParentID;
        /// <summary>
        /// 场景块的大小
        /// </summary>
        public int      Size;
        /// <summary>
        /// 场景快的偏移值
        /// </summary>
        public Vector2  Offset;
    }
    
    /// <summary>
    /// 场景块的索引封装
    /// </summary>
    public class TileIndex
    {
        /// <summary>
        /// Row表示X方向
        /// </summary>
        public int Row;
        /// <summary>
        /// Col表示Z方向
        /// </summary>
        public int Col;
    
        public static TileIndex Zero
        {
            get { return new TileIndex(0, 0); }
        }
    
        public TileIndex(int rRow, int rCol)
        {
            this.Reset(rRow, rCol);
        }
    
        public TileIndex(TileIndex rTileIndex)
        {
            this.Row = rTileIndex.Row;
            this.Col = rTileIndex.Col;
        }
    
        public void Reset(int rRow, int rCol)
        {
            this.Row = rRow;
            this.Col = rCol;
        }
    
        public static TileIndex operator ++(TileIndex rValue1)
        {
            rValue1.Row++;
            rValue1.Col++;
            return rValue1;
        }
    
        public static TileIndex operator --(TileIndex rValue1)
        {
            rValue1.Row--;
            rValue1.Col--;
            return rValue1;
        }
    
        /// <summary>
        /// @TODO: 慎用，会改变自身的值
        /// </summary>
        public static TileIndex operator +(TileIndex rValue1, int rValue2)
        {
            rValue1.Row += rValue2;
            rValue1.Col += rValue2;
            return rValue1;
        }
    
        public TileIndex Add(int rValue)
        {
            this.Row += rValue;
            this.Col += rValue;
            return this;
        }
    
        public TileIndex Add(int rRow, int rCol)
        {
            this.Row += rRow;
            this.Col += rCol;
            return this;
        }
    
        public TileIndex AddOnlyRow(int rValue)
        {
            this.Row += rValue;
            return this;
        }
    
        public TileIndex AddOnlyCol(int rValue)
        {
            this.Col += rValue;
            return this;
        }
    
        public bool Equals(TileIndex obj)
        {
            if (obj == null) return false;
            TileIndex rTileIndex = obj as TileIndex;
            if (rTileIndex == null) return false;
    
            return this.Row == rTileIndex.Row && this.Col == rTileIndex.Col;
        }
    
        public override string ToString()
        {
            return string.Format("TileIndex ({0}, {1})", this.Row, this.Col);
        }
    }
}