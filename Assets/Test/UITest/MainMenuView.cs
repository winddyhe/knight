//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;

namespace Test
{
    public class MainMenuView : View
    {
        public void OnExitViewButton_Clicked()
        {
            UIManager.Instance.Open("LoginPage", State.dispatch);
            //UIManager.Instance.Pop();
            //UIManager.Instance.CloseView(this.GUID);
        }

        public void OnFirstLevelButton_Clicked()
        {
            Debug.Log("Load first level.");
        }
    }
}


