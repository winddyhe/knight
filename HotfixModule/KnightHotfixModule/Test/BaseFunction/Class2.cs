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
    public class Class2 : MonoBehaviourProxy
    {
        private HotfixEventHandler mEventHandler;
        
        public override void SetObjects(List<UnityEngine.Object> rObjs)
        {
            base.SetObjects(rObjs);
        }

        public override void Start()
        {
            mEventHandler = new HotfixEventHandler(this.Objects);
            mEventHandler.AddEventListener(this.Objects[0], OnButton_Clicked);
            mEventHandler.AddEventListener(this.Objects[1], OnButton1_Clicked);
        }

        public override void OnDestroy()
        {
            if (mEventHandler != null)
                mEventHandler.RemoveAll();
            mEventHandler = null;
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mEventHandler == null) return;
            mEventHandler.Handle(rTarget);
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
