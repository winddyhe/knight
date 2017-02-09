using Framework.WindUI;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Hotfix
{
    public class ViewControllerHotfixAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IViewController); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        class Adaptor : IViewController, CrossBindingAdaptorType
        {
            private ILTypeInstance      __instance;

            private AppDomain           mAppdomain;
            
            private IMethod             mOnOpeningMethod;
            private bool                mIsTestOnOpeningInvoking    = false;

            private IMethod             mOnOpenedMethod;
            private bool                mIsTestOnOpenedInvoking     = false;

            private IMethod             mOnShowMethod;
            private bool                mIsTestOnShowInvoking       = false;

            private IMethod             mOnHideMethod;
            private bool                mIsTestOnHideInvoking       = false;

            private IMethod             mOnRefreshMethod;
            private bool                mIsTestOnRefreshInvoking    = false;

            private IMethod             mOnClosingMethod;
            private bool                mIsTestOnClosingInvoking    = false;

            private IMethod             mOnClosedMethod;
            private bool                mIsTestOnClosedInvoking     = false;

            public Adaptor()
            {
            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.mAppdomain = appdomain;
                this.__instance = instance;
            }

            public ILTypeInstance       ILInstance { get { return __instance; } }

            public override void OnOpening()
            {
                if (mOnOpeningMethod == null)
                {
                    mOnOpeningMethod = __instance.Type.GetMethod("OnOpening", 0);
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
                if (mOnOpenedMethod == null)
                {
                    mOnOpenedMethod = __instance.Type.GetMethod("OnOpened", 0);
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
                if (mOnShowMethod == null)
                {
                    mOnShowMethod = __instance.Type.GetMethod("OnShow", 0);
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
                if (mOnHideMethod == null)
                {
                    mOnHideMethod = __instance.Type.GetMethod("OnHide", 0);
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
                if (mOnRefreshMethod == null)
                {
                    mOnRefreshMethod = __instance.Type.GetMethod("OnHide", 0);
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
                if (mOnClosingMethod == null)
                {
                    mOnClosingMethod = __instance.Type.GetMethod("OnHide", 0);
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
                if (mOnClosedMethod == null)
                {
                    mOnClosedMethod = __instance.Type.GetMethod("OnHide", 0);
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
