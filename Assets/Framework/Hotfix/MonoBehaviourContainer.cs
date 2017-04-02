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
        protected string                        mHotfixName;
        [HideInInspector][SerializeField]
        protected List<UnityObject>             mObjects;
        [HideInInspector][SerializeField]
        protected List<BaseDataDisplayObject>   mBaseDatas;
        
        private MonoBehaviourProxy              mMBProxyHObj;
        public  MonoBehaviourProxy              ProxyHotfixObject { get { return this.mMBProxyHObj; } }

        protected virtual void Awake()
        {
            if (mMBProxyHObj == null)
                mMBProxyHObj = HotfixManager.Instance.App.CreateInstance<MonoBehaviourProxy>(this.mHotfixName);

            if (mMBProxyHObj != null)
            {
                mMBProxyHObj.SetObjects(this.mObjects, this.ToBaseDataObjects(mBaseDatas));
                mMBProxyHObj.Awake();
            }
        }

        protected virtual void Start()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.Start();
        }

        protected virtual void Update()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.Update();
        }

        protected virtual void OnDestroy()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.OnDestroy();
            mObjects.Clear();
            mMBProxyHObj = null;
        }

        protected virtual void OnEnable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.OnDisable();
        }

        public virtual void OnUnityEvent(Object rTarget)
        {
            if (mMBProxyHObj == null) return;
            mMBProxyHObj.OnUnityEvent(rTarget);
        }

        protected List<BaseDataObject> ToBaseDataObjects(List<BaseDataDisplayObject> rBaseDatas)
        {
            List<BaseDataObject> rBaseDataObjects = new List<BaseDataObject>();
            for (int i = 0; i < rBaseDatas.Count; i++)
            {
                BaseDataObject rObj = new BaseDataObject();
                rObj.Type = rBaseDatas[i].Type;
                rObj.Name = rBaseDatas[i].Name;
                if (rBaseDatas[i].Type == "Int")
                    rObj.Object = rBaseDatas[i].IntObject;
                else if (rBaseDatas[i].Type == "Long")
                    rObj.Object = rBaseDatas[i].LongObject;
                else if (rBaseDatas[i].Type == "Float")
                    rObj.Object = rBaseDatas[i].FloatObject;
                else if (rBaseDatas[i].Type == "Double")
                    rObj.Object = rBaseDatas[i].DoubleObject;
                else if (rBaseDatas[i].Type == "String")
                    rObj.Object = rBaseDatas[i].StringObject;
                rBaseDataObjects.Add(rObj);
            }
            return rBaseDataObjects;
        }
    }
}
