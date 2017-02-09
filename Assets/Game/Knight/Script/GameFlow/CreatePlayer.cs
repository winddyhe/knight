//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Framework;
using Framework.WindUI;
using Core;

namespace Game.Knight
{
    public class CreatePlayer : MonoBehaviour
    {
        private static CreatePlayer     __instance;
        public  static CreatePlayer     Instance { get { return __instance; } }

        private GameMode_CreatePlayer   mGameMode;

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }

        void Start()
        {
            // 设置当前的游戏模式
            mGameMode = new GameMode_CreatePlayer();
            mGameMode.StageConfig = GameConfig.Instance.GetStageConfig(10001);
            if (mGameMode.StageConfig == null)
            {
                Debug.LogError("Not found stage 10001 .");
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

        /// <summary>
        /// 开始创建角色
        /// </summary>
        public void Create(string rActorName, int rProfessionalID)
        {
            GamePlayProtocol.Instance.DoClientCreatePlayerRequest(
                Account.Instance.AccountID, rActorName, rProfessionalID, Account.Instance.ServerID);
        }

        /// <summary>
        /// 创建角色的响应
        /// </summary>
        public void OnPlayerCreateResponse(NetworkMsgCode rMsgCode, string rActorName, int rProfessionalID, long rActorID)
        {
            if (rMsgCode == NetworkMsgCode.Success)
            {
                Account.Instance.CreateActor(rActorName, rProfessionalID, rActorID);
                UIManager.Instance.Open("KNPlayerList", View.State.dispatch);
            }
            else if (rMsgCode == NetworkMsgCode.FA_ACTOR_IS_EXSIST)
            {
                Toast.Instance.Show("角色名已经存在！");
            }
        }
    }
}
