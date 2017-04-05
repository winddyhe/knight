//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.MultiScene
{
    public class SceneTileRequest : CoroutineRequest<SceneTileRequest>
    {
        public SceneTileWrapper SceneTileWrapper;
    
        public TileIndex        CurIndex;
    
        public bool             IsRunning;
    
        public bool             IsCompleted;
    
        public SceneTileRequest(TileIndex rTileIndex)
        {
            this.CurIndex = new TileIndex(rTileIndex);
        }
    }
    
    public class SceneBlockLoader : MonoBehaviour
    {
        private static SceneBlockLoader __instance;
        public  static SceneBlockLoader Instance { get { return __instance; } }
    
        private SceneBlockBuffer        mSceneBlockBuffer;
        
        private List<SceneTileRequest>  mRunningRequests;
    
        private List<SceneTileRequest>  mRemovedRequests;
    
        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
                mRunningRequests = new List<SceneTileRequest>();
                mRemovedRequests = new List<SceneTileRequest>();
            }
        }
    
        public void SetBuffer(SceneBlockBuffer rBlockBuffer)
        {
            this.mSceneBlockBuffer = rBlockBuffer;
        }
    
        void Update()
        {
            if (mSceneBlockBuffer == null) return;
    
            for (int i = 0; i < mRunningRequests.Count; i++)
            {
                // 检测那些是需要开始的加载的
                if (!mRunningRequests[i].IsRunning && !mRunningRequests[i].IsCompleted)
                {
                    mRunningRequests[i].Start(this.HandleRequest(mRunningRequests[i]));
                    mRunningRequests[i].IsRunning = true;
                }
    
                // 检测哪些是加载完了需要删掉的
                if (!mRunningRequests[i].IsRunning && mRunningRequests[i].IsCompleted)
                {
                    mRemovedRequests.Add(mRunningRequests[i]);
                }
            }
    
            // 删除那些已经加载完成的Request
            for (int i = 0; i < mRemovedRequests.Count; i++)
            {
                mRemovedRequests[i].Stop();
                mRunningRequests.Remove(mRemovedRequests[i]);
            }
            mRemovedRequests.Clear();
        }
    
        /// <summary>
        /// 重置缓冲区
        /// </summary>
        public void ResetBuffer(TileIndex rTileIndex)
        {
            mSceneBlockBuffer.Reset(rTileIndex);
            SceneBlock rCurBlock = mSceneBlockBuffer.CurBlock;
            for (int i = 0; i < rCurBlock.TileCount; i++)
            {
                CreateLoadRequest(rCurBlock[i]);
            }
        }
    
        /// <summary>
        /// 创建一个加载请求
        /// </summary>
    
        public void CreateLoadRequest(TileIndex rTileIndex)
        {
            var rRequest = mRunningRequests.Find((rItem)=> { return rItem.CurIndex.Equals(rTileIndex); });
            if (rRequest != null) return;
    
            rRequest = new SceneTileRequest(rTileIndex);
            mRunningRequests.Add(rRequest);
        }
    
        public void ReleaseLoadRequest(TileIndex rTileIndex)
        {
            var rRequest = mRunningRequests.Find((rItem) => { return rItem.CurIndex.Equals(rTileIndex); });
            if (rRequest != null) rRequest.Stop();
            
            mSceneBlockBuffer.Release(rTileIndex);
        }
    
        /// <summary>
        /// 加载单个场景块
        /// </summary>
        public IEnumerator HandleRequest(SceneTileRequest rSceneTileRequest)
        {
            TileIndex rTileIndex = rSceneTileRequest.CurIndex;
    
            SceneTileWrapper rSceneTileWrapper = mSceneBlockBuffer.Get(rTileIndex);
            if (rSceneTileWrapper != null)
            {
                rSceneTileRequest.SceneTileWrapper = rSceneTileWrapper;
                rSceneTileWrapper.SetActive(true);
                rSceneTileRequest.IsRunning = false;
                rSceneTileRequest.IsCompleted = true;
                mSceneBlockBuffer.SetActive(rTileIndex, true);
                yield break;
            }
    
            string rSceneTileConfigPath = string.Format("{0}/tile_{1}_{2}_config", MultiSceneManager.CurrentTestSceneName, rTileIndex.Row, rTileIndex.Col);
            string rSceneTilePath = string.Format("{0}/tile_{1}_{2}", MultiSceneManager.CurrentTestSceneName, rTileIndex.Row, rTileIndex.Col);
            
            ResourceRequest rRequest = Resources.LoadAsync<SceneTile>(rSceneTileConfigPath);
            yield return rRequest;
            SceneTile rSceneTile = rRequest.asset as SceneTile;
    
            ResourceRequest rDataRequest = Resources.LoadAsync<TerrainData>(rSceneTilePath);
            yield return rDataRequest;
            TerrainData rTerrainData = rDataRequest.asset as TerrainData;
            
            rSceneTileWrapper = mSceneBlockBuffer.Alloc(rTileIndex, rSceneTile, rTerrainData);
    
            rSceneTileRequest.SceneTileWrapper = rSceneTileWrapper;
            rSceneTileRequest.IsRunning = false;
            rSceneTileRequest.IsCompleted = true;
        }
    
        public void UpdateDelta(int rDeltaRow, int rDeltaCol)
        {
            if (mSceneBlockBuffer == null) return;
    
            List<TileIndex> rPrevIndices = new List<TileIndex>(mSceneBlockBuffer.PrevBlock.Tiles);
    
            TileIndex rCenterTile = mSceneBlockBuffer.CurBlock.GetCenter();
            if (rCenterTile == null) return;
            mSceneBlockBuffer.PrevBlock.Initialize(rCenterTile.Row, rCenterTile.Col);
            mSceneBlockBuffer.CurBlock.AddDelta(rDeltaRow, rDeltaCol);
            
            List<TileIndex> rMiddle1Block = SceneBlock.UnionBlock(mSceneBlockBuffer.CurBlock.Tiles, mSceneBlockBuffer.PrevBlock.Tiles);
            List<TileIndex> rMiddle2Block = SceneBlock.UnionBlock(mSceneBlockBuffer.PrevBlock.Tiles, rPrevIndices);
            List<TileIndex> rBigBlock = SceneBlock.UnionBlock(rMiddle1Block, rPrevIndices);
    
            List<TileIndex> rDeleteIndices = SceneBlock.DiffBlock(rBigBlock, rMiddle1Block);
            List<TileIndex> rDeactiveIndices = SceneBlock.DiffBlock(rBigBlock, mSceneBlockBuffer.CurBlock.Tiles);
            List<TileIndex> rLoadIndices = SceneBlock.DiffBlock(rBigBlock, rMiddle2Block);
    
            for (int i = 0; i < rDeactiveIndices.Count; i++)
            {
                mSceneBlockBuffer.SetActive(rDeactiveIndices[i], false);
            }
    
            for (int i = 0; i < rDeleteIndices.Count; i++)
            {
                ReleaseLoadRequest(rDeleteIndices[i]);
            }
    
            for (int i = 0; i < mSceneBlockBuffer.CurBlock.TileCount; i++)
            {
                mSceneBlockBuffer.SetActive(mSceneBlockBuffer.CurBlock[i], true);
            }
    
            for (int i = 0; i < rLoadIndices.Count; i++)
            {
                CreateLoadRequest(rLoadIndices[i]);
            }
        }
    }
}