using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Cecil;
using System.Linq;
using System;
using Mono.Cecil.Cil;

namespace Knight.Core.Editor
{
    public class ViewModelInjectEditor
    {
        private static string           mDLLPath    = "Library/ScriptAssemblies/Game.Hotfix.dll";
        private static string           mDLLNewPath = "Library/ScriptAssemblies/Game.Hotfix.Inject.dll";

        private static TypeReference    mObjType    = null;

        [MenuItem("Tools/Other/ViewModel Injector")]
        public static void Inject()
        {
            AssemblyDefinition rAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = true };
                rAssembly = AssemblyDefinition.ReadAssembly(mDLLPath, readerParameters);

                var rViewModelDataBindingTypes = rAssembly.MainModule
                    .Types.Where(rType => rType != null &&
                                          rType.BaseType != null &&
                                          rType.BaseType.FullName.Equals("Knight.Hotfix.Core.ViewModel") &&
                                          rType.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("UnityEngine.UI.DataBindingAttribute")));

                var rViewModelTypeList = new List<TypeDefinition>(rViewModelDataBindingTypes);
                foreach (var rType in rViewModelTypeList)
                {
                    var rNeedInjectProps = rType.Properties.Where(rProp => rProp != null &&
                                                                           rProp.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("UnityEngine.UI.DataBindingAttribute")));

                    var rNeedInjectPropList = new List<PropertyDefinition>(rNeedInjectProps);
                    foreach (var rProp in rNeedInjectPropList)
                    {
                        InjectType(rAssembly, rType, rProp.Name);
                    }
                }
                var rWriteParameters = new WriterParameters { WriteSymbols = true };
                rAssembly.Write(mDLLNewPath, rWriteParameters);
                rAssembly.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Debug.Log("ViewModel inject success!!!");
        }

        private static void InjectType(AssemblyDefinition rAssembly, TypeDefinition rNeedInjectType, string rPropertyName)
        {
            if (rNeedInjectType == null) return;

            PropertyDefinition rNeedInjectProperty = rNeedInjectType.Properties.Single(t => t.Name == rPropertyName);
            if (rNeedInjectProperty == null)
            {
                Console.WriteLine("Can not find property " + rPropertyName);
                return;
            }

            var rBaseType = rNeedInjectType.BaseType as TypeDefinition;
            var rPropChangedMethod = rBaseType.Methods.SingleOrDefault(t => t.Name == "PropChanged");

            // 通过Inject Property的名字找到对应的Set方法
            var rNeedInjectPropertySetMethod = rNeedInjectType.Methods.SingleOrDefault(t => t.Name == "set_" + rPropertyName);

            InjectProperty(rAssembly, rPropChangedMethod, rNeedInjectPropertySetMethod, rPropertyName);
        }

        private static void InjectProperty(AssemblyDefinition rAssembly, MethodDefinition rPropChangedMethod, MethodDefinition rNeedInjectPropertySetMethod, string rPropertyName)
        {
            if (rPropChangedMethod == null) return;
            if (rNeedInjectPropertySetMethod == null) return;

            var rInsertPoint = rNeedInjectPropertySetMethod.Body.Instructions[2];
            var rProcessor = rNeedInjectPropertySetMethod.Body.GetILProcessor();

            // 顺序是反的
            rProcessor.InsertAfter(rInsertPoint, rProcessor.Create(OpCodes.Nop));
            rProcessor.InsertAfter(rInsertPoint, rProcessor.Create(OpCodes.Call, rPropChangedMethod));
            rProcessor.InsertAfter(rInsertPoint, rProcessor.Create(OpCodes.Ldstr, rPropertyName));
            rProcessor.InsertAfter(rInsertPoint, rProcessor.Create(OpCodes.Ldarg_0));
        }
    }
}