using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

namespace Framework.TiledMap
{
    public class TiledMapManager : MonoBehaviour
    {
        public int              TiledWidth      = 10;
        public int              TiledHeight     = 10;
        public int              TiledNum        = 3;

        public List<TiledMap>   BaseTiles;
        public List<TiledMap>   GenerateTiles;

        public void Initialize(Vector3 rInitPos, int nTiledWidth, int nTiledHeight, int nTileNum, RandGenerator rRandGenerator)
        {
            this.TiledWidth  = nTiledWidth;
            this.TiledHeight = nTiledHeight;
            this.TiledNum    = nTileNum;

            int nBaseTileCount = this.BaseTiles.Count;
            if (nBaseTileCount == 0) return;

            this.GenerateTiles = new List<TiledMap>();

            // 随机第一块地图
            TiledIndex rCurIndex = new TiledIndex();
            TiledMap rFirstTile = this.RandomGenerateTile(rCurIndex, rRandGenerator);
            this.GenerateTiles.Add(rFirstTile);

            int nTileCount = nTileNum * nTileNum;
            int nMoveStep = 0;
            var nCurMoveDir = (int)TiledDirection.Up;

            int i = 1;
            while(i < nTileCount)
            {
                if (nCurMoveDir == (int)TiledDirection.Up || nCurMoveDir == (int)TiledDirection.Down)
                {
                    nMoveStep++;
                }
                for (int k = 0; k < nMoveStep; k++)
                {
                    if (i >= nTileCount) break;
                    rCurIndex = TiledIndex.TiledMoveActions[(TiledDirection)nCurMoveDir](rCurIndex);
                    TiledMap rGenerateTile = this.RandomGenerateTile_CheckEdge(rCurIndex, rRandGenerator);
                    this.GenerateTiles.Add(rGenerateTile);
                    i++;
                }
                nCurMoveDir++;
                nCurMoveDir = nCurMoveDir % 4;
            }
        }

        public void Destroy()
        {
            if (this.GenerateTiles == null) return;
            for (int i = 0; i < this.GenerateTiles.Count; i++)
            {
                GameObject.DestroyImmediate(this.GenerateTiles[i].gameObject);
            }
            this.GenerateTiles.Clear();
        }

        private TiledMap RandomGenerateTile(TiledIndex rTiledIndex, RandGenerator rRandGenerator)
        {
            int nBaseTileCount = this.BaseTiles.Count;
            TiledMap rTiledMap = this.GenerateTile(this.BaseTiles, rRandGenerator.Range(0, nBaseTileCount));
            rTiledMap.transform.parent = this.transform;
            rTiledMap.transform.localPosition = new Vector3(rTiledIndex.X * this.TiledWidth , 0, rTiledIndex.Z * this.TiledHeight);
            rTiledMap.name = "tile_" + rTiledIndex.X + "_" + rTiledIndex.Z;
            rTiledMap.TiledIndex = rTiledIndex;
            return rTiledMap;
        }
        
        private TiledMap RandomGenerateTile_CheckEdge(TiledIndex rTiledIndex, RandGenerator rRandGenerator)
        {
            // 遍历当前块周围有没有地块
            List<TiledMap> rCurUsedBaseTiles = new List<TiledMap>(this.BaseTiles);
            for (int i = 0; i < 4; i++)
            {
                TiledIndex rEdgeTile = TiledIndex.TiledMoveActions[(TiledDirection)i](rTiledIndex);
                TiledMap rEdgeTiledMap = this.GenerateTiles.Find((rItem) => { return TiledIndex.IsEquals(rItem.TiledIndex, rEdgeTile); });
                if (rEdgeTiledMap == null) continue;

                HashSet<TiledMap> rValidTileMaps = new HashSet<TiledMap>();
                for (int k = 0; k < rCurUsedBaseTiles.Count; k++)
                {
                    if (rEdgeTiledMap.CheckIsSameEdge((TiledDirection)i, rCurUsedBaseTiles[k]))
                    {
                        rValidTileMaps.Add(rCurUsedBaseTiles[k]);
                    }
                }
                rCurUsedBaseTiles = new List<TiledMap>(rValidTileMaps);
            }
            
            // 随机剩余的块
            int nBaseTileCount = rCurUsedBaseTiles.Count;
            TiledMap rTiledMap = null; 
            if (nBaseTileCount > 0)
                rTiledMap = this.RandomTiledMap_Weight(rCurUsedBaseTiles, rRandGenerator);
            else
                rTiledMap = this.GenerateTile(this.BaseTiles, 0);

            rTiledMap.transform.parent = this.transform;
            rTiledMap.transform.localPosition = new Vector3(rTiledIndex.X * this.TiledWidth, 0, rTiledIndex.Z * this.TiledHeight);
            rTiledMap.name = "tile_" + rTiledIndex.X + "_" + rTiledIndex.Z;
            rTiledMap.TiledIndex = rTiledIndex;

            return rTiledMap;
        }
        
        private TiledMap GenerateTile(List<TiledMap> rBaseTiles, int nTiledIndex)
        {
            TiledMap rBaseTile = rBaseTiles[nTiledIndex];
            GameObject rInstTileGo = GameObject.Instantiate(rBaseTile.gameObject);
            return rInstTileGo.GetComponent<TiledMap>();
        }
        
        /// <summary>
        /// 带权值的随机TiledMap
        /// </summary>
        private TiledMap RandomTiledMap_Weight(List<TiledMap> rBaseTiles, RandGenerator rRandGenerator)
        {
            var rWeightedList = new Dictionary<int, List<TiledMap>>();
            var rWeights = new List<int>();
            for (int i = 0; i < rBaseTiles.Count; i++)
            {
                List<TiledMap> rTiles = null;
                if (rWeightedList.TryGetValue(rBaseTiles[i].RandWeight, out rTiles))
                {
                    rTiles.Add(rBaseTiles[i]);
                }
                else
                {
                    rTiles = new List<TiledMap>();
                    rTiles.Add(rBaseTiles[i]);
                    rWeightedList.Add(rBaseTiles[i].RandWeight, rTiles);
                }

                if (!rWeights.Contains(rBaseTiles[i].RandWeight))
                    rWeights.Add(rBaseTiles[i].RandWeight);
            }

            // 排序
            rWeights.Sort((a, b) => { return a.CompareTo(b); });

            float fTotalValue = 0.0f;
            for (int i = 0; i < rWeights.Count; i++)
            {
                fTotalValue += rWeights[i];
            }

            float fCurValue = 0.0f;
            float fRandValue = rRandGenerator.Range01();
            int nWeight = rWeights[0];
            for (int i = 0; i < rWeights.Count; i++)
            {
                fCurValue += rWeights[i] / fTotalValue;
                if (fRandValue <= fCurValue)
                {
                    nWeight = rWeights[i];
                }
            }

            return this.GenerateTile(rWeightedList[nWeight], rRandGenerator.Range(0, rWeightedList[nWeight].Count));
        }
    }
}


