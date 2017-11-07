//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

namespace Framework.Hotfix
{
    public class CoroutineAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return null; }
        }

        public override Type[] BaseCLRTypes
        {
            get
            {
                //跨域继承只能有1个Adapter，因此应该尽量避免一个类同时实现多个外部接口，对于coroutine来说是IEnumerator<object>,IEnumerator和IDisposable，
                //ILRuntime虽然支持，但是一定要小心这种用法，使用不当很容易造成不可预期的问题
                //日常开发如果需要实现多个DLL外部接口，请在Unity这边先做一个基类实现那些个接口，然后继承那个基类
                return new Type[] { typeof(IEnumerator<object>), typeof(IEnumerator), typeof(IDisposable) };
            }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        //Coroutine生成的类实现了IEnumerator<System.Object>, IEnumerator, IDisposable,所以都要实现，这个可以通过reflector之类的IL反编译软件得知
        internal class Adaptor : IEnumerator<System.Object>, IEnumerator, IDisposable, CrossBindingAdaptorType
        {
            private ILTypeInstance                          __instance;
            private ILRuntime.Runtime.Enviorment.AppDomain  mAppdomain;

            private IMethod                                 mCurrentMethod;
            private bool                                    mCurrentMethodGot   = false;

            private IMethod                                 mDisposeMethod;
            private bool                                    mDisposeMethodGot   = false;
            
            private IMethod                                 mMoveNextMethod;
            private bool                                    mMoveNextMethodGot  = false;
            
            private IMethod                                 mResetMethod;
            private bool                                    mResetMethodGot     = false;

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance ILInstance { get { return __instance; } }

            public object Current
            {
                get
                {
                    if (!mCurrentMethodGot)
                    {
                        mCurrentMethod = __instance.Type.GetMethod("get_Current", 0);
                        if (mCurrentMethod == null)
                        {
                            //这里写System.Collections.IEnumerator.get_Current而不是直接get_Current是因为coroutine生成的类是显式实现这个接口的，通过Reflector等反编译软件可得知
                            //为了兼容其他只实现了单一Current属性的，所以上面先直接取了get_Current
                            mCurrentMethod = __instance.Type.GetMethod("System.Collections.IEnumerator.get_Current", 0);
                        }
                        mCurrentMethodGot = true;
                    }

                    if (mCurrentMethod != null)
                    {
                        var res = mAppdomain.Invoke(mCurrentMethod, __instance, null);
                        return res;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public void Dispose()
            {
                if (!mDisposeMethodGot)
                {
                    mDisposeMethod = __instance.Type.GetMethod("Dispose", 0);
                    if (mDisposeMethod == null)
                    {
                        mDisposeMethod = __instance.Type.GetMethod("System.IDisposable.Dispose", 0);
                    }
                    mDisposeMethodGot = true;
                }

                if (mDisposeMethod != null)
                {
                    mAppdomain.Invoke(mDisposeMethod, __instance, null);
                }
            }

            public bool MoveNext()
            {
                if (!mMoveNextMethodGot)
                {
                    mMoveNextMethod = __instance.Type.GetMethod("MoveNext", 0);
                    mMoveNextMethodGot = true;
                }

                if (mMoveNextMethod != null)
                {
                    return (bool)mAppdomain.Invoke(mMoveNextMethod, __instance, null);
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                if (!mResetMethodGot)
                {
                    mResetMethod = __instance.Type.GetMethod("Reset", 0);
                    mResetMethodGot = true;
                }

                if (mResetMethod != null)
                {
                    mAppdomain.Invoke(mResetMethod, __instance, null);
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
