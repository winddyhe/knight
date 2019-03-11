//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Knight.Framework.Hotfix.Editor
{
    public class HotfixManagerEditor : UnityEditor.Editor
    {
        private const string mSelectHotfixDebugModeMenuPath = "Tools/Hotfix Debug Mode";

        [MenuItem(mSelectHotfixDebugModeMenuPath, priority = 1000)]
        public static void SelectDevelopeMode_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectHotfixDebugModeMenuPath);
            EditorPrefs.SetBool(HotfixManager.IsHotfixDebugModeKey, !bSelected);
            Menu.SetChecked(mSelectHotfixDebugModeMenuPath, !bSelected);
        }

        [MenuItem(mSelectHotfixDebugModeMenuPath, true)]
        public static bool SelectDevelopeMode_Check_Menu()
        {
            Menu.SetChecked(mSelectHotfixDebugModeMenuPath, EditorPrefs.GetBool(HotfixManager.IsHotfixDebugModeKey));
            return true;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AllScriptsReloaded()
        {
            string rDLLRootPath = "Library/ScriptAssemblies/Game.Hotfix.dll";
            string rPDBRootPath = "Library/ScriptAssemblies/Game.Hotfix.pdb";
            string rNewDLLRootPath = HotfixManager.HotfixDllDir + "KnightHotfix.bytes";
            string rNewPDBRootPath = HotfixManager.HotfixDllDir + "KnightHotfix_PDB.bytes";

            File.Copy(rDLLRootPath, rNewDLLRootPath, true);
            File.Copy(rPDBRootPath, rNewPDBRootPath, true);
        }
    }
}
