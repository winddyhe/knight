//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WindHotfix.Core
{
    public class THotfixMB<T> where T : class
    {
        public GameObject           GameObject;
        public List<UnityObject>    Objects;
        
        public void Awake_Proxy(GameObject rGo, List<UnityObject> rObjs)
        {
            this.GameObject = rGo;
            this.Objects    = rObjs;

            this.Awake();
        }

        public void Start_Proxy()
        {
            this.Start();
        }

        public void Update_Proxy()
        {
            this.Update();
        }

        public void OnDestroy_Proxy()
        {
            this.OnDestroy();

            this.GameObject = null;
            if (this.Objects != null)
            {
                for (int i = 0; i < this.Objects.Count; i++)
                {
                    this.Objects[i] = null;
                }
                this.Objects.Clear();
            }
        }

        public void OnEnable_Proxy()
        {
            this.OnEnable();
        }

        public void OnDisable_Proxy()
        {
            this.OnDisable();
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

        private void OnDisable()
        {
        }
    }
}
