//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using WindHotfix.Core;
using WindHotfix.GameStage;

namespace Game.Knight
{
    public class World : THotfixMB<World>
    {
        private static World    __instance;
        public  static World    Instance { get { return __instance; } }

        private GameMode_World  mGameMode;

        public override void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }

        public override void Start()
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

