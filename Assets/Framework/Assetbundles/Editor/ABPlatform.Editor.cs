//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AssetBundles;

namespace UnityEditor.AssetBundles
{
    public class ABPlatformEditor
    {
        private const string    mSelectDevelopeModeMenuPath         = "Tools/Develope Mode";

        private const string    mSelectSimulateModeMenuPath_Scene   = "Tools/Simulate Mode/Scene";
        private const string    mSelectSimulateModeMenuPath_Avatar  = "Tools/Simulate Mode/Avatar";
        private const string    mSelectSimulateModeMenuPath_Config  = "Tools/Simulate Mode/Config";
        private const string    mSelectSimulateModeMenuPath_GUI     = "Tools/Simulate Mode/GUI";
        private const string    mSelectSimulateModeMenuPath_Script  = "Tools/Simulate Mode/Script";

        [MenuItem(mSelectDevelopeModeMenuPath, priority = 1000)]
        public static void SelectDevelopeMode_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectDevelopeModeMenuPath);
            EditorPrefs.SetBool(ABPlatform.IsDevelopeModeKey, !bSelected);
            Menu.SetChecked(mSelectDevelopeModeMenuPath, !bSelected);
        }

        [MenuItem(mSelectDevelopeModeMenuPath, true)]
        public static bool SelectDevelopeMode_Check_Menu()
        {
            Menu.SetChecked(mSelectDevelopeModeMenuPath, EditorPrefs.GetBool(ABPlatform.IsDevelopeModeKey));
            return true;
        }

        [MenuItem(mSelectSimulateModeMenuPath_Scene, priority = 1050)]
        public static void SelectSimulateMode_Scene_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath_Scene);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey_Scene, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath_Scene, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath_Scene, true)]
        public static bool SelectSimulateMode_Check_Scene_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath_Scene, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Scene));
            return true;
        }

        [MenuItem(mSelectSimulateModeMenuPath_Avatar, priority = 1050)]
        public static void SelectSimulateMode_Avatar_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath_Avatar);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey_Avatar, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath_Avatar, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath_Avatar, true)]
        public static bool SelectSimulateMode_Check_Avatar_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath_Avatar, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Avatar));
            return true;
        }

        [MenuItem(mSelectSimulateModeMenuPath_Config, priority = 1050)]
        public static void SelectSimulateMode_Config_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath_Config);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey_Config, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath_Config, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath_Config, true)]
        public static bool SelectSimulateMode_Check_Config_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath_Config, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Config));
            return true;
        }

        [MenuItem(mSelectSimulateModeMenuPath_GUI, priority = 1050)]
        public static void SelectSimulateMode_GUI_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath_GUI);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey_GUI, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath_GUI, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath_GUI, true)]
        public static bool SelectSimulateMode_Check_GUI_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath_GUI, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_GUI));
            return true;
        }

        [MenuItem(mSelectSimulateModeMenuPath_Script, priority = 1050)]
        public static void SelectSimulateMode_Script_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath_Script);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey_Script, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath_Script, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath_Script, true)]
        public static bool SelectSimulateMode_Check_Script_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath_Script, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Script));
            return true;
        }
    }
}
