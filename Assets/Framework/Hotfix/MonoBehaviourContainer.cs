//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Game.Knight;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Hotfix
{
    public class MonoBehaviourContainer : MonoBehaviour
    {
        [HideInInspector][SerializeField]
        private string              mHotfixName;
        [HideInInspector][SerializeField]
        private List<Object>        mObjects;
        [HideInInspector][SerializeField]
        private List<string>        mTypes;

        private HotfixObject        mMBProxyHObj;
        
        void Start()
        {
            mMBProxyHObj = HotfixManager.Instance.App.CreateInstance(this.mHotfixName);

            mMBProxyHObj.InvokeInstance("SetObjects", this.mObjects);
            mMBProxyHObj.InvokeInstance("Start");
        }
        
        void Update()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("Update");
        }

        void OnDestroy()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnDestroy");
        }

        void OnEnable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnEnable");
        }

        void OnDisable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnDisable");
        }

        public void OnUnityEvent(Object rTarget)
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnUnityEvent", rTarget);
        }
    }
}
