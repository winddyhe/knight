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
        protected string            mHotfixName;
        [HideInInspector][SerializeField]
        protected List<Object>      mObjects;
        [HideInInspector][SerializeField]
        protected List<string>      mTypes;

        private HotfixObject        mMBProxyHObj;

        protected virtual void Awake()
        {
            if (mMBProxyHObj == null)
                mMBProxyHObj = HotfixManager.Instance.App.CreateInstance(this.mHotfixName);

            if (mMBProxyHObj != null)
            {
                mMBProxyHObj.InvokeInstance("SetObjects", this.mObjects);
                mMBProxyHObj.InvokeInstance("Awake");
            }
        }

        protected virtual void Start()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("Start");
        }

        protected virtual void Update()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("Update");
        }

        protected virtual void OnDestroy()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnDestroy");
            mObjects.Clear();
            mMBProxyHObj = null;
        }

        protected virtual void OnEnable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnEnable");
        }

        protected virtual void OnDisable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnDisable");
        }

        public virtual void OnUnityEvent(Object rTarget)
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.InvokeInstance("OnUnityEvent", rTarget);
        }
    }
}
