//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;

namespace Test
{
    public class UITest1 : MonoBehaviour
    {
        void Start()
        {
            UIManager.Instance.Open("LoginPage", View.State.dispatch);
        }
    }
}


