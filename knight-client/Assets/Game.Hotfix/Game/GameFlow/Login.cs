//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Hotfix.Core;
using Knight.Framework;
using System.Threading.Tasks;
using Knight.Core;

namespace Game
{
    public class Login : THotfixSingleton<Login>
    {
        private Login()
        {
        }

        public async Task Initialize()
        {
            //打开Login界面
            await ViewManager.Instance.OpenAsync("KNLogin", View.State.Fixing);
            //隐藏进度条
            GameLoading.Instance.Hide();
        }
    }
}
