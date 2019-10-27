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
    unsafe class UnityEngine_UI_ViewModelDataSourceTemplate_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.ViewModelDataSourceTemplate);

            field = type.GetField("ViewModelProp", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelProp_0);
            app.RegisterCLRFieldSetter(field, set_ViewModelProp_0);
            field = type.GetField("ViewModelPropertyWatcher", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelPropertyWatcher_1);
            app.RegisterCLRFieldSetter(field, set_ViewModelPropertyWatcher_1);
            field = type.GetField("ViewModelPath", flag);
            app.RegisterCLRFieldGetter(field, get_ViewModelPath_2);
            app.RegisterCLRFieldSetter(field, set_ViewModelPath_2);


        }



        static object get_ViewModelProp_0(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelProp;
        }
        static void set_ViewModelProp_0(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelProp = (UnityEngine.UI.DataBindingProperty)v;
        }
        static object get_ViewModelPropertyWatcher_1(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelPropertyWatcher;
        }
        static void set_ViewModelPropertyWatcher_1(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelPropertyWatcher = (UnityEngine.UI.DataBindingPropertyWatcher)v;
        }
        static object get_ViewModelPath_2(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelPath;
        }
        static void set_ViewModelPath_2(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceTemplate)o).ViewModelPath = (System.String)v;
        }


    }
}
