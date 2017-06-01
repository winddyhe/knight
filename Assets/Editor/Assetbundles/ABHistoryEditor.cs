//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.AssetBundles
{
    public class ABHistoryEditor : EditorWindow
    {
        [MenuItem("Tools/AssetBundle/AssetBundle History")]
        private static void Init()
        {
            var rABHistoryEditorWindow = EditorWindow.GetWindow<ABHistoryEditor>();
            rABHistoryEditorWindow.Show();
        }
    }
}
