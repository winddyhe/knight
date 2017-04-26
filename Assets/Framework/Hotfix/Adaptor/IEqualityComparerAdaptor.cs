//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;

namespace Framework.Hotfix
{

    public class IEqualityComparerAdaptor : CrossBindingAdaptor
    {
        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override Type BaseCLRType
        {
            get { return typeof(IEqualityComparer<object>); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        internal class Adaptor : IEqualityComparer<object>, CrossBindingAdaptorType
        {
            private ILTypeInstance                          __instance;
            private ILRuntime.Runtime.Enviorment.AppDomain  mAppdomain;

            private IMethod                                 mEqualsMethod;
            private bool                                    mEqualsMethodGot        = false;

            private IMethod                                 mGetHashCodeMethod;
            private bool                                    mGetHashCodeMethodGot   = false;

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance ILInstance { get { return __instance; } }

            public new bool Equals(object x, object y)
            {
                if (!mEqualsMethodGot)
                {
                    mEqualsMethod = __instance.Type.GetMethod("Equals", 2);
                    mEqualsMethodGot = true;
                }

                if (mEqualsMethod != null)
                {
                    return (bool)mAppdomain.Invoke(mEqualsMethod, __instance, x, y);
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(object obj)
            {
                if (!mGetHashCodeMethodGot)
                {
                    mGetHashCodeMethod = __instance.Type.GetMethod("MoveNext", 1);
                    mGetHashCodeMethodGot = true;
                }

                if (mGetHashCodeMethod != null)
                {
                    return (int)mAppdomain.Invoke(mGetHashCodeMethod, __instance, obj);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
