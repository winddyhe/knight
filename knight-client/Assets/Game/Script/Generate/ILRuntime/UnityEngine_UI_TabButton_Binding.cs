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
    unsafe class UnityEngine_UI_TabButton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.TabButton);

            field = type.GetField("TabIndex", flag);
            app.RegisterCLRFieldGetter(field, get_TabIndex_0);
            app.RegisterCLRFieldSetter(field, set_TabIndex_0);


        }



        static object get_TabIndex_0(ref object o)
        {
            return ((UnityEngine.UI.TabButton)o).TabIndex;
        }
        static void set_TabIndex_0(ref object o, object v)
        {
            ((UnityEngine.UI.TabButton)o).TabIndex = (System.Int32)v;
        }


    }
}
