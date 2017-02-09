//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.MultiScene
{
    public class SceneTileWrapper
    {
        public SceneTile    sceneTile;
    
        public TileIndex    curIndex;
        
        public GameObject   tileGo;
    
        public void Release()
        {
            if (this.tileGo != null)
            {
                GameObject.DestroyImmediate(this.tileGo);
                this.tileGo = null;
            }
        }
    
        public void SetActive(bool bActive)
        {
            if (this.tileGo != null)
            {
                this.tileGo.SetActive(bActive);
            }
        }
    }
    
    /// <summary>
    /// 场景Block的缓冲区，缓存算法，缓存上一个Block和当前Block的所有Tile即可
    /// </summary>
    public class SceneBlockBuffer
    {
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        private int                     mTileNum;
        /// <summary>
        /// 缓冲区
        private List<SceneTileWrapper>  mBlockBuffers;
    
        private SceneBlock              mCurBlock;
        private SceneBlock              mPrevBlock;
        /// <summary>
        /// MultiScene的根节点
        /// </summary>
        private GameObject              mMultiSceneRootGo;
    
        /// <summary>
        /// 当前的Block
        /// </summary>
        public SceneBlock               CurBlock
        {
            get { return mCurBlock; }
        }
        /// <summary>
        /// 前一个Block
        /// </summary>
        public SceneBlock               PrevBlock
        {
            get { return mPrevBlock; }
        }
    
        public int                      TileNum
        {
            get { return mTileNum; }
        }
    
        public SceneBlockBuffer(int rTileNum)
        {
            this.mTileNum = rTileNum;
            this.mBlockBuffers = new List<SceneTileWrapper>();
    
            this.mCurBlock = new SceneBlock(rTileNum);
            this.mPrevBlock = new SceneBlock(rTileNum);
    
            this.mMultiSceneRootGo = UtilTool.CreateGameObject("__multiSceneRoot");
        }
        
        /// <summary>
        /// 重置当前的缓冲区
        /// </summary>
        public void Reset(TileIndex rTileIndex)
        {
            this.mCurBlock.Initialize(rTileIndex.Row, rTileIndex.Col);
            this.mPrevBlock.Initialize(rTileIndex.Row, rTileIndex.Col);
        }
    
        /// <summary>
        /// 得到缓冲区里面的一个SceneTileWrapper
        /// </summary>
        public SceneTileWrapper Get(TileIndex rTileIndex)
        {
            if (mBlockBuffers == null) return null;
            return mBlockBuffers.Find((rItem) => { return rItem.curIndex.Equals(rTileIndex); });
        }
        
        /// <summary>
        /// 创建一个SceneTileWrapper
        /// </summary>
        public SceneTileWrapper Alloc(TileIndex rTileIndex, SceneTile rSceneTile, TerrainData rTerrainData)
        {
            Debug.Log("加载块: " + rTileIndex.ToString());
    
            var rSceneTileWrapper = new SceneTileWrapper();
            rSceneTileWrapper.curIndex = new TileIndex(rTileIndex);
            if (rSceneTile != null && rTerrainData != null)
            {
                rSceneTileWrapper.tileGo = Terrain.CreateTerrainGameObject(rTerrainData);
                rSceneTileWrapper.tileGo.name = string.Format("tile_{0}_{1}", rTileIndex.Row, rTileIndex.Col);
                rSceneTileWrapper.tileGo.transform.parent = mMultiSceneRootGo.transform;
                rSceneTileWrapper.tileGo.transform.position = new Vector3(rSceneTile.Offset.x, 0, rSceneTile.Offset.y);
                rSceneTileWrapper.sceneTile = rSceneTile;
            }
            mBlockBuffers.Add(rSceneTileWrapper);
            return rSceneTileWrapper;
        }
    
        public void LinkTerrain()
        {
            for (int i = 0; i < mCurBlock.TileCount; i++)
            {
    
            }
        }
    
        public void Release(TileIndex rTileIndex)
        {
            if (mBlockBuffers == null) return;
    
            SceneTileWrapper rSceneTileWrapper = mBlockBuffers.Find((rItem) => { return rItem.curIndex.Equals(rTileIndex); });
            if (rSceneTileWrapper != null)
            {
                rSceneTileWrapper.Release();
                Debug.Log("删除块: " + rTileIndex.ToString());
                mBlockBuffers.Remove(rSceneTileWrapper);
            }
        }
    
        public void SetActive(TileIndex rTileIndex, bool bActive)
        {
            SceneTileWrapper rSceneTileWrapper = this.Get(rTileIndex);
            if (rSceneTileWrapper != null)
            {
                rSceneTileWrapper.SetActive(bActive);
            }
        }
    }
}