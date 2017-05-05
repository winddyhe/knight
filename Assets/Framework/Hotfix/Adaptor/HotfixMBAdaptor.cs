//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using ILRuntime.CLR.Method;
using UnityEngine;

namespace Framework.Hotfix
{
    public class HotfixMBAdaptor : CrossBindingAdaptor
    {
        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override Type BaseCLRType
        {
            get { return typeof(HotfixMB); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : HotfixMB, CrossBindingAdaptorType
        {
            private ILTypeInstance  __instance;
            private AppDomain       mAppdomain;
            
            private bool            mIsInitializeInvoking   = false;

            private IMethod         mAwakeMethod;
            private bool            mAwakeGot               = false;
            private bool            mIsAwakeInvoking        = false;

            private IMethod         mStartMethod;
            private bool            mStartGot               = false;
            private bool            mIsStartInvoking        = false;
            
            private IMethod         mUpdateMethod;
            private bool            mUpdateGot              = false;
            private bool            mIsUpdateInvoking       = false;
            
            private bool            mIsOnDestroyInvoking    = false;
                        
            private IMethod         mOnEnableMethod;
            private bool            mOnEnableGot            = false;
            private bool            mIsOnEnableInvoking     = false;
            
            private IMethod         mOnDisableMethod;
            private bool            mOnDisableGot           = false;
            private bool            mIsOnDisableInvoking    = false;
            
            private bool            mIsOnUnityEventInvoking = false;

            //缓存这个数组来避免调用时的GC Alloc
            private object[]        mParam0                 = new object[2];
            private object[]        mParam1                 = new object[1];

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) 
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get { return __instance; }
            }

            public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
            {
                if (!mIsInitializeInvoking)
                {
                    mIsInitializeInvoking = true;
                    mParam0[0] = rObjs;
                    mParam0[1] = rBaseDatas;
                    mAppdomain.Invoke(this.ParentType, "Initialize", __instance, mParam0);
                    mIsInitializeInvoking = false;
                }
                else
                {
                    base.Initialize(rObjs, rBaseDatas);
                }
            }

            public override void Awake()
            {
                if (!mAwakeGot)
                {
                    mAwakeMethod = __instance.Type.GetMethod("Awake", 0);
                    mAwakeGot = true;
                }

                if (mAwakeMethod != null && !mIsAwakeInvoking)
                {
                    mIsAwakeInvoking = true;
                    mAppdomain.Invoke(mAwakeMethod, __instance);
                    mIsAwakeInvoking = false;
                }
                else
                {
                    base.Awake();
                }
            }

            public override void Start()
            {
                if (!mStartGot)
                {
                    mStartMethod = __instance.Type.GetMethod("Start", 0);
                    mStartGot = true;
                }

                if (mStartMethod != null && !mIsStartInvoking)
                {
                    mIsStartInvoking = true;
                    mAppdomain.Invoke(mStartMethod, __instance);
                    mIsStartInvoking = false;
                }
                else
                {
                    base.Start();
                }
            }

            public override void Update()
            {
                if (!mUpdateGot)
                {
                    mUpdateMethod = __instance.Type.GetMethod("Update", 0);
                    mUpdateGot = true;
                }

                if (mUpdateMethod != null && !mIsUpdateInvoking)
                {
                    mIsUpdateInvoking = true;
                    mAppdomain.Invoke(mUpdateMethod, __instance);
                    mIsUpdateInvoking = false;
                }
                else
                {
                    base.Update();
                }
            }

            public override void OnDestroy()
            {
                if (!mIsOnDestroyInvoking)
                {
                    mIsOnDestroyInvoking = true;
                    mAppdomain.Invoke(this.ParentType, "Destroy", __instance);
                    mIsOnDestroyInvoking = false;
                }
                else
                {
                    base.OnDestroy();
                }
            }

            public override void OnEnable()
            {
                if (!mOnEnableGot)
                {
                    mOnEnableMethod = __instance.Type.GetMethod("OnEnable", 0);
                    mOnEnableGot = true;
                }

                if (mOnEnableMethod != null && !mIsOnEnableInvoking)
                {
                    mIsOnEnableInvoking = true;
                    mAppdomain.Invoke(mOnEnableMethod, __instance);
                    mIsOnEnableInvoking = false;
                }
                else
                {
                    base.OnEnable();
                }
            }

            public override void OnDisable()
            {
                if (!mOnDisableGot)
                {
                    mOnDisableMethod = __instance.Type.GetMethod("OnDisable", 0);
                    mOnDisableGot = true;
                }

                if (mOnDisableMethod != null && !mIsOnDisableInvoking)
                {
                    mIsOnDisableInvoking = true;
                    mAppdomain.Invoke(mOnDisableMethod, __instance);
                    mIsOnDisableInvoking = false;
                }
                else
                {
                    base.OnDisable();
                }
            }

            public override void OnUnityEvent(UnityEngine.Object rTarget)
            {
                if (!mIsOnUnityEventInvoking)
                {
                    mIsOnUnityEventInvoking = true;
                    mParam1[0] = rTarget;
                    mAppdomain.Invoke(this.ParentType, "OnUnityEvent", __instance, mParam1);
                    mIsOnUnityEventInvoking = false;
                }
                else
                {
                    base.OnUnityEvent(rTarget);
                }
            }
        }
    }
}