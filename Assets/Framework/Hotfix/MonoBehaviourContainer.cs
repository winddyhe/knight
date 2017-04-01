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
        [System.Serializable]
        public class UnityObject
        {
            public string Name;
            public Object Object;
            public string Type;
        }

        [System.Serializable]
        public class BaseDataObject
        {
            public string Name;
            public int    IntObject;
            public long   LongObject;
            public float  FloatObject;
            public double DoubleObject;
            public string StringObject;
            public string Type;
        }

        public enum BaseDataType
        {
            Int,
            Long,
            Float,
            Double,
            String,
        }
        
        [HideInInspector][SerializeField]
        protected string                        mHotfixName;
        [HideInInspector][SerializeField]
        protected List<UnityObject>             mObjects;
        //[HideInInspector][SerializeField]
        //protected List<string>                mTypes;
        [HideInInspector][SerializeField]
        protected List<BaseDataObject>          mBaseDatas;
        //[HideInInspector][SerializeField]
        //protected List<string>                mBaseTypes;

        private HotfixObject                    mMBProxyHObj;

        protected virtual void Awake()
        {
            if (mMBProxyHObj == null)
                mMBProxyHObj = HotfixManager.Instance.App.CreateInstance(this.mHotfixName);

            if (mMBProxyHObj != null)
            {
                mMBProxyHObj.InvokeInstance("SetObjects", this.mObjects, this.mBaseDatas);
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
