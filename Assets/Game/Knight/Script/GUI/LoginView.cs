//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using Framework.WindUI;
using UnityEngine.UI;

namespace Game.Knight
{
    public class LoginView : View
    {
        public InputField AccountName;
        public InputField AccountPassword;

        public void OnLoginButton_Clicked()
        {
            if (string.IsNullOrEmpty(this.AccountName.text))
            {
                Toast.Instance.Show("用户名不能为空。");
                return;
            }
            if (string.IsNullOrEmpty(this.AccountPassword.text))
            {
                Toast.Instance.Show("密码不能为空。");
                return;
            }
            Login.Instance.LoginGateServer(this.AccountName.text, this.AccountPassword.text, 1001);
        }
    }
}