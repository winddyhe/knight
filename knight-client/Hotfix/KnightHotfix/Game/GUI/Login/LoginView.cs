using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Knight.Framework;

namespace Game
{
    public class LoginView : ViewController
    {
        [HotfixBinding("Login")]
        public LoginViewModel   ViewModel;
        [HotfixBinding("Login1")]
        public LoginViewModel   ViewModel1;

        protected override void OnOpening()
        {
        }
        
        protected override void OnUpdate()
        {
        }

        [DataBinding]
        private void OnBtnButton_Clicked(EventArg rEventArg)
        {
            GameLoading.Instance.StartLoading(1.0f, "进入游戏大厅...");
            ViewManager.Instance.CloseView(this.GUID);

            // @TODO: 账户数据通过网络初始化
            Account.Instance.Initialize();

            World.Instance.Initialize();
        }
    }
}
