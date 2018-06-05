//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine.UI;
using Knight.Hotfix.Core;
using Knight.Framework;
using UnityEngine;

namespace Game
{
    public class LoginView : TViewController<LoginView>
    {
        [HotfixBinding("AccountInput")]
        public InputField       AccountInput;
        [HotfixBinding("PasswordInput")]
        public InputField       PasswordInput;
        
        [HotfixBindingEvent("LoginBtn", HEventTriggerType.PointerClick)]
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

            // 创建角色账户，并进入创建角色界面
            Debug.Log("进入游戏...");
        }
    }
}
