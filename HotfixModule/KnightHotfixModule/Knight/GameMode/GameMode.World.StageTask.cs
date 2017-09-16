//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework;
using System.Collections;
using UnityEngine;
using WindHotfix.GameStage;
using WindHotfix.GUI;
using WindHotfix.Game;

namespace Game.Knight
{
    public partial class GameMode_World
    {
        /// <summary>
        /// 加载场景资源的StageTask
        /// </summary>
        public class StageTask_LoadAssets : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_LoadAssets(GameMode_World rGameMode)
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
        /// 创建ECS模块
        /// </summary>
        public class StageTask_CreateECS : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_CreateECS(GameMode_World rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "CreateECS";
                return true;
            }

            protected override IEnumerator OnRun_Async()
            {
                // 创建主角
                // 角色的初始位置
                Vector3 rBornPos = new Vector3(this.GameMode.StageConfig.BornPos[0], this.GameMode.StageConfig.BornPos[1], this.GameMode.StageConfig.BornPos[2]);
                this.GameMode.MainPlayer1 = new EntityPlayer();
                yield return this.GameMode.MainPlayer1.Create(Account.Instance.ActiveActor, rBornPos);
                ECSManager.Instance.AddEntity(this.GameMode.MainPlayer1);

                // 创建相机对象
                Camera rMainCamera = GameFlowLevelManager.Instance.GetMainCamera();
                this.GameMode.MainCamera = new EntityCamera();
                this.GameMode.MainCamera.Create(rMainCamera, this.GameMode.StageConfig, this.GameMode.MainPlayer1.CompUnitGo.GameObject);
                ECSManager.Instance.AddEntity(this.GameMode.MainCamera);

                // 创建系统
                ECSManager.Instance
                    .AddSystem(new SystemTransform())
                    .AddSystem(new SystemInputMove())
                    .AddSystem(new SystemMove())
                    .AddSystem(new SystemAnimatorMove())
                    .AddSystem(new SystemAnimatorInput())
                    .AddSystem(new SystemAnimator());

                Debug.Log("GameStage -- create ECS complete.");
            }
        }

        /// <summary>
        /// 初始化数据， 加载界面
        /// </summary>
        public class StageTask_InitData : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_InitData(GameMode_World rGameMode)
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
                // 加载Gamepad界面
                yield return ViewManager.Instance.OpenAsync("KNGamePad", View.State.dispatch);
                GameLoading.Instance.Hide();

                Debug.Log("GameStage -- Init data complete.");
            }
        }
    }
}
