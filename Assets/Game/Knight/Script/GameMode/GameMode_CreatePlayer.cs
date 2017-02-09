//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using Core;
using System.Collections.Generic;
using Framework.WindUI;

namespace Game.Knight
{
    public class GameMode_CreatePlayer : GameMode
    {
        #region StageTask
        /// <summary>
        /// 加载角色资源的StageTask
        /// </summary>
        public class StageTask_LoadAssets : StageTask
        {
            public GameMode_CreatePlayer GameMode;

            public StageTask_LoadAssets(GameMode_CreatePlayer rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "LoadAssets";
                return true;
            }

            protected override IEnumerator OnRun_Async()
            {
                // 加载场景
                var rSceneRequest = SceneAssetLoader.Instance.Load_Async(
                    this.GameMode.StageConfig.SceneABPath,
                    this.GameMode.StageConfig.ScenePath, 
                    UnityEngine.SceneManagement.LoadSceneMode.Additive);

                yield return rSceneRequest.Coroutine;
                
                Debug.Log("GameStage -- Load assets complete.");
            }
        }

        /// <summary>
        /// 初始化数据， 加载界面
        /// </summary>
        public class StageTask_InitData : StageTask
        {
            public GameMode_CreatePlayer GameMode;

            public StageTask_InitData(GameMode_CreatePlayer rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "InitData";

                return true;
            }

            protected override IEnumerator OnRun_Async()
            {
                string rViewName = "KNCreatePlayer";
                if (Account.Instance.NetActors != null && Account.Instance.NetActors.Count > 0)
                    rViewName = "KNPlayerList";
                yield return UIManager.Instance.OpenAsync(rViewName, View.State.dispatch);

                GameLoading.Instance.Hide();

                Debug.Log("GameStage -- Init data complete.");
                yield break;
            }
        }
        #endregion

        #region GameMode

        protected override void OnBuildStages()
        {
            // 构建GameStages
            this.gsm.gameStages = new Dict<int, GameStage>();

            // 加载资源的GameStage
            GameStage rStageLoadAssets = new GameStage();
            rStageLoadAssets.index = 0;
            rStageLoadAssets.taskList = new List<StageTask>();
            rStageLoadAssets.taskList.Add(new StageTask_LoadAssets(this));
            this.gsm.gameStages.Add(rStageLoadAssets.index, rStageLoadAssets);

            // 初始化游戏数据
            GameStage rStageInitData = new GameStage();
            rStageInitData.index = 1;
            rStageInitData.taskList = new List<StageTask>();
            rStageInitData.taskList.Add(new StageTask_InitData(this));
            this.gsm.gameStages.Add(rStageInitData.index, rStageInitData);
        }

        #endregion

        #region GameMode_CreatePlayer

        public StageConfig StageConfig;

        #endregion
    }
}