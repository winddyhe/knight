//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.WindUI;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Hotfix
{
    public class ViewControllerAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(ViewController); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : ViewController, CrossBindingAdaptorType
        {
            private ILTypeInstance      __instance;

            private AppDomain           mAppdomain;

            private IMethod             mInitializeMethod;
            private bool                mInitializeGot              = false;
            private bool                mIsInitializeInvoking       = false;

            private IMethod             mOnOpeningMethod;
            private bool                mOnOpeningGot               = false;
            private bool                mIsTestOnOpeningInvoking    = false;

            private IMethod             mOnOpenedMethod;
            private bool                mOnOpenedGot                = false;
            private bool                mIsTestOnOpenedInvoking     = false;

            private IMethod             mOnShowMethod;
            private bool                mOnShowGot                  = false;
            private bool                mIsTestOnShowInvoking       = false;

            private IMethod             mOnHideMethod;
            private bool                mOnHideGot                  = false;
            private bool                mIsTestOnHideInvoking       = false;

            private IMethod             mOnRefreshMethod;
            private bool                mOnRefreshGot               = false;
            private bool                mIsTestOnRefreshInvoking    = false;

            private IMethod             mOnClosingMethod;
            private bool                mOnClosingGot               = false;
            private bool                mIsTestOnClosingInvoking    = false;

            private IMethod             mOnClosedMethod;
            private bool                mOnClosedGot                = false;
            private bool                mIsTestOnClosedInvoking     = false;

            private IMethod             mOnUnityEventMethod;
            private bool                mOnUnityEventGot        = false;
            private bool                mIsOnUnityEventInvoking = false;
            
            //缓存这个数组来避免调用时的GC Alloc
            private object[]            mParam1                     = new object[1];

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance       ILInstance { get { return __instance; } }

            public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
            {
                if (!mInitializeGot)
                {
                    mInitializeMethod = __instance.Type.GetMethod("Initialize", 1);
                    mInitializeGot = true;
                }

                if (mInitializeMethod != null && !mIsInitializeInvoking)
                {
                    mIsInitializeInvoking = true;
                    mParam1[0] = rObjs;
                    mAppdomain.Invoke(mInitializeMethod, __instance, mParam1);
                    mIsInitializeInvoking = false;
                }
                else
                    base.Initialize(rObjs, rBaseDatas);
            }

            public override void OnOpening()
            {
                if (!mOnOpeningGot)
                {
                    mOnOpeningMethod = __instance.Type.GetMethod("OnOpening", 0);
                    mOnOpeningGot = true;
                }

                if (mOnOpeningMethod != null && !mIsTestOnOpeningInvoking)
                {
                    mIsTestOnOpeningInvoking = true;
                    mAppdomain.Invoke(mOnOpeningMethod, __instance);
                    mIsTestOnOpeningInvoking = false;
                }
                else
                {
                    base.OnOpening();
                }
            }

            public override void OnOpened()
            {
                if (!mOnOpenedGot)
                {
                    mOnOpenedMethod = __instance.Type.GetMethod("OnOpened", 0);
                    mOnOpenedGot = true;
                }

                if (mOnOpenedMethod != null && !mIsTestOnOpenedInvoking)
                {
                    mIsTestOnOpenedInvoking = true;
                    mAppdomain.Invoke(mOnOpenedMethod, __instance);
                    mIsTestOnOpenedInvoking = false;
                }
                else
                {
                    base.OnOpened();
                }
            }

            public override void OnShow()
            {
                if (!mOnShowGot)
                {
                    mOnShowMethod = __instance.Type.GetMethod("OnShow", 0);
                    mOnShowGot = true;
                }

                if (mOnShowMethod != null && !mIsTestOnShowInvoking)
                {
                    mIsTestOnShowInvoking = true;
                    mAppdomain.Invoke(mOnShowMethod, __instance);
                    mIsTestOnShowInvoking = false;
                }
                else
                {
                    base.OnShow();
                }
            }

            public override void OnHide()
            {
                if (!mOnHideGot)
                {
                    mOnHideMethod = __instance.Type.GetMethod("OnHide", 0);
                    mOnHideGot = true;
                }

                if (mOnHideMethod != null && !mIsTestOnHideInvoking)
                {
                    mIsTestOnHideInvoking = true;
                    mAppdomain.Invoke(mOnHideMethod, __instance);
                    mIsTestOnHideInvoking = false;
                }
                else
                {
                    base.OnHide();
                }
            }

            public override void OnRefresh()
            {
                if (!mOnRefreshGot)
                {
                    mOnRefreshMethod = __instance.Type.GetMethod("OnHide", 0);
                    mOnRefreshGot = true;
                }

                if (mOnRefreshMethod != null && !mIsTestOnRefreshInvoking)
                {
                    mIsTestOnRefreshInvoking = true;
                    mAppdomain.Invoke(mOnRefreshMethod, __instance);
                    mIsTestOnRefreshInvoking = false;
                }
                else
                {
                    base.OnRefresh();
                }
            }

            public override void OnClosing()
            {
                if (!mOnClosingGot)
                {
                    mOnClosingMethod = __instance.Type.GetMethod("OnHide", 0);
                    mOnClosingGot = true;
                }

                if (mOnClosingMethod != null && !mIsTestOnClosingInvoking)
                {
                    mIsTestOnClosingInvoking = true;
                    mAppdomain.Invoke(mOnClosingMethod, __instance);
                    mIsTestOnClosingInvoking = false;
                }
                else
                {
                    base.OnClosing();
                }
            }

            public override void OnClosed()
            {
                if (!mOnClosedGot)
                {
                    mOnClosedMethod = __instance.Type.GetMethod("OnHide", 0);
                    mOnClosedGot = true;
                }

                if (mOnClosedMethod != null && !mIsTestOnClosedInvoking)
                {
                    mIsTestOnClosedInvoking = true;
                    mAppdomain.Invoke(mOnClosedMethod, __instance);
                    mIsTestOnClosedInvoking = false;
                }
                else
                {
                    base.OnClosed();
                }
            }

            public override void OnUnityEvent(UnityEngine.Object rTarget)
            {
                if (!mOnUnityEventGot)
                {
                    mOnUnityEventMethod = __instance.Type.GetMethod("OnUnityEvent", 1);
                    mOnUnityEventGot = true;
                }

                if (mOnUnityEventMethod != null && !mIsOnUnityEventInvoking)
                {
                    mIsOnUnityEventInvoking = true;
                    mParam1[0] = rTarget;
                    mAppdomain.Invoke(mOnUnityEventMethod, __instance, mParam1);
                    mIsOnUnityEventInvoking = false;
                }
                else
                {
                    base.OnUnityEvent(rTarget);
                }
            }

            public override string ToString()
            {
                IMethod m = mAppdomain.ObjectType.GetMethod("ToString", 0);
                m = __instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return __instance.ToString();
                }
                else
                    return __instance.Type.FullName;
            }
        }
    }
}
