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
    unsafe class Knight_Core_AssetLoaderRequest_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Knight.Core.AssetLoaderRequest);

            field = type.GetField("Asset", flag);
            app.RegisterCLRFieldGetter(field, get_Asset_0);
            app.RegisterCLRFieldSetter(field, set_Asset_0);


        }



        static object get_Asset_0(ref object o)
        {
            return ((Knight.Core.AssetLoaderRequest)o).Asset;
        }
        static void set_Asset_0(ref object o, object v)
        {
            ((Knight.Core.AssetLoaderRequest)o).Asset = (UnityEngine.Object)v;
        }


    }
}
