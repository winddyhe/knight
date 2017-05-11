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
        public string       HotfixName;
        public GameObject   GameObject;
        public string       ParentType = "WindHotfix.Core.THotfixMB`1";

        public HotfixMB()
        {
        }

        public void SetHotfix(string rHotfixName, GameObject rGameObject)
        {
            this.HotfixName = rHotfixName;
            this.GameObject = rGameObject;
            this.ParentType = string.Format("WindHotfix.Core.THotfixMB`1<{0}>", rHotfixName);
        }

        public virtual void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnUnityEvent(Object rTarget)
        {
        }
    }
}
