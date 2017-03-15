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
        public List<Object>         Objects;

        private HotfixObject        mMBProxyHObj;
        
        void Start()
        {
            mMBProxyHObj = HotfixManager.Instance.App.CreateInstance("KnightHotfixModule.Test.Class1");
            mMBProxyHObj.InvokeInstance("SetObjects", this.Objects);
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
    }
}
