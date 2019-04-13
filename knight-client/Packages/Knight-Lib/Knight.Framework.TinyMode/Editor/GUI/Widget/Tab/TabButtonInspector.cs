using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TabButton), true)]
    public class TabButtonInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
