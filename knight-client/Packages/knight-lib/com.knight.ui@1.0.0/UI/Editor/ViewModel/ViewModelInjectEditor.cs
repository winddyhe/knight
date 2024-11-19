using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Cecil;
using System.Linq;
using System;
using Mono.Cecil.Cil;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

namespace Knight.Framework.UI.Editor
{
    class ViewModelIbject_CustomBuildProcessor : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder { get { return 0; } }
        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            Debug.Log("ViewModelIbject_CustomBuildProcessor.OnPostBuildPlayerScriptDLLs for target " + report.summary.platform + " at path " + report.summary.outputPath);
            ViewModelInjectEditor.InjectByPath("Temp/StagingArea/Data/Managed/Game.Hotfix.dll");
        }
    }

    public class ViewModelInjectEditor
    {
        private static string mHotfixDLLPath = "Library/ScriptAssemblies/Game.Hotfix.dll";
        private static string mUIFrameworkDLLPath = "Library/ScriptAssemblies/Knight.UI.dll";
        
        private static TypeReference mObjType = null;

        [MenuItem("Tools/ViewModel/ViewModel Injector")]
        public static void Inject()
        {
            InjectByPath(mHotfixDLLPath);
        }

        public static void InjectByPath(string rHotfixDLLPath)
        {
            AssemblyDefinition rHotfixAssembly = null;
            AssemblyDefinition rUIFrameworkAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = true, ReadWrite = true };
                rUIFrameworkAssembly = AssemblyDefinition.ReadAssembly(mUIFrameworkDLLPath, readerParameters);
                rHotfixAssembly = AssemblyDefinition.ReadAssembly(rHotfixDLLPath, readerParameters);

                var rHotfixTemplateType = rHotfixAssembly.MainModule.GetType("Game.HotfixTemplate");
                if (rHotfixTemplateType == null)
                {
                    rUIFrameworkAssembly.Dispose();
                    rHotfixAssembly.Dispose();
                    return;
                }
                var rField = rHotfixTemplateType.Fields.SingleOrDefault(f => f.Name.Equals("__Is_ViewModel_Injected__"));
                if (rField != null)
                {
                    rUIFrameworkAssembly.Dispose();
                    rHotfixAssembly.Dispose();
                    return;
                }
                rField = new FieldDefinition("__Is_ViewModel_Injected__", FieldAttributes.Static | FieldAttributes.Public, GetBoolValueTypeReference(rHotfixAssembly));
                rHotfixTemplateType.Fields.Add(rField);

                var rViewModelDataBindingTypes = rHotfixAssembly.MainModule
                    .Types.Where(rType => rType != null &&
                                          rType.BaseType != null &&
                                          rType.BaseType.FullName.Equals("Knight.Framework.UI.ViewModel") &&
                                          rType.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Framework.UI.DataBindingAttribute")));

                var rViewModelTypeList = new List<TypeDefinition>(rViewModelDataBindingTypes);
                foreach (var rType in rViewModelTypeList)
                {
                    var rNeedInjectProps = rType.Properties.Where(rProp => rProp != null &&
                                                                           rProp.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Framework.UI.DataBindingAttribute")));

                    var rNeedInjectPropList = new List<PropertyDefinition>(rNeedInjectProps);
                    foreach (var rProp in rNeedInjectPropList)
                    {
                        InjectType(rUIFrameworkAssembly, rHotfixAssembly, rType, rProp.Name);
                    }
                }
                var rWriteParameters = new WriterParameters { WriteSymbols = true };
                rHotfixAssembly.Write(rWriteParameters);
                rUIFrameworkAssembly.Dispose();
                rHotfixAssembly.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                rUIFrameworkAssembly?.Dispose();
                rHotfixAssembly?.Dispose();
            }
            Debug.Log("ViewModel inject success!!!");
        }

        private static void InjectType(AssemblyDefinition rUIFrameworkAssembly, AssemblyDefinition rHotfixAssembly, TypeDefinition rNeedInjectType, string rPropertyName)
        {
            if (rNeedInjectType == null) return;

            PropertyDefinition rNeedInjectProperty = rNeedInjectType.Properties.Single(t => t.Name == rPropertyName);
            if (rNeedInjectProperty == null)
            {
                Console.WriteLine("Can not find property " + rPropertyName);
                return;
            }

            var rBaseType = rUIFrameworkAssembly.MainModule.GetType(rNeedInjectType.BaseType.Namespace, rNeedInjectType.BaseType.Name);
            var rPropChangedMethod = rBaseType.Methods.SingleOrDefault(t => t.Name == "PropChanged");

            // 通过Inject Property的名字找到对应的Set方法
            var rNeedInjectPropertySetMethod = rNeedInjectType.Methods.SingleOrDefault(t => t.Name == "set_" + rPropertyName);
            var rPropChangedMethodRef = rNeedInjectPropertySetMethod.Module.ImportReference(rPropChangedMethod);

            InjectProperty(rHotfixAssembly, rPropChangedMethodRef, rNeedInjectPropertySetMethod, rPropertyName);
        }

        private static void InjectProperty(AssemblyDefinition rAssembly, MethodReference rPropChangedMethod, MethodDefinition rNeedInjectPropertySetMethod, string rPropertyName)
        {
            if (rPropChangedMethod == null) return;
            if (rNeedInjectPropertySetMethod == null) return;

            var rInsertPoint = rNeedInjectPropertySetMethod.Body.Instructions[rNeedInjectPropertySetMethod.Body.Instructions.Count - 1];
            var rProcessor = rNeedInjectPropertySetMethod.Body.GetILProcessor();

            var rGenericInstanceMethod = new GenericInstanceMethod(rPropChangedMethod);
            rGenericInstanceMethod.GenericArguments.Add(rNeedInjectPropertySetMethod.Parameters[0].ParameterType);

            //IL_0001: ldarg.0
            //IL_0002: ldstr "Name"
            //IL_0007: ldarg.1
            //IL_0008: call instance void [Knight.UI]Knight.Framework.UI.ViewModel::PropChanged<string>(string, !!0)
            //IL_000d: nop
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldarg_0));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldstr, rPropertyName));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldarg_1));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Call, rGenericInstanceMethod));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Nop));
        }

        private static TypeReference GetBoolValueTypeReference(AssemblyDefinition rAssembly)
        {
            var rHotfixTemplateType = rAssembly.MainModule.Types.SingleOrDefault(
                                        rType => rType != null &&
                                        rType.BaseType != null &&
                                        rType.FullName.Equals("Game.HotfixTemplate"));

            if (rHotfixTemplateType != null)
            {
                var rBoolValueField = rHotfixTemplateType.Fields.SingleOrDefault(
                                        rField => rField.Name.Equals("__Bool_Value_Template__"));
                return rBoolValueField.FieldType;
            }
            return null;
        }
    }
}