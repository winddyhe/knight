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
using WindHotfix.Core;

namespace Game.Knight
{
    public class LoginView : TViewController<LoginView>
    {
        [HotfixBinding("AccountInput")]
        public InputField       AccountInput;
        [HotfixBinding("PasswordInput")]
        public InputField       PasswordInput;
        
        private int             mServerID       = 1001;
        //private string        mGateHost       = "127.0.0.1";
        //private int           mGatePort       = 3010;

        [HotfixBindingEvent("LoginBtn", EventTriggerType.PointerClick)]
        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            if (string.IsNullOrEmpty(this.AccountInput.text))
            {
                Toast.Instance.Show("用户名不能为空。");
                return;
            }
            if (string.IsNullOrEmpty(this.PasswordInput.text))
            {
                Toast.Instance.Show("密码不能为空。");
                return;
            }
            //Login.Instance.LoginGateServer(this.mGateHost, this.mGatePort, this.mServerID, this.mAccountInput.text, this.mPasswordInput.text);

            // 创建角色账户，并进入创建角色界面
            Account.Instance.Create(10001, "红烧肉", mServerID);
            CoroutineManager.Instance.Start(Login.Instance.JumpToCreatePlayer(new List<ActorNet>()));
        }
    }
}
