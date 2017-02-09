//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.MultiScene
{
    /// <summary>
    /// 多场景管理器
    /// </summary>
    public class MultiSceneManager : TSingleton<MultiSceneManager>
    {
        public static string        CurrentTestSceneName = "terrain_test1";
    
        /// <summary>
        /// 场景Block的边长
        /// </summary>
        private int                 mTileNum = 3;
        /// <summary>
        /// 大场景
        /// </summary>
        private LargeScene          mLargeScene;
        /// <summary>
        /// 当前所在的小块
        /// </summary>
        private TileIndex           mCurTileIndex;
        /// <summary>
        /// 场景block的缓冲区
        /// </summary>
        private SceneBlockBuffer    mSceneBlockBuffer;
    
        private MultiSceneManager() { }
    
        /// <summary>
        /// 多场景管理器的初始化
        /// </summary>
        public void Initialize(string rCurScene, int rTileNum, Vector3 rPos)
        {
            this.mTileNum = rTileNum;
            this.mLargeScene = Resources.Load<LargeScene>("MultiScene/" + rCurScene);
    
            // Test Scene
            CurrentTestSceneName = this.mLargeScene.ID;
    
            this.mSceneBlockBuffer = new SceneBlockBuffer(this.mTileNum);
    
            SceneBlockLoader.Instance.SetBuffer(this.mSceneBlockBuffer);
    
            this.Reset(rPos);
        }
    
        /// <summary>
        /// 加载一个场景Block
        /// </summary>
        public void Reset(Vector3 rPos)
        {
            int rRow = (int)(rPos.x - mLargeScene.Offset.x) / (int)(mLargeScene.Size.x / mLargeScene.TileRows);
            int rCol = (int)(rPos.z - mLargeScene.Offset.y) / (int)(mLargeScene.Size.z / mLargeScene.TileCols);
            Debug.LogFormat("当前的块: ({0}, {1})", rRow, rCol);
    
            mCurTileIndex = new TileIndex(rRow, rCol);
    
            SceneBlockLoader.Instance.ResetBuffer(mCurTileIndex);
        }
        
        /// <summary>
        /// 加载场景Tile
        /// </summary>
        public IEnumerator LoadTile(TileIndex rTileIndex)
        {
            yield return 0;
        }
    
        /// <summary>
        /// 更新
        /// </summary>
        public void UpdatePosition(Vector3 rPos)
        {
            int rNextRow = (int)(rPos.x - mLargeScene.Offset.x) / (int)(mLargeScene.Size.x / mLargeScene.TileRows);
            int rNextCol = (int)(rPos.z - mLargeScene.Offset.y) / (int)(mLargeScene.Size.z / mLargeScene.TileCols);
            
            int rDeltaRow = rNextRow - mCurTileIndex.Row;
            int rDeltaCol = rNextCol - mCurTileIndex.Col;
    
            if (rDeltaRow == 0 && rDeltaCol == 0) return;
    
            Debug.LogFormat("当前的块: ({0}, {1})", rNextRow, rNextCol);
    
            SceneBlockLoader.Instance.UpdateDelta(rDeltaRow, rDeltaCol);
    
            mCurTileIndex.Reset(rNextRow, rNextCol);
        }
    }

}

