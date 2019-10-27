//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.IO;
using Knight.Framework.AssetBundles.Editor;

namespace Game.Editor
{
    public class ABEntryProcessor_GameConfig : ABEntryProcessor
    {
        public string HotfixDllPath = "Assets/Game/GameAsset/Hotfix/Libs/KnightHotfix.bytes";

        public override void PreprocessAssets()
        {
            byte[] rBytes = File.ReadAllBytes(this.HotfixDllPath);
            Assembly rHotfixAssembly = Assembly.Load(rBytes);

            var rGameConfigType = rHotfixAssembly.GetType("Game.GameConfig");
            rGameConfigType.InvokeMember("Load_Local", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { this.Entry.abOriginalResPath });
            rHotfixAssembly = null;

            AssetDatabase.Refresh();
        }
    }

    public class ABEntryProcessor_SkillConfig : ABEntryProcessor
    {
        public string HotfixDllPath = "Assets/Game/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";

        public override void PreprocessAssets()
        {
            byte[] rBytes = File.ReadAllBytes(this.HotfixDllPath);
            Assembly rHotfixAssembly = Assembly.Load(rBytes);

            var rGameConfigType = rHotfixAssembly.GetType("Game.GPCSkillConfig");
            rGameConfigType.InvokeMember("Load_Local", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { this.Entry.abOriginalResPath });
            rHotfixAssembly = null;

            AssetDatabase.Refresh();
        }
    }
}