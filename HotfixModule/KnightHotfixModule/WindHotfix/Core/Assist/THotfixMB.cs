//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WindHotfix.Core
{
    public class THotfixMB<T> where T : class
    {
        public GameObject                   GameObject;
        public List<UnityObject>            Objects;

        protected List<HotfixEventObject>   mEventObjs;

        public void Awake_Proxy(GameObject rGo, List<UnityObject> rObjs)
        {
            this.GameObject = rGo;
            this.Objects    = rObjs;

            // Data binding 
            this.BindHotfixMB();
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

            for (int i = 0; i < this.mEventObjs.Count; i++)
            {
                if (!this.mEventObjs[i].NeedUnbind) continue;
                HotfixEventManager.Instance.UnBinding(this.mEventObjs[i].TargetObject, this.mEventObjs[i].EventType, this.mEventObjs[i].EventHandler);
            }
            this.mEventObjs.Clear();

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

        public UnityObject Get(string rObjName)
        {
            if (this.Objects == null) return null;
            for (int i = 0; i < this.Objects.Count; i++)
            {
                if (this.Objects[i].Name.Equals(rObjName))
                {
                    return this.Objects[i];
                }
            }
            return null;
        }

        public UnityObject Get(int nIndex)
        {
            if (this.Objects == null) return null;
            if (nIndex < 0 || nIndex >= this.Objects.Count) return null;
            return this.Objects[nIndex];
        }
        
        public void BindHotfixMB()
        {
            this.mEventObjs = new List<HotfixEventObject>();

            Type rType = this.GetType();
            if (rType == null) return;

            var rBindingFlags = BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rFiledInfos = rType.GetFields(rBindingFlags);
            for (int i = 0; i < rFiledInfos.Length; i++)
            {
                var rAttrObjs = rFiledInfos[i].GetCustomAttributes(typeof(HotfixBindingAttribute), false);
                if (rAttrObjs == null || rAttrObjs.Length == 0) continue;

                var rBindingAttr = rAttrObjs[0] as HotfixBindingAttribute;
                if (rBindingAttr != null)
                {
                    UnityObject rUnityObject = null;
                    if (!string.IsNullOrEmpty(rBindingAttr.Name))
                        rUnityObject = this.Get(rBindingAttr.Name);
                    else
                        rUnityObject = this.Get(rBindingAttr.Index);

                    // 如果属性没有，直接报错
                    if (rUnityObject == null)
                        UnityEngine.Debug.LogErrorFormat("Not find binding data, please check prefab and hofix script. {0}", rFiledInfos[i].Name);
                    else
                    {
                        if (!rUnityObject.Type.Equals(rFiledInfos[i].FieldType.ToString()))
                            UnityEngine.Debug.LogErrorFormat("Binding data type is not match. {0}", rFiledInfos[i].Name);
                        else
                            rFiledInfos[i].SetValue(this, rUnityObject.Object);
                    }
                }
            }

            rBindingFlags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rMethodInfos = rType.GetMethods(rBindingFlags);
            for (int i = 0; i < rMethodInfos.Length; i++)
            {
                var rAttrObjs = rMethodInfos[i].GetCustomAttributes(typeof(HotfixBindingEventAttribute), false);
                if (rAttrObjs == null || rAttrObjs.Length == 0) continue;

                var rBindingEventAttr = rAttrObjs[0] as HotfixBindingEventAttribute;
                if (rBindingEventAttr != null)
                {
                    UnityObject rUnityObject = null;
                    if (!string.IsNullOrEmpty(rBindingEventAttr.Name))
                        rUnityObject = this.Get(rBindingEventAttr.Name);

                    if (rUnityObject != null)
                    {
                        // 委托所在的对象，如果不是当前对象，要改动 
                        MethodInfo rMethodInfo = rMethodInfos[i];
                        Action<Object> rActionDelegate = (rObj) => { rMethodInfo.Invoke(this, new object[] { rUnityObject.Object }); };
                        HotfixEventManager.Instance.Binding(rUnityObject.Object, rBindingEventAttr.EventType, rActionDelegate);
                        this.mEventObjs.Add(new HotfixEventObject()
                        {
                            TargetObject = rUnityObject.Object,
                            EventHandler = rActionDelegate,
                            EventType = rBindingEventAttr.EventType,
                            NeedUnbind = rBindingEventAttr.NeedUnbind
                        });
                    }
                }
            }
        }
    }
}
