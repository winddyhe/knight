//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;

namespace Game.Knight
{
    public class World : MonoBehaviour
    {
        private static World __instance;
        public  static World Instance { get { return __instance; } }

        private GameMode_World mGameMode;

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }

        void Start()
        {
            mGameMode = new GameMode_World();
            mGameMode.StageConfig = GameConfig.Instance.GetStageConfig(10002);
            if (mGameMode.StageConfig == null)
            {
                Debug.LogError("Not found stage 10002 .");
                return;
            }

            GameMode.GetCurrentMode = (() =>
            {
                return mGameMode;
            });

            // 开始游戏
            GameStageManager.Instance.InitGame();
            GameStageManager.Instance.StageIntialize();
            GameStageManager.Instance.StageRunning();
        }
    }
}

