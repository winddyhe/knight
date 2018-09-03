using System;
using System.Collections.Generic;
using System.Linq;
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
    unsafe class UnityEngine_UI_EventBinding_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.EventBinding);
            args = new Type[]{typeof(System.Action<Knight.Framework.EventArg>)};
            method = type.GetMethod("InitEventWatcher", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitEventWatcher_0);
            args = new Type[]{};
            method = type.GetMethod("OnDestroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnDestroy_1);

            field = type.GetField("ViewModelMethod", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelMethod_0);
            app.RegisterCLRFieldSetter(field, set_ViewModelMethod_0);
            field = type.GetField("IsListTemplate", flag);
            app.RegisterCLRFieldGetter(field, get_IsListTemplate_1);
            app.RegisterCLRFieldSetter(field, set_IsListTemplate_1);


        }


        static StackObject* InitEventWatcher_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<Knight.Framework.EventArg> @rAction = (System.Action<Knight.Framework.EventArg>)typeof(System.Action<Knight.Framework.EventArg>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.EventBinding instance_of_this_method = (UnityEngine.UI.EventBinding)typeof(UnityEngine.UI.EventBinding).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitEventWatcher(@rAction);

            return __ret;
        }

        static StackObject* OnDestroy_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.EventBinding instance_of_this_method = (UnityEngine.UI.EventBinding)typeof(UnityEngine.UI.EventBinding).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDestroy();

            return __ret;
        }


        static object get_ViewModelMethod_0(ref object o)
        {
            return ((UnityEngine.UI.EventBinding)o).ViewModelMethod;
        }
        static void set_ViewModelMethod_0(ref object o, object v)
        {
            ((UnityEngine.UI.EventBinding)o).ViewModelMethod = (System.String)v;
        }
        static object get_IsListTemplate_1(ref object o)
        {
            return ((UnityEngine.UI.EventBinding)o).IsListTemplate;
        }
        static void set_IsListTemplate_1(ref object o, object v)
        {
            ((UnityEngine.UI.EventBinding)o).IsListTemplate = (System.Boolean)v;
        }


    }
}
