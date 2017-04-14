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
    public class HotfixMBInheritAdaptor : CrossBindingAdaptor
    {
        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override Type BaseCLRType
        {
            get { return typeof(HotfixMBInherit); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : HotfixMBInherit, CrossBindingAdaptorType
        {
            private ILTypeInstance  __instance;
            private AppDomain       mAppdomain;
            
            private IMethod         mInitializeMethod;
            private bool            mInitializeGot          = false;
            private bool            mIsInitializeInvoking   = false;

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
        }
    }
}