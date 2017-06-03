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
        private const  string   mSelectDevelopeModeMenuPath = "Tools/Develope Mode";
        private const  string   mSelectSimulateModeMenuPath = "Tools/Simulate Mode";
        
        [MenuItem(mSelectDevelopeModeMenuPath)]
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

        [MenuItem(mSelectSimulateModeMenuPath)]
        public static void SelectSimulateMode_Menu()
        {
            bool bSelected = Menu.GetChecked(mSelectSimulateModeMenuPath);
            EditorPrefs.SetBool(ABPlatform.IsSimulateModeKey, !bSelected);
            Menu.SetChecked(mSelectSimulateModeMenuPath, !bSelected);
        }

        [MenuItem(mSelectSimulateModeMenuPath, true)]
        public static bool SelectSimulateMode_Check_Menu()
        {
            Menu.SetChecked(mSelectSimulateModeMenuPath, EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey));
            return true;
        }
    }
}
