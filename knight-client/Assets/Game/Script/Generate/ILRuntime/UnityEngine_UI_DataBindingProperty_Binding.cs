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
    unsafe class UnityEngine_UI_DataBindingProperty_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.DataBindingProperty);
            args = new Type[]{};
            method = type.GetMethod("GetValue", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetValue_0);

            field = type.GetField("Property", flag);
            app.RegisterCLRFieldGetter(field, get_Property_0);
            app.RegisterCLRFieldSetter(field, set_Property_0);
            field = type.GetField("PropertyOwner", flag);
            app.RegisterCLRFieldGetter(field, get_PropertyOwner_1);
            app.RegisterCLRFieldSetter(field, set_PropertyOwner_1);
            field = type.GetField("PropertyOwnerKey", flag);
            app.RegisterCLRFieldGetter(field, get_PropertyOwnerKey_2);
            app.RegisterCLRFieldSetter(field, set_PropertyOwnerKey_2);
            field = type.GetField("ConvertMethod", flag);
            app.RegisterCLRFieldGetter(field, get_ConvertMethod_3);
            app.RegisterCLRFieldSetter(field, set_ConvertMethod_3);
            field = type.GetField("PropertyName", flag);
            app.RegisterCLRFieldGetter(field, get_PropertyName_4);
            app.RegisterCLRFieldSetter(field, set_PropertyName_4);

            args = new Type[]{typeof(System.Object), typeof(System.String), typeof(System.String)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* GetValue_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.DataBindingProperty instance_of_this_method = (UnityEngine.UI.DataBindingProperty)typeof(UnityEngine.UI.DataBindingProperty).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetValue();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }


        static object get_Property_0(ref object o)
        {
            return ((UnityEngine.UI.DataBindingProperty)o).Property;
        }
        static void set_Property_0(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingProperty)o).Property = (System.Reflection.PropertyInfo)v;
        }
        static object get_PropertyOwner_1(ref object o)
        {
            return ((UnityEngine.UI.DataBindingProperty)o).PropertyOwner;
        }
        static void set_PropertyOwner_1(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingProperty)o).PropertyOwner = (System.Object)v;
        }
        static object get_PropertyOwnerKey_2(ref object o)
        {
            return ((UnityEngine.UI.DataBindingProperty)o).PropertyOwnerKey;
        }
        static void set_PropertyOwnerKey_2(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingProperty)o).PropertyOwnerKey = (System.String)v;
        }
        static object get_ConvertMethod_3(ref object o)
        {
            return ((UnityEngine.UI.DataBindingProperty)o).ConvertMethod;
        }
        static void set_ConvertMethod_3(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingProperty)o).ConvertMethod = (System.Reflection.MethodInfo)v;
        }
        static object get_PropertyName_4(ref object o)
        {
            return ((UnityEngine.UI.DataBindingProperty)o).PropertyName;
        }
        static void set_PropertyName_4(ref object o, object v)
        {
            ((UnityEngine.UI.DataBindingProperty)o).PropertyName = (System.String)v;
        }

        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @rPropName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @rPropOwnerKey = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Object @rPropOwner = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new UnityEngine.UI.DataBindingProperty(@rPropOwner, @rPropOwnerKey, @rPropName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
