//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
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
        protected string                    mHotfixName;
        [HideInInspector][SerializeField]
        protected List<UnityObject>         mObjects;
        [HideInInspector][SerializeField]
        protected bool                      mNeedUpdate;

        private HotfixObject                mMBHotfixObj;
        public  HotfixObject                MBHotfixObject  { get { return this.mMBHotfixObj;   } }

        public string                       HotfixName      { get { return mHotfixName;         } set { mHotfixName = value; } }
        public bool                         NeedUpdate      { get { return mNeedUpdate;         } set { mNeedUpdate = value; } }
        public List<UnityObject>            Objects         { get { return mObjects;            } }

        private string mParentType = "WindHotfix.Core.THotfixMB`1<{0}>";

        protected virtual void Awake()
        {
            this.InitHotfixMB();
        }

        protected virtual void Start()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.InvokeParent(this.mParentType, "Start_Proxy");
        }

        protected virtual void Update()
        {
            if (!mNeedUpdate) return;

            if (mMBHotfixObj == null) return;
            mMBHotfixObj.InvokeParent(this.mParentType, "Update_Proxy");
        }

        protected virtual void OnDestroy()
        {
            if (mMBHotfixObj != null)
                mMBHotfixObj.InvokeParent(this.mParentType, "OnDestroy_Proxy");

            if (mObjects != null)
                mObjects.Clear();

            mMBHotfixObj = null;
            mObjects = null;
        }

        protected virtual void OnEnable()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.InvokeParent(this.mParentType, "OnEnable_Proxy");
        }

        protected virtual void OnDisable()
        {
            if (mMBHotfixObj == null) return;
            mMBHotfixObj.InvokeParent(this.mParentType, "OnDisable_Proxy");
        }

        protected void InitHotfixMB()
        {
            if (mMBHotfixObj == null && !string.IsNullOrEmpty(mHotfixName))
            {
                this.mParentType = string.Format(this.mParentType, this.mHotfixName);
                this.mMBHotfixObj = HotfixManager.Instance.Instantiate(this.mHotfixName);
                this.mMBHotfixObj.InvokeParent(this.mParentType, "Awake_Proxy", this.gameObject, this.mObjects);
            }
        }

        public void Initialize(string rHotfixName, bool bNeedUpdate = false)
        {
            this.mHotfixName = rHotfixName;
            this.mNeedUpdate = bNeedUpdate;
            this.Awake();
        }
    }
}
