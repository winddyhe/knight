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
    unsafe class UnityEngine_UI_DataBindingRelatedAttribute_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.DataBindingRelatedAttribute);

            field = type.GetField("RelatedProps", flag);
            app.RegisterCLRFieldGetter(field, get_RelatedProps_0);
            app.RegisterCLRFieldSetter(field, set_RelatedProps_0);


        }



        static object get_RelatedProps_0(ref object o)
        {
            return ((UnityEngine.UI.DataBindingRelatedAttribute)o).RelatedProps;
        }
        static void set_RelatedProps_0(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingRelatedAttribute)o).RelatedProps = (System.String)v;
        }


    }
}
