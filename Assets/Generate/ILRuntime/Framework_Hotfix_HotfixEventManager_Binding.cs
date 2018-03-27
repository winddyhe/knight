using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Framework_Hotfix_HotfixEventManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Framework.Hotfix.HotfixEventManager);
            args = new Type[]{};
            method = type.GetMethod("Initialize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Initialize_0);
            args = new Type[]{typeof(UnityEngine.Object), typeof(Framework.HEventTriggerType), typeof(System.Action<UnityEngine.Object>)};
            method = type.GetMethod("Binding", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Binding_1);
            args = new Type[]{typeof(UnityEngine.Object), typeof(Framework.HEventTriggerType), typeof(System.Action<UnityEngine.Object>)};
            method = type.GetMethod("UnBinding", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnBinding_2);


        }


        static StackObject* Initialize_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Hotfix.HotfixEventManager instance_of_this_method = (Framework.Hotfix.HotfixEventManager)typeof(Framework.Hotfix.HotfixEventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Initialize();

            return __ret;
        }

        static StackObject* Binding_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.Object> @rEventHandler = (System.Action<UnityEngine.Object>)typeof(System.Action<UnityEngine.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.HEventTriggerType @rEventType = (Framework.HEventTriggerType)typeof(Framework.HEventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Object @rTargetGo = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            Framework.Hotfix.HotfixEventManager instance_of_this_method = (Framework.Hotfix.HotfixEventManager)typeof(Framework.Hotfix.HotfixEventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Binding(@rTargetGo, @rEventType, @rEventHandler);

            return __ret;
        }

        static StackObject* UnBinding_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.Object> @rEventHandler = (System.Action<UnityEngine.Object>)typeof(System.Action<UnityEngine.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.HEventTriggerType @rEventType = (Framework.HEventTriggerType)typeof(Framework.HEventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Object @rTargetGo = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            Framework.Hotfix.HotfixEventManager instance_of_this_method = (Framework.Hotfix.HotfixEventManager)typeof(Framework.Hotfix.HotfixEventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UnBinding(@rTargetGo, @rEventType, @rEventHandler);

            return __ret;
        }



    }
}
