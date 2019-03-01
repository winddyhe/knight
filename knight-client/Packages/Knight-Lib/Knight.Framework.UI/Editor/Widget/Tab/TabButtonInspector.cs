using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TabButton), true)]
    public class TabButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
