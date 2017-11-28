//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using WindHotfix.Core;
using UnityEngine;
using UnityEngine.UI;

namespace WindHotfix.Test
{
    public class Class2 : THotfixMB<Class2>
    {
        [HotfixBinding("HotfixTest1")]
        public Button   BtnTest1;

        [HotfixBinding(1)]
        public Button   BtnTest2;

        public override void Start()
        {
        }

        public override void OnDestroy()
        {
        }

        [HotfixBindingEvent("HotfixTest1", UnityEngine.EventSystems.EventTriggerType.PointerClick)]
        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError(rObj.name + " Clicked...");
        }

        [HotfixBindingEvent("HotfixTest1 (1)", UnityEngine.EventSystems.EventTriggerType.PointerClick)]
        private void OnButton1_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError(rObj.name + " Clicked...");
            for (int i = 0; i < 1000; i++)
            {
                v += Vector2.one;
                dot += Vector2.Dot(v, Vector2.zero);
            }
        }

        Vector2 v = Vector2.zero;
        int x = 1;
        float dot = 0;

        public override void Update()
        {
            v += Vector2.one;
            dot += Vector2.Dot(v, Vector2.zero);
            //for (int i = 0; i < 1000; i++)
            //{
            //    v += Vector2.one;
            //    dot += Vector2.Dot(v, Vector2.zero);
            //}
        }
    }
}
