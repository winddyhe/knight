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
    unsafe class UnityEngine_UI_TabView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.TabView);

            field = type.GetField("TabButtons", flag);
            app.RegisterCLRFieldGetter(field, get_TabButtons_0);
            app.RegisterCLRFieldSetter(field, set_TabButtons_0);
            field = type.GetField("TabTemplateGo", flag);
            app.RegisterCLRFieldGetter(field, get_TabTemplateGo_1);
            app.RegisterCLRFieldSetter(field, set_TabTemplateGo_1);


        }



        static object get_TabButtons_0(ref object o)
        {
            return ((UnityEngine.UI.TabView)o).TabButtons;
        }
        static void set_TabButtons_0(ref object o, object v)
        {
            ((UnityEngine.UI.TabView)o).TabButtons = (System.Collections.Generic.List<UnityEngine.UI.TabButton>)v;
        }
        static object get_TabTemplateGo_1(ref object o)
        {
            return ((UnityEngine.UI.TabView)o).TabTemplateGo;
        }
        static void set_TabTemplateGo_1(ref object o, object v)
        {
            ((UnityEngine.UI.TabView)o).TabTemplateGo = (UnityEngine.GameObject)v;
        }


    }
}
