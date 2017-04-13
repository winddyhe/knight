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
    public class HotfixMBContainer : MonoBehaviour
    {
        [HideInInspector][SerializeField]
        protected string                        mHotfixName;
        [HideInInspector][SerializeField]
        protected List<UnityObject>             mObjects;
        [HideInInspector][SerializeField]
        protected List<BaseDataDisplayObject>   mBaseDatas;
        [HideInInspector][SerializeField]
        protected bool                          mIsInheritance;

        private HotfixMB                        mMBHotfixObj;
        public  HotfixMB                        MBHotfixObject { get { return this.mMBHotfixObj; } }

        public  HotfixMBInherit                 mMBHotfixInheritObj;
        public  HotfixMBInherit                 InheritObject  { get { return this.mMBHotfixInheritObj; } }

        protected virtual void Awake()
        {
            if (this.mIsInheritance)
                this.InitHotfixMBInherit();
            else
                this.InitHotfixMB();
        }

        protected virtual void Start()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.Start();
        }

        protected virtual void Update()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.Update();
        }

        protected virtual void OnDestroy()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.OnDestroy();
            mObjects.Clear();
            mBaseDatas.Clear();
            mMBHotfixObj = null;
        }

        protected virtual void OnEnable()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.OnEnable();
        }

        protected virtual void OnDisable()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.OnDisable();
        }

        public virtual void OnUnityEvent(Object rTarget)
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.OnUnityEvent(rTarget);
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


        private void InitHotfixMBInherit()
        {
            if (mMBHotfixInheritObj == null)
                mMBHotfixInheritObj = new HotfixMBInherit(mHotfixName);

            if (mMBHotfixInheritObj != null)
                mMBHotfixInheritObj.Initialize(this.mObjects, this.ToBaseDataObjects(mBaseDatas));
        }

        private void InitHotfixMB()
        {
            if (mMBHotfixObj == null)
                mMBHotfixObj = new HotfixMB(mHotfixName);

            if (mMBHotfixObj != null)
            {
                mMBHotfixObj.Initialize(this.mObjects, this.ToBaseDataObjects(mBaseDatas));
                mMBHotfixObj.Awake();
            }
        }
    }
}
