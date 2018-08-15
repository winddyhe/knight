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
    unsafe class UnityEngine_UI_ViewModelContainer_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.ViewModelContainer);

            field = type.GetField("ViewModelClass", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelClass_0);
            app.RegisterCLRFieldSetter(field, set_ViewModelClass_0);
            field = type.GetField("ViewModels", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModels_1);
            app.RegisterCLRFieldSetter(field, set_ViewModels_1);
            field = type.GetField("EventBindings", flag);
            app.RegisterCLRFieldGetter(field, get_EventBindings_2);
            app.RegisterCLRFieldSetter(field, set_EventBindings_2);


        }



        static object get_ViewModelClass_0(ref object o)
        {
            return ((UnityEngine.UI.ViewModelContainer)o).ViewModelClass;
        }
        static void set_ViewModelClass_0(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelContainer)o).ViewModelClass = (System.String)v;
        }
        static object get_ViewModels_1(ref object o)
        {
            return ((UnityEngine.UI.ViewModelContainer)o).ViewModels;
        }
        static void set_ViewModels_1(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelContainer)o).ViewModels = (System.Collections.Generic.List<UnityEngine.UI.ViewModelDataSource>)v;
        }
        static object get_EventBindings_2(ref object o)
        {
            return ((UnityEngine.UI.ViewModelContainer)o).EventBindings;
        }
        static void set_EventBindings_2(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelContainer)o).EventBindings = (System.Collections.Generic.List<UnityEngine.UI.EventBinding>)v;
        }


    }
}
