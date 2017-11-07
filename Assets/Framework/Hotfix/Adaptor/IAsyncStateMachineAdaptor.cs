//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Runtime.CompilerServices;

namespace Framework.Hotfix
{
    public class IAsyncStateMachineAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IAsyncStateMachine); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor : IAsyncStateMachine, CrossBindingAdaptorType
        {
            private ILTypeInstance                          __instance;
            private ILRuntime.Runtime.Enviorment.AppDomain  mAppDomain;

            private IMethod                                 mMoveNext;
            private IMethod                                 mSetStateMachine;
            
            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain rAppDomain, ILTypeInstance rInstance)
            {
                this.mAppDomain = rAppDomain;
                this.__instance = rInstance;
            }

            public ILTypeInstance ILInstance { get { return __instance; } }

            public void MoveNext()
            {
                if (this.mMoveNext == null)
                {
                    mMoveNext = __instance.Type.GetMethod("MoveNext", 0);
                }
                this.mAppDomain.Invoke(mMoveNext, __instance, null);
            }

            public void SetStateMachine(IAsyncStateMachine rSstateMachine)
            {
                if (this.mSetStateMachine == null)
                {
                    mSetStateMachine = __instance.Type.GetMethod("SetStateMachine");
                }
                this.mAppDomain.Invoke(mSetStateMachine, __instance, rSstateMachine);
            }

            public override string ToString()
            {
                IMethod m = this.mAppDomain.ObjectType.GetMethod("ToString", 0);
                m = __instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return __instance.ToString();
                }
                return __instance.Type.FullName;
            }
        }
    }
}
