//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Framework;
using System.Collections;
using UnityEngine;
using Core;
using Framework.WindUI;
using WindHotfix.GameStage;
using WindHotfix.Core;
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
        /// 加载角色的StageTask
        /// </summary>
        public class StageTask_CreatePlayer : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_CreatePlayer(GameMode_World rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "CreatePlayer";
                return true;
            }

            protected override IEnumerator OnRun_Async()
            {
                // 加载场景
                var rActorRequest = Actor.CreateActor(Account.Instance.ActiveActor);
                yield return rActorRequest;

                Actor rMainPlayer = rActorRequest.Actor;
                if (rMainPlayer != null)
                {
                    GameObject.DontDestroyOnLoad(rMainPlayer.ActorGo);
                    this.GameMode.MainPlayer = rMainPlayer;
                    this.GameMode.PlayerList.Add(rMainPlayer);
                }

                Debug.Log("GameStage -- Create player complete.");
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
                Camera rMainCamera = GameFlowLevelManager.Instance.GetMainCamera();
                CameraSetting rCameraSetting = new CameraSetting();
                rCameraSetting.AngleX   = this.GameMode.StageConfig.CameraSettings[0];
                rCameraSetting.AngleY   = this.GameMode.StageConfig.CameraSettings[1];
                rCameraSetting.Distance = this.GameMode.StageConfig.CameraSettings[2];
                rCameraSetting.OffsetY  = this.GameMode.StageConfig.CameraSettings[3];
                rCameraSetting.Target   = this.GameMode.MainPlayer.ActorGo;

                var rCameraController   = rMainCamera.gameObject.ReceiveComponent<CameraController>();
                rCameraController.CameraSettings = rCameraSetting;
                
                // 设置角色的初始位置
                Vector3 rBornPos = new Vector3(
                    this.GameMode.StageConfig.BornPos[0],
                    this.GameMode.StageConfig.BornPos[1],
                    this.GameMode.StageConfig.BornPos[2]);
                this.GameMode.MainPlayer.ActorGo.transform.position = rBornPos;

                // 添加角色控制器
                //this.GameMode.MainPlayer.ActorGo.ReceiveHotfixComponent<ActorUserController>();
                //this.GameMode.MainPlayer.ActorGo.ReceiveHotfixComponent<ActorController>();

                // 加载Gamepad界面
                yield return ViewManager.Instance.OpenAsync("KNGamePad", View.State.dispatch);

                GameLoading.Instance.Hide();
                Debug.Log("GameStage -- Init data complete.");
                yield break;
            }
        }

        /// <summary>
        /// 初始化数据， 加载界面
        /// </summary>
        public class StageTask_InitMainPlayer : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_InitMainPlayer(GameMode_World rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "InitMainPlayer";
                return true;
            }

            protected override IEnumerator OnRun_Async()
            {
                var rCompAvatar = this.GameMode.MainPlayer1.AddComponent<ComponentAvatar>();
                yield break;
            }
        }
    }
}
