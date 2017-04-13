//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Hotfix
{
    public class HotfixMB
    {
        public HotfixObject HotfixObj;
        public string       ParentType = "WindHotfix.Core.THotfixMonoBehaviour`1";

        public HotfixMB(string rHotfixName)
        {
            this.HotfixObj = HotfixApp.Instance.Instantiate(rHotfixName);
            this.ParentType = string.Format("WindHotfix.Core.THotfixMonoBehaviour`1<{0}>", rHotfixName);
        }

        public void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Initialize", rObjs, rBaseDatas);
        }

        public void Awake()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("Awake");
        }

        public void Start()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("Start");
        }

        public void Update()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("Update");
        }

        public void OnDestroy()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Destroy");
        }

        public void OnEnable()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnEnable");
        }

        public void OnDisable()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnDisable");
        }

        public void OnUnityEvent(Object rTarget)
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "OnUnityEvent", rTarget);
        }
    }
}
