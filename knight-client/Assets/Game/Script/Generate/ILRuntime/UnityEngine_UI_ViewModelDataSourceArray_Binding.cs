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
    unsafe class UnityEngine_UI_ViewModelDataSourceArray_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.ViewModelDataSourceArray);

            field = type.GetField("HasInitData", flag);
            app.RegisterCLRFieldGetter(field, get_HasInitData_0);
            app.RegisterCLRFieldSetter(field, set_HasInitData_0);
            field = type.GetField("ItemTemplateGo", flag);
            app.RegisterCLRFieldGetter(field, get_ItemTemplateGo_1);
            app.RegisterCLRFieldSetter(field, set_ItemTemplateGo_1);


        }



        static object get_HasInitData_0(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceArray)o).HasInitData;
        }
        static void set_HasInitData_0(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceArray)o).HasInitData = (System.Boolean)v;
        }
        static object get_ItemTemplateGo_1(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceArray)o).ItemTemplateGo;
        }
        static void set_ItemTemplateGo_1(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceArray)o).ItemTemplateGo = (UnityEngine.GameObject)v;
        }


    }
}
