//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Framework.WindUI;
using UnityEngine.UI;
using WindHotfix.GUI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Game.Knight
{
    public class LoginView : TViewController<LoginView>
    {
        private InputField  mAccountInput;
        private InputField  mPasswordInput;

        private string      mGateHost = "127.0.0.1";
        private int         mGatePort = 3010;
        private int         mServerID = 1001;

        public override void OnInitialize()
        {
            mAccountInput  = this.Objects[0].Object as InputField;
            mPasswordInput = this.Objects[1].Object as InputField;
            
            this.EventBinding(this.Objects[2].Object, EventTriggerType.PointerClick, OnButton_Clicked);
        }

        public override void OnClosed()
        {
            this.EventUnBinding(this.Objects[2].Object, EventTriggerType.PointerClick, OnButton_Clicked);
        }

        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            if (string.IsNullOrEmpty(this.mAccountInput.text))
            {
                Toast.Instance.Show("用户名不能为空。");
                return;
            }
            if (string.IsNullOrEmpty(this.mPasswordInput.text))
            {
                Toast.Instance.Show("密码不能为空。");
                return;
            }
            //Login.Instance.LoginGateServer(this.mGateHost, this.mGatePort, this.mServerID, this.mAccountInput.text, this.mPasswordInput.text);

            // 创建角色账户，并进入创建角色界面
            Account.Instance.Create(10001, "红烧肉", mServerID);
            CoroutineManager.Instance.Start(Login.Instance.JumpToCreatePlayer(new List<NetActor>()));
        }
    }
}
