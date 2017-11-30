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
    unsafe class Framework_AvatarLoaderRequest_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.AvatarLoaderRequest);

            field = type.GetField("AvatarGo", flag);
            app.RegisterCLRFieldGetter(field, get_AvatarGo_0);
            app.RegisterCLRFieldSetter(field, set_AvatarGo_0);


        }



        static object get_AvatarGo_0(ref object o)
        {
            return ((Framework.AvatarLoaderRequest)o).AvatarGo;
        }
        static void set_AvatarGo_0(ref object o, object v)
        {
            ((Framework.AvatarLoaderRequest)o).AvatarGo = (UnityEngine.GameObject)v;
        }


    }
}
