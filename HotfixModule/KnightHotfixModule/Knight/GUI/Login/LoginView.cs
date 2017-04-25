using System.Collections.Generic;
using Framework.WindUI;
using WindHotfix.Core;
using UnityEngine.UI;
using KnightHotfixModule.Knight.GameFlow;
using Framework.Hotfix;
using UnityEngine;
using WindHotfix.GUI;

namespace KnightHotfixModule.Knight.GUI
{
    public class LoginView : THotfixViewController<LoginView>
    {
        private InputField  mAccountInput;
        private InputField  mPasswordInput;

        private string      mGateHost = "";
        private int         mGatePort = 0;
        private int         mServerID = 0;

        public override void OnInitialize()
        {
            mAccountInput  = this.Objects[0].Object as InputField;
            mPasswordInput = this.Objects[1].Object as InputField;
            
            this.mGateHost = (string)this.GetData("GateHost");
            this.mGatePort = (int)this.GetData("GatePort");
            this.mServerID = (int)this.GetData("ServerID");
            
            this.AddEventListener(this.Objects[2].Object, OnButton_Clicked);
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
            Login.Instance.LoginGateServer(this.mGateHost, this.mGatePort, this.mServerID, this.mAccountInput.text, this.mPasswordInput.text);
        }
    }
}
