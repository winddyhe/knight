using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using System;
using Core;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using ILRuntime.CLR.Method;

namespace Framework.Hotfix
{
    public class DictAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IDict); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILRuntime.Runtime.Intepreter.ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : IDict, CrossBindingAdaptorType
        {
            private ILTypeInstance  __instance;
            private AppDomain       mAppdomain;

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get { return __instance; }
            }

            private IMethod         mAddObjectMethod;
            private bool            mAddObjectGot           = false;
            private bool            mIsAddObjectInvoking    = false;
            public void AddObject(object key, object value)
            {

            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public Dictionary<object, object> OriginCollection
            {
                get { throw new NotImplementedException(); }
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
