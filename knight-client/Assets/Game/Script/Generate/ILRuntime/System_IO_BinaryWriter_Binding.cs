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
    unsafe class System_IO_BinaryWriter_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.IO.BinaryWriter);
            args = new Type[]{typeof(System.Char)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_0);
            args = new Type[]{typeof(System.Byte)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_1);
            args = new Type[]{typeof(System.SByte)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_3);
            args = new Type[]{typeof(System.Int16)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_4);
            args = new Type[]{typeof(System.UInt16)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_5);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_6);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_7);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_8);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_9);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_10);
            args = new Type[]{typeof(System.Double)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_11);
            args = new Type[]{typeof(System.Decimal)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_12);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_13);

            args = new Type[]{typeof(System.IO.Stream)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* Write_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Char @ch = (char)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@ch);

            return __ret;
        }

        static StackObject* Write_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte @value = (byte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.SByte @value = (sbyte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int16 @value = (short)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt16 @value = (ushort)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @value = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @value = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Decimal @value = (System.Decimal)typeof(System.Decimal).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }

        static StackObject* Write_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.IO.BinaryWriter instance_of_this_method = (System.IO.BinaryWriter)typeof(System.IO.BinaryWriter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(@value);

            return __ret;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.IO.Stream @output = (System.IO.Stream)typeof(System.IO.Stream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new System.IO.BinaryWriter(@output);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
