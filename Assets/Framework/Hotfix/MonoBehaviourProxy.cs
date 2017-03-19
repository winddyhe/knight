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
    [System.Serializable]
    public class MonoBehaviourProxy
    {
        public List<Object> Objects;

        public virtual void SetObjects(List<Object> rObjs)
        {
            this.Objects = rObjs;
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
