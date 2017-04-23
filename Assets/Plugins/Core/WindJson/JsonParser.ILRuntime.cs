using ILRuntime.Runtime.Stack;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using System.Collections.Generic;
using System;

namespace Core.WindJson
{
    public partial class JsonParser
    {
        public unsafe static void RegisterILRuntimeCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain rAppdomain)
        {
            foreach (var rMethodInfo in typeof(JsonParser).GetMethods())
            {
                if (rMethodInfo.Name == "ToObject" && rMethodInfo.IsGenericMethodDefinition)
                {
                    rAppdomain.RegisterCLRMethodRedirection(rMethodInfo, IL_ToObject);
                }
            }
        }

        public unsafe static StackObject* IL_ToObject(ILIntepreter intp, StackObject* esp, List<object> mStack, CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            JsonClass rJsonNode = (JsonClass)typeof(JsonClass).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var type = method.GenericArguments[0].ReflectionType;
            var result_of_this_method = ToObject(rJsonNode, type);

            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }
    }
}
