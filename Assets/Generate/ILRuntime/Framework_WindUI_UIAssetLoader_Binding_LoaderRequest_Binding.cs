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
    unsafe class Framework_WindUI_UIAssetLoader_Binding_LoaderRequest_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.WindUI.UIAssetLoader.LoaderRequest);

            field = type.GetField("ViewPrefabGo", flag);
            app.RegisterCLRFieldGetter(field, get_ViewPrefabGo_0);
            app.RegisterCLRFieldSetter(field, set_ViewPrefabGo_0);


        }



        static object get_ViewPrefabGo_0(ref object o)
        {
            return ((Framework.WindUI.UIAssetLoader.LoaderRequest)o).ViewPrefabGo;
        }
        static void set_ViewPrefabGo_0(ref object o, object v)
        {
            ((Framework.WindUI.UIAssetLoader.LoaderRequest)o).ViewPrefabGo = (UnityEngine.GameObject)v;
        }


    }
}
