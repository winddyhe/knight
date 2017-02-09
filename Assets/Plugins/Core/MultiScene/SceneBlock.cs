//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.MultiScene
{
    public class SceneBlock
    {
        private List<TileIndex> mTiles;

        private int             mTileNum;

        public int TileCount
        {
            get { return mTiles != null ? mTiles.Count : 0; }
        }

        public List<TileIndex> Tiles
        {
            get { return mTiles; }
        }

        public SceneBlock(int rTileNum)
        {
            Debug.Assert(rTileNum % 2 == 1, "TileNum必须是奇数。");

            this.mTileNum = rTileNum;
        }

        public void Initialize(int rRow, int rCol)
        {
            this.mTiles = new List<TileIndex>();

            for (int i = rRow - mTileNum / 2; i <= rRow + mTileNum / 2; i++)
            {
                for (int j = rCol - mTileNum / 2; j <= rCol + mTileNum / 2; j++)
                {
                    this.mTiles.Add(new TileIndex(i, j));
                }
            }
        }

        public void AddDelta(int rDeltaRow, int rDeltaCol)
        {
            for (int i = 0; i < this.TileCount; i++)
            {
                this.mTiles[i].Add(rDeltaRow, rDeltaCol);
            }
        }

        public TileIndex this[int rIndex]
        {
            get { return mTiles != null ? mTiles[rIndex] : null; }
        }

        public TileIndex this[int rRow, int rCol]
        {
            get { return this.Get(rRow, rCol); }
        }

        public TileIndex Get(int rRow, int rCol)
        {
            if (mTiles == null) return null;

            return mTiles.Find((rItem) => { return rItem.Row == rRow && rItem.Col == rCol; });
        }

        public TileIndex GetCenter()
        {
            return this[((mTileNum / 2) * mTileNum + (mTileNum / 2))];
        }

        public static List<TileIndex> UnionBlock(List<TileIndex> rBlock1, List<TileIndex> rBlock2)
        {
            List<TileIndex> rReuslts = new List<TileIndex>(rBlock1);
            for (int i = 0; i < rBlock2.Count; i++)
            {
                if (rReuslts.FindIndex((rItem) => { return rItem.Equals(rBlock2[i]); }) == -1)
                {
                    rReuslts.Add(rBlock2[i]);
                }
            }
            return rReuslts;
        }

        public static List<TileIndex> DiffBlock(List<TileIndex> rBigBlock, List<TileIndex> rSmallBlock)
        {
            List<TileIndex> rResults = new List<TileIndex>(rBigBlock);
            for (int i = rResults.Count - 1; i >= 0; i--)
            {
                if (rSmallBlock.FindIndex((rItem) => { return rItem.Equals(rResults[i]); }) >= 0)
                {
                    rResults.RemoveAt(i);
                }
            }
            return rResults;
        }
    }
}