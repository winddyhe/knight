using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEditor;
using Knight.Core;

namespace Game.Editor
{
    public class ViewModelCodeGeneratorTool
    {
        [MenuItem("LogicTools/ViewModel/Generate Game Config ViewModels")]
        public static void GenerateGameConfigViewModels()
        {
            var rGeneratorPath = "Assets/Game.Hotfix/Generate/ConfigViewModel/";
            var rGameConfigType = typeof(GameConfig);

            var rAllFieldInfos = rGameConfigType.GetFields();
            foreach (var rFieldInfo in rAllFieldInfos)
            {
                var rFieldName = rFieldInfo.Name;
                var rFieldType = rFieldInfo.FieldType;
                var rTableFiledInfo = rFieldType.GetField("Table");
                var rConfigFieldType = rTableFiledInfo.FieldType.GetGenericArguments()[1];

                var rFilePath = PathTool.Combine(rGeneratorPath, rConfigFieldType.FullName + ".ViewModel.Generate.cs");
                var rCodeGenerator = new GameConfigViewModel_CodeGenerator(rFilePath, rConfigFieldType, rFieldName);
                rCodeGenerator.CodeGenerate();
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("LogicTools/ViewModel/Generate Game NetProto ViewModels")]
        public static void GenerateGameNetProtoViewModels()
        {
            var rGeneratorPath = "Assets/Game.Hotfix/Generate/ProtoViewModel/";
            var rGameConfigAssembly = typeof(GameConfig).Assembly;
            
            var rGameNetProtoTypes = new List<Type>();
            foreach (var rType in rGameConfigAssembly.GetTypes())
            {
                if (typeof(IMessage).IsAssignableFrom(rType))
                {
                    rGameNetProtoTypes.Add(rType);
                }
            }

            foreach (var rGameNetProtoType in rGameNetProtoTypes)
            {
                var rGameNetProtoName = rGameNetProtoType.Name;
                var rGameNetProtoFullName = rGameNetProtoType.FullName;
                var rGameNetProtoNamespace = rGameNetProtoType.Namespace;

                var rFilePath = PathTool.Combine(rGeneratorPath, rGameNetProtoFullName + ".ViewModel.Generate.cs");
                var rCodeGenerator = new GameNetProtoViewModel_CodeGenerator(rFilePath, rGameNetProtoNamespace, rGameNetProtoType);
                rCodeGenerator.CodeGenerate();
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("LogicTools/ViewModel/Generate All ViewModels")]
        public static void GenerateAllViewModels()
        {
            GenerateGameConfigViewModels();
            GenerateGameNetProtoViewModels();
        }
    }
}
