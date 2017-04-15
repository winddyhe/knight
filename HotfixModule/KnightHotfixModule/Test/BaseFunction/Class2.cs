//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using WindHotfix.Core;
using UnityEngine;

namespace WindHotfix.Test
{
    public class Class2 : THotfixMonoBehaviour<Class2>
    {
        public override void Start()
        {
            this.AddEventListener(this.Objects[0].Object, OnButton_Clicked);
            this.AddEventListener(this.Objects[1].Object, OnButton1_Clicked);

            Debug.LogError("1111");
            string rName = (string)this.GetData("Name");
            Debug.LogError(rName);
        }

        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError(rObj.name + " Clicked...");
        }

        private void OnButton1_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError(rObj.name + " Clicked...");
        }
    }
}
