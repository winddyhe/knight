//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Knight.Framework.Hotfix.Editor;
using UnityEditor;
using UnityEngine.UI;

namespace Knight.Framework.WindUI.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ViewContainer), true)]
    public class ViewInspector : HotfixMBContainerInspector
    {
        public override void OnInspectorGUI()
        {
            this.DrawBaseInspectorGUI();
            base.OnInspectorGUI();
        }
    }
}
