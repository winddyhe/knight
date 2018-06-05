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
    unsafe class Knight_Framework_Hotfix_UnityObject_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Knight.Framework.Hotfix.UnityObject);

            field = type.GetField("Name", flag);
            app.RegisterCLRFieldGetter(field, get_Name_0);
            app.RegisterCLRFieldSetter(field, set_Name_0);
            field = type.GetField("Type", flag);
            app.RegisterCLRFieldGetter(field, get_Type_1);
            app.RegisterCLRFieldSetter(field, set_Type_1);
            field = type.GetField("Object", flag);
            app.RegisterCLRFieldGetter(field, get_Object_2);
            app.RegisterCLRFieldSetter(field, set_Object_2);


        }



        static object get_Name_0(ref object o)
        {
            return ((Knight.Framework.Hotfix.UnityObject)o).Name;
        }
        static void set_Name_0(ref object o, object v)
        {
            ((Knight.Framework.Hotfix.UnityObject)o).Name = (System.String)v;
        }
        static object get_Type_1(ref object o)
        {
            return ((Knight.Framework.Hotfix.UnityObject)o).Type;
        }
        static void set_Type_1(ref object o, object v)
        {
            ((Knight.Framework.Hotfix.UnityObject)o).Type = (System.String)v;
        }
        static object get_Object_2(ref object o)
        {
            return ((Knight.Framework.Hotfix.UnityObject)o).Object;
        }
        static void set_Object_2(ref object o, object v)
        {
            ((Knight.Framework.Hotfix.UnityObject)o).Object = (UnityEngine.Object)v;
        }


    }
}
