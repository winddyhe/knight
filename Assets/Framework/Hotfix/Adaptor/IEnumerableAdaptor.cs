//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;

namespace Framework.Hotfix
{
    public class IEnumerableAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IEnumerable); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        internal class Adaptor : IEnumerable, CrossBindingAdaptorType
        {
            private ILTypeInstance                          __instance;
            private ILRuntime.Runtime.Enviorment.AppDomain  mAppdomain;

            private IMethod                                 mGetEnumeratorMethod;
            private bool                                    mGetEnumeratorMethodGot = false;

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance ILInstance { get { return __instance; } }

            public IEnumerator GetEnumerator()
            {
                if (!mGetEnumeratorMethodGot)
                {
                    mGetEnumeratorMethod = __instance.Type.GetMethod("GetEnumerator", 0);
                    mGetEnumeratorMethodGot = true;
                }

                if (mGetEnumeratorMethod != null)
                {
                    var res = mAppdomain.Invoke(mGetEnumeratorMethod, __instance, null);
                    return (IEnumerator)res;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
