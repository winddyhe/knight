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
    public class Class2 : THotfixMB<Class2>
    {
        public override void Start()
        {
            HotfixEventManager.Instance.Binding(this.Objects[0].Object, UnityEngine.EventSystems.EventTriggerType.PointerClick, OnButton_Clicked);
            HotfixEventManager.Instance.Binding(this.Objects[1].Object, UnityEngine.EventSystems.EventTriggerType.PointerClick, OnButton1_Clicked);
        }

        public override void OnDestroy()
        {
            HotfixEventManager.Instance.UnBinding(this.Objects[0].Object, UnityEngine.EventSystems.EventTriggerType.PointerClick, OnButton_Clicked);
            HotfixEventManager.Instance.UnBinding(this.Objects[1].Object, UnityEngine.EventSystems.EventTriggerType.PointerClick, OnButton1_Clicked);
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
