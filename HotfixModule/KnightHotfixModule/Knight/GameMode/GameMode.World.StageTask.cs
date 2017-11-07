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
using System.Threading.Tasks;

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

            protected override async Task OnRun_Async()
            {
                // 加载场景
                var rSceneRequest = await SceneAssetLoader.Instance.Load_Async(
                    this.GameMode.StageConfig.SceneABPath,
                    this.GameMode.StageConfig.ScenePath,
                    UnityEngine.SceneManagement.LoadSceneMode.Additive);

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

            protected override async Task OnRun_Async()
            {
                // 创建主角
                // 角色的初始位置
                Vector3 rBornPos = new Vector3(this.GameMode.StageConfig.BornPos[0], this.GameMode.StageConfig.BornPos[1], this.GameMode.StageConfig.BornPos[2]);
                this.GameMode.MainPlayer = new EntityPlayer();
                await this.GameMode.MainPlayer.Create(Account.Instance.ActiveActor, rBornPos);
                ECSManager.Instance.AddEntity(this.GameMode.MainPlayer);

                // 创建相机对象
                Camera rMainCamera = GameFlowLevelManager.Instance.GetMainCamera();
                this.GameMode.MainCamera = new EntityCamera();
                this.GameMode.MainCamera.Create(rMainCamera, this.GameMode.StageConfig, this.GameMode.MainPlayer.CompUnitGo.GameObject);
                ECSManager.Instance.AddEntity(this.GameMode.MainCamera);

                // 创建系统
                ECSManager.Instance
                    .AddSystem(new SystemTransform())
                    .AddSystem(new SystemInputMove())
                    .AddSystem(new SystemMove())
                    .AddSystem(new SystemAnimatorMove())
                    .AddSystem(new SystemAnimatorInput())
                    .AddSystem(new SystemAnimator())
                    .AddSystem(new SystemCamera());

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

            protected override async Task OnRun_Async()
            {
                // 加载Gamepad界面
                await ViewManager.Instance.Open("KNGamePad", View.State.dispatch);
                GameLoading.Instance.Hide();
                
                // 初始化ECS模块
                ECSManager.Instance.Initialize();

                Debug.Log("GameStage -- Init data complete.");
            }
        }
    }
}
