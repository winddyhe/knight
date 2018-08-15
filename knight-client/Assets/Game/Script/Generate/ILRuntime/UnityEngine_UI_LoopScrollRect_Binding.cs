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
    unsafe class UnityEngine_UI_LoopScrollRect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.UI.LoopScrollRect);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("RefillCells", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefillCells_0);
            args = new Type[]{};
            method = type.GetMethod("RefreshCells", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshCells_1);

            field = type.GetField("OnFillCellFunc", flag);
            app.RegisterCLRFieldGetter(field, get_OnFillCellFunc_0);
            app.RegisterCLRFieldSetter(field, set_OnFillCellFunc_0);
            field = type.GetField("totalCount", flag);
            app.RegisterCLRFieldGetter(field, get_totalCount_1);
            app.RegisterCLRFieldSetter(field, set_totalCount_1);


        }


        static StackObject* RefillCells_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @offset = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.LoopScrollRect instance_of_this_method = (UnityEngine.UI.LoopScrollRect)typeof(UnityEngine.UI.LoopScrollRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefillCells(@offset);

            return __ret;
        }

        static StackObject* RefreshCells_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.LoopScrollRect instance_of_this_method = (UnityEngine.UI.LoopScrollRect)typeof(UnityEngine.UI.LoopScrollRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshCells();

            return __ret;
        }


        static object get_OnFillCellFunc_0(ref object o)
        {
            return ((UnityEngine.UI.LoopScrollRect)o).OnFillCellFunc;
        }
        static void set_OnFillCellFunc_0(ref object o, object v)
        {
            ((UnityEngine.UI.LoopScrollRect)o).OnFillCellFunc = (System.Action<UnityEngine.Transform, System.Int32>)v;
        }
        static object get_totalCount_1(ref object o)
        {
            return ((UnityEngine.UI.LoopScrollRect)o).totalCount;
        }
        static void set_totalCount_1(ref object o, object v)
        {
            ((UnityEngine.UI.LoopScrollRect)o).totalCount = (System.Int32)v;
        }


    }
}
