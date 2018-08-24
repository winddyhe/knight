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
    unsafe class UnityEngine_UI_ViewModelDataSourceTab_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.ViewModelDataSourceTab);

            field = type.GetField("TabView", flag);
            app.RegisterCLRFieldGetter(field, get_TabView_0);
            app.RegisterCLRFieldSetter(field, set_TabView_0);


        }



        static object get_TabView_0(ref object o)
        {
            return ((UnityEngine.UI.ViewModelDataSourceTab)o).TabView;
        }
        static void set_TabView_0(ref object o, object v)
        {
            ((UnityEngine.UI.ViewModelDataSourceTab)o).TabView = (UnityEngine.UI.TabView)v;
        }


    }
}
