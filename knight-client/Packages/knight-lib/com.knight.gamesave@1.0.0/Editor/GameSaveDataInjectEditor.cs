using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using Mono.Cecil;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using Mono.Cecil.Cil;

namespace Knight.Framework.GameSave.Editor
{
    class GameSaveDataInject_CustomBuildProcessor : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder { get { return 1; } }
        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            Debug.Log("GameSaveDataInject_CustomBuildProcessor.OnPostBuildPlayerScriptDLLs for target " + report.summary.platform + " at path " + report.summary.outputPath);
            GameSaveDataInjectEditor.InjectByPath("Temp/StagingArea/Data/Managed/Game.Hotfix.dll");
        }
    }

    public class GameSaveDataInjectEditor
    {
        private static string mHotfixDLLPath = "Library/ScriptAssemblies/Game.Hotfix.dll";
        private static string mGameSaveFrameworkDLLPath = "Library/ScriptAssemblies/Knight.GameSave.dll";

        private static TypeReference mObjType = null;

        [MenuItem("Tools/GameSave/GameSaveData Injector")]
        public static void Inject()
        {
            InjectByPath(mHotfixDLLPath);
        }

        public static void InjectByPath(string rHotfixDLLPath)
        {
            AssemblyDefinition rHotfixAssembly = null;
            AssemblyDefinition rGameSaveFrameworkAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = true, ReadWrite = true };
                rGameSaveFrameworkAssembly = AssemblyDefinition.ReadAssembly(mGameSaveFrameworkDLLPath, readerParameters);
                rHotfixAssembly = AssemblyDefinition.ReadAssembly(rHotfixDLLPath, readerParameters);

                var rHotfixTemplateType = rHotfixAssembly.MainModule.GetType("Game.HotfixTemplate");
                if (rHotfixTemplateType == null)
                {
                    rGameSaveFrameworkAssembly.Dispose();
                    rHotfixAssembly.Dispose();
                    return;
                }
                var rField = rHotfixTemplateType.Fields.SingleOrDefault(f => f.Name.Equals("__Is_GameSave_Injected__"));
                if (rField != null)
                {
                    rGameSaveFrameworkAssembly.Dispose();
                    rHotfixAssembly.Dispose();
                    return;
                }
                rField = new FieldDefinition("__Is_GameSave_Injected__", FieldAttributes.Static | FieldAttributes.Public, GetBoolValueTypeReference(rHotfixAssembly));
                rHotfixTemplateType.Fields.Add(rField);

                var rGameSaveDataTypes = rHotfixAssembly.MainModule
                    .Types.Where(rType => rType != null &&
                                          rType.BaseType != null &&
                                          rType.BaseType.FullName.Equals("Knight.Framework.GameSave.GameSaveData") &&
                                          rType.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Framework.GameSave.GameSaveAttribute")));

                var rGameSaveTypeList = new List<TypeDefinition>(rGameSaveDataTypes);
                foreach (var rType in rGameSaveTypeList)
                {
                    var rNeedInjectProps = rType.Properties.Where(rProp => rProp != null &&
                                                                           rProp.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("Nino.Core.NinoMemberAttribute")));

                    var rNeedInjectPropList = new List<PropertyDefinition>(rNeedInjectProps);
                    foreach (var rProp in rNeedInjectPropList)
                    {
                        InjectType(rGameSaveFrameworkAssembly, rHotfixAssembly, rType, rProp.Name);
                    }
                }
                var rWriteParameters = new WriterParameters { WriteSymbols = true };
                rHotfixAssembly.Write(rWriteParameters);
                rGameSaveFrameworkAssembly.Dispose();
                rHotfixAssembly.Dispose();
                Debug.Log("GameSaveData inject success!!!");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                rGameSaveFrameworkAssembly?.Dispose();
                rHotfixAssembly?.Dispose();
            }
        }

        private static void InjectType(AssemblyDefinition rGameSaveFrameworkAssembly, AssemblyDefinition rHotfixAssembly, TypeDefinition rNeedInjectType, string rPropertyName)
        {
            if (rNeedInjectType == null) return;

            PropertyDefinition rNeedInjectProperty = rNeedInjectType.Properties.Single(t => t.Name == rPropertyName);
            if (rNeedInjectProperty == null)
            {
                Console.WriteLine("Can not find property " + rPropertyName);
                return;
            }

            var rBaseType = rGameSaveFrameworkAssembly.MainModule.GetType(rNeedInjectType.BaseType.Namespace, rNeedInjectType.BaseType.Name);
            var rMarkDirtyMethod = rBaseType.Methods.SingleOrDefault(t => t.Name == "MarkDirty");

            // 通过Inject Property的名字找到对应的Set方法
            var rNeedInjectPropertySetMethod = rNeedInjectType.Methods.SingleOrDefault(t => t.Name == "set_" + rPropertyName);
            var rMarkDirtyMethodRef = rNeedInjectPropertySetMethod.Module.ImportReference(rMarkDirtyMethod);

            InjectProperty(rHotfixAssembly, rMarkDirtyMethodRef, rNeedInjectPropertySetMethod, rPropertyName);
        }

        private static void InjectProperty(AssemblyDefinition rAssembly, MethodReference rMarkDirtyMethod, MethodDefinition rNeedInjectPropertySetMethod, string rPropertyName)
        {
            if (rMarkDirtyMethod == null) return;
            if (rNeedInjectPropertySetMethod == null) return;

            var rInsertPoint = rNeedInjectPropertySetMethod.Body.Instructions[rNeedInjectPropertySetMethod.Body.Instructions.Count - 1];
            var rProcessor = rNeedInjectPropertySetMethod.Body.GetILProcessor();

            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Ldarg_0));
            rProcessor.InsertBefore(rInsertPoint, rProcessor.Create(OpCodes.Call, rMarkDirtyMethod));
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
