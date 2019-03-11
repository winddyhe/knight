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
    unsafe class UnityFx_Async_AsyncExtensions_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityFx.Async.AsyncExtensions);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(Knight.Core.AssetLoaderRequest)};
            if (genericMethods.TryGetValue("GetAwaiter", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(UnityFx.Async.CompilerServices.AsyncAwaiter<Knight.Core.AssetLoaderRequest>), typeof(UnityFx.Async.IAsyncOperation<Knight.Core.AssetLoaderRequest>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetAwaiter_0);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(Knight.Core.WaitAsync.WaitForSecondsRequest)};
            if (genericMethods.TryGetValue("GetAwaiter", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(UnityFx.Async.CompilerServices.AsyncAwaiter<Knight.Core.WaitAsync.WaitForSecondsRequest>), typeof(UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForSecondsRequest>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetAwaiter_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(Knight.Core.WaitAsync.WaitForEndOfFrameRequest)};
            if (genericMethods.TryGetValue("GetAwaiter", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(UnityFx.Async.CompilerServices.AsyncAwaiter<Knight.Core.WaitAsync.WaitForEndOfFrameRequest>), typeof(UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForEndOfFrameRequest>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetAwaiter_2);

                        break;
                    }
                }
            }


        }


        static StackObject* GetAwaiter_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityFx.Async.IAsyncOperation<Knight.Core.AssetLoaderRequest> @op = (UnityFx.Async.IAsyncOperation<Knight.Core.AssetLoaderRequest>)typeof(UnityFx.Async.IAsyncOperation<Knight.Core.AssetLoaderRequest>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityFx.Async.AsyncExtensions.GetAwaiter<Knight.Core.AssetLoaderRequest>(@op);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetAwaiter_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForSecondsRequest> @op = (UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForSecondsRequest>)typeof(UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForSecondsRequest>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityFx.Async.AsyncExtensions.GetAwaiter<Knight.Core.WaitAsync.WaitForSecondsRequest>(@op);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetAwaiter_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForEndOfFrameRequest> @op = (UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForEndOfFrameRequest>)typeof(UnityFx.Async.IAsyncOperation<Knight.Core.WaitAsync.WaitForEndOfFrameRequest>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityFx.Async.AsyncExtensions.GetAwaiter<Knight.Core.WaitAsync.WaitForEndOfFrameRequest>(@op);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
