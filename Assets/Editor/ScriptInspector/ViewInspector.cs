using System;
using System.Collections.Generic;
using Framework.Hotfix.Editor;
using UnityEditor;

namespace Framework.WindUI.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(View), true)]
    public class ViewInspector : HotfixMBContainerInspector
    {
        public override void OnInspectorGUI()
        {
            this.DrawBaseInspectorGUI();
            base.OnInspectorGUI();
        }
    }
}
