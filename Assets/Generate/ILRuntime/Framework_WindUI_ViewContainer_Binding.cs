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
    unsafe class Framework_WindUI_ViewContainer_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.WindUI.ViewContainer);

            field = type.GetField("GUID", flag);
            app.RegisterCLRFieldGetter(field, get_GUID_0);
            app.RegisterCLRFieldSetter(field, set_GUID_0);
            field = type.GetField("ViewName", flag);
            app.RegisterCLRFieldGetter(field, get_ViewName_1);
            app.RegisterCLRFieldSetter(field, set_ViewName_1);
            field = type.GetField("CurState", flag);
            app.RegisterCLRFieldGetter(field, get_CurState_2);
            app.RegisterCLRFieldSetter(field, set_CurState_2);


        }



        static object get_GUID_0(ref object o)
        {
            return ((Framework.WindUI.ViewContainer)o).GUID;
        }
        static void set_GUID_0(ref object o, object v)
        {
            ((Framework.WindUI.ViewContainer)o).GUID = (System.String)v;
        }
        static object get_ViewName_1(ref object o)
        {
            return ((Framework.WindUI.ViewContainer)o).ViewName;
        }
        static void set_ViewName_1(ref object o, object v)
        {
            ((Framework.WindUI.ViewContainer)o).ViewName = (System.String)v;
        }
        static object get_CurState_2(ref object o)
        {
            return ((Framework.WindUI.ViewContainer)o).CurState;
        }
        static void set_CurState_2(ref object o, object v)
        {
            ((Framework.WindUI.ViewContainer)o).CurState = (Framework.WindUI.ViewContainer.State)v;
        }


    }
}
