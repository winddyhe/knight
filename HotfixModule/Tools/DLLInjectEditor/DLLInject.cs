using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System;

namespace Core.Editor
{
    /// <summary>
    /// 给DLL静态注入Delegate供外部调用
    /// </summary>
    public class DLLInject
    {
        private static TypeReference    mObjType = null;

        public static TypeDefinition GetDelegateDefinition(string rDelegateAssemblyPath, string rClassName, string rActionName)
        {
            AssemblyDefinition rAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = false };
                rAssembly = AssemblyDefinition.ReadAssembly(rDelegateAssemblyPath, readerParameters);

                var rHotfixDelegateAttributeType = rAssembly.MainModule.Types.Single(t => t.FullName == "DelegateHelper.DLLInjectDelegateAttribute");
                var rHotfixDelegateGenType = (from type in rAssembly.MainModule.Types
                                              where type.CustomAttributes.Any(ca => ca.AttributeType == rHotfixDelegateAttributeType)
                                              select type)
                                              .Single(t => t.Name == rActionName);
                return rHotfixDelegateGenType;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        public static void Inject(string rInjectAssemblyPath, string rClassName, string rMethodName, TypeDefinition rDelegateType)
        {
            AssemblyDefinition rAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = false }; 
                rAssembly = AssemblyDefinition.ReadAssembly(rInjectAssemblyPath, readerParameters);

                // 添加一个类型标记用来限制是否重复注入
                string rDLLInjectFlag = string.Format("__DLL_INJECT_{0}_{0}_GEN_FLAG__", rClassName, rMethodName);
                if (rAssembly.MainModule.Types.Any(t => t.Name == rDLLInjectFlag))
                {
                    Debug.Log(rDLLInjectFlag + "had injected!");
                    return;
                }
                rAssembly.MainModule.Types.Add(new TypeDefinition("__DLL_INJECT", rDLLInjectFlag, Mono.Cecil.TypeAttributes.Class, mObjType));

                // 找到要注入的那个Type
                var rNeedInjectType = rAssembly.MainModule.Types.Single(t => t.FullName == rClassName);

                Debug.LogError(rNeedInjectType);

                InjectType(rAssembly, rNeedInjectType, rMethodName, rDelegateType);

                // 重新写入Assembly
                //var rWriterParameters = new WriterParameters { WriteSymbols = false };
                //rAssembly.Write(rInjectAssemblyPath, rWriterParameters);
                //Debug.Log("hotfix inject finish!");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
        }

        private static void InjectType(AssemblyDefinition rAssembly, TypeDefinition rNeedInjectType, string rMethodName, TypeDefinition rDelegateType)
        {
            if (rNeedInjectType == null) return;

            MethodDefinition rNeedInjectMethod = rNeedInjectType.Methods.Single(t => t.Name == rMethodName);
            if (rNeedInjectType == null)
            {
                Debug.LogError("Can not find Method " + rMethodName);
                return;
            }

            InjectMethod(rAssembly, rNeedInjectMethod, rDelegateType);
        }

        private static void InjectMethod(AssemblyDefinition rAssembly, MethodDefinition rNeedInjectMethod, TypeDefinition rDelegateType)
        {
            if (rDelegateType == null) return;

            var rDelegateInvokeMethod = rDelegateType.Methods.Single(m => m.Name == "Invoke");
            if (rDelegateInvokeMethod == null) return;

            var rClassType = rNeedInjectMethod.DeclaringType;
            var rDelegateName = "__" + rNeedInjectMethod.Name + "_delegate__";

            FieldDefinition rFieldDefinition = new FieldDefinition(rDelegateName, Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public, rDelegateType);
            rClassType.Fields.Add(rFieldDefinition);
            
            var rFieldReference = rFieldDefinition.Resolve();
            var rInsertPoint = rNeedInjectMethod.Body.Instructions[0];
            var rProcessor = rNeedInjectMethod.Body.GetILProcessor();

            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldsfld, rFieldReference));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Brfalse, rInsertPoint));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldsfld, rFieldReference));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Call, rDelegateInvokeMethod));
        }
    }
}