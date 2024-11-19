using UnityEngine;
using UnityEditor;
using System.IO;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor;
using Knight.Core;

namespace Knight.Framework.Hotfix.Editor
{
    public class HybridCLREditor
    {
        [MenuItem("Tools/HybridCLR/Compile DLL Active Build Target", priority = 100)]
        public static void CompilerDLLActiveBuildTarget()
        {
            // 编译DLL
            CompileDllCommand.CompileDllActiveBuildTarget();

            var rHotfixDir = "Assets/GameAssets/Hotfix/Libs/";
            var rHybridCLRDir = "HybridCLRData/HotUpdateDlls/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";

            // 注入ViewModel到Game.Hotfix.dll中
            ViewModelInjectInHotfixDLL(rHybridCLRDir + "Game.Hotfix.dll");
            // 注入GameSave到Game.Hotfix.dll中
            GameSaveInjectInHotfixDLL(rHybridCLRDir + "Game.Hotfix.dll");

            // 复制路径到我们的热更新资源目录
            var rHotfixDlls = SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;
            foreach (var rHotfixDll in rHotfixDlls)
            {
                var rDLLPath = rHybridCLRDir + rHotfixDll + ".dll";
                var rDLLBytesPath = rHotfixDir + rHotfixDll + ".bytes";
                File.Copy(rDLLPath, rDLLBytesPath, true);

                var rPDBPath = rHybridCLRDir + rHotfixDll + ".pdb";
                var rPDBBytesPath = rHotfixDir + rHotfixDll + ".PDB.bytes";
                File.Copy(rPDBPath, rPDBBytesPath, true);
            }
            Debug.Log("compile dll copy finish!!!");
        }

        public static void InstallFromRepo()
        {
            var rInstallerController = new InstallerController();
            rInstallerController.InstallDefaultHybridCLR();
        }

        [MenuItem("Tools/HybridCLR/HybridCLR Prebuild Project", priority = 100)]
        public static void PreBuildProject()
        {
            // Generate All
            PrebuildCommand.GenerateAll();
            // 编译热更新DLL
            HybridCLREditor.CompilerDLLActiveBuildTarget();
            // 复制元数据dll到Resources中
            CopyAOTAssembliesToResources();

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/HybridCLR/Copy AOT Assemblies To Resources")]
        public static void CopyAOTAssembliesToResources()
        {
            var rTarget = EditorUserBuildSettings.activeBuildTarget;
            string rAotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(rTarget);
            string rAotAssembliesDstDir = Application.dataPath + "/GameAssets/Hotfix/AOTAsms";

            foreach (var rDLL in SettingsUtil.AOTAssemblyNames)
            {
                string rSrcDllPath = $"{rAotAssembliesSrcDir}/{rDLL}.dll";
                if (!File.Exists(rSrcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{rSrcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string rDllBytesPath = $"{rAotAssembliesDstDir}/{rDLL}.dll.bytes";
                File.Copy(rSrcDllPath, rDllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {rSrcDllPath} -> {rDllBytesPath}");
            }
        }

        public static void SetHybridCLREnabled(bool bIsEnabled)
        {
            HybridCLR.Editor.SettingsUtil.Enable = bIsEnabled;
        }

        private static void ViewModelInjectInHotfixDLL(string rHotfixDLLPath)
        {
            var rUIFrameworkAssembly = System.Reflection.Assembly.Load("Knight.UI.Editor");
            if (rUIFrameworkAssembly == null)
            {
                Debug.LogError("Cannot find dll: Knight.UI.Editor.");
                return;
            }
            var rViewModelInjectEditorType = rUIFrameworkAssembly.GetType("Knight.Framework.UI.Editor.ViewModelInjectEditor");
            if (rViewModelInjectEditorType == null)
            {
                Debug.LogError("Cannot find class: Knight.Framework.UI.Editor.ViewModelInjectEditor.");
                return;
            }
            rViewModelInjectEditorType.InvokeMember("InjectByPath", ReflectTool.flags_method_static, null, null, new object[] { rHotfixDLLPath });
        }

        private static void GameSaveInjectInHotfixDLL(string rHotfixDLLPath)
        {
            var rGameSaveFrameworkAssembly = System.Reflection.Assembly.Load("Knight.GameSave.Editor");
            if (rGameSaveFrameworkAssembly == null)
            {
                Debug.LogError("Cannot find dll: Knight.GameSave.Editor.");
                return;
            }
            var rGameSaveInjectEditorType = rGameSaveFrameworkAssembly.GetType("Knight.Framework.GameSave.Editor.GameSaveDataInjectEditor");
            if (rGameSaveInjectEditorType == null)
            {
                Debug.LogError("Cannot find class: Knight.Framework.GameSave.Editor.GameSaveDataInjectEditor.");
                return;
            }
            rGameSaveInjectEditorType.InvokeMember("InjectByPath", ReflectTool.flags_method_static, null, null, new object[] { rHotfixDLLPath });
        }
    }
}

