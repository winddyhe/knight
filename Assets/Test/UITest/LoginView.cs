//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Framework.WindUI;

namespace Test
{
    /// <summary>
    /// 登陆的View
    /// </summary>
    class LoginView : View
    {
        public void OnNewGameButton_Clicked()
        {
            UIManager.Instance.Open("NewGamePage", State.dispatch);
        }

        public void OnExitGameButton_Clicked()
        {
            UtilTool.ExitApplication();
        }
    }
}
