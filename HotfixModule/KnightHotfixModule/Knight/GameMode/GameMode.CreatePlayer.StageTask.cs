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
using WindHotfix.GameStage;
using WindHotfix.GUI;

namespace Game.Knight
{
    public partial class GameMode_CreatePlayer
    {
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

                yield return rSceneRequest;
                
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
                yield return ViewManager.Instance.OpenAsync(rViewName, View.State.dispatch);

                GameLoading.Instance.Hide();

                Debug.Log("GameStage -- Init data complete.");
                yield break;
            }
        }
    }
}