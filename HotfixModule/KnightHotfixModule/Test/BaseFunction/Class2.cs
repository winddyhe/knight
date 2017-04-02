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
        
        public override void SetObjects(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            base.SetObjects(rObjs, rBaseDatas);
        }

        public override void Start()
        {
            mEventHandler = new HotfixEventHandler();
            mEventHandler.AddEventListener(this.Objects[0].Object, OnButton_Clicked);
            mEventHandler.AddEventListener(this.Objects[1].Object, OnButton1_Clicked);

            string rName = (string)this.GetData("Name");
            Debug.LogError(rName);
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
