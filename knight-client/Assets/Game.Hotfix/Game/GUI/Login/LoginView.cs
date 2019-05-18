using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Knight.Core;

namespace Game
{
    public class LoginView : ViewController
    {
        protected override async Task OnInitialize()
        {
            await base.OnInitialize();
            Account.Instance.PlayerName = "Winddy";
        }

        [DataBinding]
        private void OnBtnButton_Clicked(EventArg rEventArg)
        {
            GameLoading.Instance.StartLoading(1.0f, "进入游戏大厅...");
            ViewManager.Instance.CloseView(this.GUID);

            // @TODO: 账户数据通过网络初始化
            Account.Instance.PlayerName = "Winddy";
            Account.Instance.CoinCount = 33344444;

            World.Instance.Initialize().WarpErrors();
        }
    }
}
