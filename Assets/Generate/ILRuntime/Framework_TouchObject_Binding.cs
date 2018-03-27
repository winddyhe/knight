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
    unsafe class Framework_TouchObject_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.TouchObject);

            field = type.GetField("position", flag);
            app.RegisterCLRFieldGetter(field, get_position_0);
            app.RegisterCLRFieldSetter(field, set_position_0);
            field = type.GetField("phase", flag);
            app.RegisterCLRFieldGetter(field, get_phase_1);
            app.RegisterCLRFieldSetter(field, set_phase_1);
            field = type.GetField("deltaPosition", flag);
            app.RegisterCLRFieldGetter(field, get_deltaPosition_2);
            app.RegisterCLRFieldSetter(field, set_deltaPosition_2);


        }



        static object get_position_0(ref object o)
        {
            return ((Framework.TouchObject)o).position;
        }
        static void set_position_0(ref object o, object v)
        {
            ((Framework.TouchObject)o).position = (UnityEngine.Vector2)v;
        }
        static object get_phase_1(ref object o)
        {
            return ((Framework.TouchObject)o).phase;
        }
        static void set_phase_1(ref object o, object v)
        {
            ((Framework.TouchObject)o).phase = (UnityEngine.TouchPhase)v;
        }
        static object get_deltaPosition_2(ref object o)
        {
            return ((Framework.TouchObject)o).deltaPosition;
        }
        static void set_deltaPosition_2(ref object o, object v)
        {
            ((Framework.TouchObject)o).deltaPosition = (UnityEngine.Vector2)v;
        }


    }
}
