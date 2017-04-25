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
            
            private bool                mIsInitializeInvoking       = false;

            private bool                mIsGetIsOpenedInvoking      = false;
            private bool                mIsSetIsOpenedInvoking      = false;

            private bool                mIsGetIsClosedInvoking      = false;
            private bool                mIsSetIsClosedInvoking      = false;

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
            
            private bool                mIsTestOnClosingInvoking    = false;

            private IMethod             mOnClosedMethod;
            private bool                mOnClosedGot                = false;
            private bool                mIsTestOnClosedInvoking     = false;
            
            private bool                mIsOnUnityEventInvoking     = false;

            //缓存这个数组来避免调用时的GC Alloc
            private object[]            mParam0                     = new object[2];
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

            public override bool IsOpened
            {
                get
                {
                    bool res = false;
                    if (!mIsGetIsOpenedInvoking)
                    {
                        mIsGetIsOpenedInvoking = true;
                        res = (bool)mAppdomain.Invoke(this.ParentType, "get_IsOpened", __instance);
                        mIsGetIsOpenedInvoking = false;
                    }
                    else
                    {
                        res = base.IsOpened;
                    }
                    return res;
                }

                set
                {
                    if (!mIsSetIsOpenedInvoking)
                    {
                        mIsSetIsOpenedInvoking = true;
                        mParam1[0] = value;
                        mAppdomain.Invoke(this.ParentType, "set_IsOpened", __instance, mParam1);
                        mIsSetIsOpenedInvoking = false;
                    }
                    else
                    {
                        base.IsOpened = value;
                    }
                }
            }

            public override bool IsClosed
            {
                get
                {
                    bool res = false;
                    if (!mIsGetIsClosedInvoking)
                    {
                        mIsGetIsClosedInvoking = true;
                        res = (bool)mAppdomain.Invoke(this.ParentType, "get_IsClosed", __instance);
                        mIsGetIsClosedInvoking = false;
                    }
                    else
                    {
                        res = base.IsClosed;
                    }
                    return res;
                }

                set
                {
                    if (!mIsSetIsClosedInvoking)
                    {
                        mIsSetIsClosedInvoking = true;
                        mParam1[0] = value;
                        mAppdomain.Invoke(this.ParentType, "set_IsClosed", __instance, mParam1);
                        mIsSetIsClosedInvoking = false;
                    }
                    else
                    {
                        base.IsClosed = value;
                    }
                }
            }

            public override void OnOpening()
            {
                if (!mIsTestOnOpeningInvoking)
                {
                    mIsTestOnOpeningInvoking = true;
                    mAppdomain.Invoke(this.ParentType, "Opening", __instance);
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
                    mOnRefreshMethod = __instance.Type.GetMethod("OnRefresh", 0);
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
                if (!mIsTestOnClosingInvoking)
                {
                    mIsTestOnClosingInvoking = true;
                    mAppdomain.Invoke(this.ParentType, "Closing", __instance);
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
                    mOnClosedMethod = __instance.Type.GetMethod("OnClosed", 0);
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
