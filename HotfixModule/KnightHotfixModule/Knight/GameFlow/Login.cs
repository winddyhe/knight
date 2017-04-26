//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections;
using Framework.WindUI;
using Game.Knight;
using Core;
using Framework;
using UnityEngine;
using WindHotfix.Core;

namespace KnightHotfixModule.Knight
{
    public class Login : THotfixMB<Login>
    {
        private static  Login       __instance;
        public  static  Login       Instance { get { return __instance; } }

        private string  mGateHost    = "";
        private int     mGatePort    = 0;
        private int     mServerID    = 0;

        private string  mAccountName = "";
        private string  mPassword    = "";
        
        public override void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public override void OnDestroy()
        {
            __instance = null;
        }

        public override void Start()
        {
            if (__instance == null)
                __instance = this;

            CoroutineManager.Instance.Start(this.Start_Async());
        }

        private IEnumerator Start_Async()
        {
            //打开Login界面
            yield return UIManager.Instance.OpenAsync("KNLogin", View.State.dispatch);
            //隐藏进度条
            GameLoading.Instance.Hide();
        }

        public void OnClientLoginResponse(NetworkMsgCode rMsgCode, long rAccountID, List<NetActor> rNetActors)
        {
            if (rMsgCode == NetworkMsgCode.Success)
            {
                // 创建角色账户，并进入创建角色界面
                Account.Instance.Create(rAccountID, mAccountName, mServerID);
                CoroutineManager.Instance.Start(JumpToCreatePlayer(rNetActors));
            }
            else if (rMsgCode == NetworkMsgCode.FA_USER_NOT_EXIST)
            {
                Toast.Instance.Show("用户不存在！");
            }
            else if (rMsgCode == NetworkMsgCode.FA_USER_PASS_ERROR)
            {
                Toast.Instance.Show("登陆密码错误！");
            }
            else if (rMsgCode == NetworkMsgCode.ServerError)
            {
                Toast.Instance.Show("服务器错误！");
            }
        }

        /// <summary>
        /// 请求连接服务器
        /// </summary>
        public void LoginGateServer(string rGateHost, int nGatePort, int nServerID, string rAccountName, string rAccountPassword)
        {
            mAccountName = rAccountName;
            mPassword = rAccountPassword;

            mGateHost = rGateHost;
            mGatePort = nGatePort;
            mServerID = nServerID;

            Debug.Log("LoginGateServer");
            NetworkClient.Instance.Connect(this.mGateHost, this.mGatePort, () =>
            {
                GamePlayProtocol.DoClientQueryGateEntryRequest(mAccountName);
            });
        }
        
        /// <summary>
        /// 请求连接Connector服务器
        /// </summary>
        public void LoginConnectorRequest(NetworkMsgCode rMsgCode, string rConnectorURL, int rConnectorPort)
        {
            // 断开连接
            NetworkClient.Instance.Disconnect();
            if (rMsgCode == NetworkMsgCode.Success)
            {
                ClientLoginRequest(rConnectorURL, rConnectorPort);
            }
            else if (rMsgCode == NetworkMsgCode.FA_NO_SERVER_AVAILABLE)
            {
                Toast.Instance.Show("无可用服务器!");
            }
            else
            {
                Toast.Instance.Show("未知的系统错误!");
            }
        }

        /// <summary>
        /// 客户端请求用户登陆
        /// </summary>
        private void ClientLoginRequest(string rConnectorURL, int rConnectorPort)
        {
            NetworkClient.Instance.Connect(rConnectorURL, rConnectorPort, () =>
            {
                string rMsg = mAccountName + "|" + mPassword + "|" + DateTime.Now.Ticks;
                string rToken = CryptoUtility.Encrypt(rMsg, UtilTool.SessionSecrect);
                GamePlayProtocol.DoClientLoginRequest(rToken);
            });
        }

        private IEnumerator JumpToCreatePlayer(List<NetActor> rNetActors)
        {
            UIManager.Instance.Pop();
            Account.Instance.NetActors = rNetActors;
            GameLoading.Instance.StartLoading(1.0f, "开始创建角色！");       // 开始创建新角色
            var rLevelRequest = Globals.Instance.LoadLevel("CreatePlayer"); // 切换到Login场景
            yield return rLevelRequest;
        }
    }
}
