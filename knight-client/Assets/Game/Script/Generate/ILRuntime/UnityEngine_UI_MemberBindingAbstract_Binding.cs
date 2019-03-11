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
    unsafe class UnityEngine_UI_MemberBindingAbstract_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.MemberBindingAbstract);
            args = new Type[]{};
            method = type.GetMethod("OnDestroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnDestroy_0);
            args = new Type[]{};
            method = type.GetMethod("SyncFromViewModel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SyncFromViewModel_1);

            field = type.GetField("ViewModelProp", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelProp_0);
            app.RegisterCLRFieldSetter(field, set_ViewModelProp_0);
            field = type.GetField("ViewModelPropertyWatcher", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelPropertyWatcher_1);
            app.RegisterCLRFieldSetter(field, set_ViewModelPropertyWatcher_1);
            field = type.GetField("IsListTemplate", flag);
            app.RegisterCLRFieldGetter(field, get_IsListTemplate_2);
            app.RegisterCLRFieldSetter(field, set_IsListTemplate_2);
            field = type.GetField("ViewPath", flag);
            app.RegisterCLRFieldGetter(field, get_ViewPath_3);
            app.RegisterCLRFieldSetter(field, set_ViewPath_3);
            field = type.GetField("ViewProp", flag);
            app.RegisterCLRFieldGetter(field, get_ViewProp_4);
            app.RegisterCLRFieldSetter(field, set_ViewProp_4);
            field = type.GetField("ViewModelPath", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelPath_5);
            app.RegisterCLRFieldSetter(field, set_ViewModelPath_5);


        }


        static StackObject* OnDestroy_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.MemberBindingAbstract instance_of_this_method = (UnityEngine.UI.MemberBindingAbstract)typeof(UnityEngine.UI.MemberBindingAbstract).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDestroy();

            return __ret;
        }

        static StackObject* SyncFromViewModel_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.MemberBindingAbstract instance_of_this_method = (UnityEngine.UI.MemberBindingAbstract)typeof(UnityEngine.UI.MemberBindingAbstract).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SyncFromViewModel();

            return __ret;
        }


        static object get_ViewModelProp_0(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelProp;
        }
        static void set_ViewModelProp_0(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelProp = (UnityEngine.UI.DataBindingProperty)v;
        }
        static object get_ViewModelPropertyWatcher_1(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelPropertyWatcher;
        }
        static void set_ViewModelPropertyWatcher_1(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelPropertyWatcher = (UnityEngine.UI.DataBindingPropertyWatcher)v;
        }
        static object get_IsListTemplate_2(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).IsListTemplate;
        }
        static void set_IsListTemplate_2(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).IsListTemplate = (System.Boolean)v;
        }
        static object get_ViewPath_3(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).ViewPath;
        }
        static void set_ViewPath_3(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).ViewPath = (System.String)v;
        }
        static object get_ViewProp_4(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).ViewProp;
        }
        static void set_ViewProp_4(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).ViewProp = (UnityEngine.UI.DataBindingProperty)v;
        }
        static object get_ViewModelPath_5(ref object o)
        {
            return ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelPath;
        }
        static void set_ViewModelPath_5(ref object o, object v)
        {
            ((UnityEngine.UI.MemberBindingAbstract)o).ViewModelPath = (System.String)v;
        }


    }
}
