using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using ILRuntime.CLR.Method;
using UnityEngine;

namespace Framework.Hotfix
{
    public class MonoBehaviourProxyAdaptor : CrossBindingAdaptor
    {
        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override Type BaseCLRType
        {
            get { return typeof(MonoBehaviourProxy); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : MonoBehaviourProxy, CrossBindingAdaptorType
        {
            private ILTypeInstance  __instance;
            private AppDomain       mAppdomain;
            
            private IMethod         mSetObjectsMethod;
            private bool            mSetObjectsGot          = false;
            private bool            mIsSetObjectsInvoking   = false;

            private IMethod         mStartMethod;
            private bool            mStartGot               = false;
            private bool            mIsStartInvoking        = false;
            
            private IMethod         mUpdateMethod;
            private bool            mUpdateGot              = false;
            private bool            mIsUpdateInvoking       = false;

            private IMethod         mOnDestroyMethod;
            private bool            mOnDestroyGot           = false;
            private bool            mIsOnDestroyInvoking    = false;
                        
            private IMethod         mOnEnableMethod;
            private bool            mOnEnableGot            = false;
            private bool            mIsOnEnableInvoking     = false;
            
            private IMethod         mOnDisableMethod;
            private bool            mOnDisableGot           = false;
            private bool            mIsOnDisableInvoking    = false;

            //缓存这个数组来避免调用时的GC Alloc
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

            public override void SetObjects(List<UnityEngine.Object> rObjs)
            {
                if (!mSetObjectsGot)
                {
                    mSetObjectsMethod = __instance.Type.GetMethod("SetObjects", 1);
                    mSetObjectsGot = true;
                }

                if (mSetObjectsMethod != null && !mIsSetObjectsInvoking)
                {
                    mIsSetObjectsInvoking = true;
                    mParam1[0] = rObjs;
                    mAppdomain.Invoke(mSetObjectsMethod, __instance, mParam1);
                    mIsSetObjectsInvoking = false;
                }
                else
                    base.SetObjects(rObjs);
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
                if (!mOnDestroyGot)
                {
                    mOnDestroyMethod = __instance.Type.GetMethod("OnDestroy", 0);
                    mOnDestroyGot = true;
                }

                if (mOnDestroyMethod != null && !mIsOnDestroyInvoking)
                {
                    mIsOnDestroyInvoking = true;
                    mAppdomain.Invoke(mOnDestroyMethod, __instance);
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
        }
    }
}
