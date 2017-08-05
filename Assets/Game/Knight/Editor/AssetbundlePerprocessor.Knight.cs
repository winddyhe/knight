//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.Editor;
using UnityEditor;
using UnityEditor.AssetBundles;
using System.Reflection;
using System.IO;

namespace Game.Knight.Editor
{
    public class ABEntryProcessor_GameConfig : ABEntryProcessor
    {
        public string HotfixDllPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";

        public override void PreprocessAssets()
        {
            byte[] rBytes = File.ReadAllBytes(this.HotfixDllPath);
            Assembly rHotfixAssembly = Assembly.Load(rBytes);

            var rGameConfigType = rHotfixAssembly.GetType("Game.Knight.GameConfig");
            rGameConfigType.InvokeMember("Load_Local", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { this.Entry.abOriginalResPath });
            rHotfixAssembly = null;

            AssetDatabase.Refresh();
        }
    }

    public class ABEntryProcessor_SkillConfig : ABEntryProcessor
    {
        public string HotfixDllPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";

        public override void PreprocessAssets()
        {
            byte[] rBytes = File.ReadAllBytes(this.HotfixDllPath);
            Assembly rHotfixAssembly = Assembly.Load(rBytes);

            var rGameConfigType = rHotfixAssembly.GetType("Game.Knight.GPCSkillConfig");
            rGameConfigType.InvokeMember("Load_Local", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { this.Entry.abOriginalResPath });
            rHotfixAssembly = null;

            AssetDatabase.Refresh();
        }
    }
}