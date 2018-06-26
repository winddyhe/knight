using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(TextStyleSelector), true)]
    public class TextStyleSelectorInspector : Editor
    {
        private TextStyleSelector mTarget;

        private void OnEnable()
        {
            mTarget = target as TextStyleSelector;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("添加Text样式"))
            {
                TextStyleSelectorWindow rWindow = EditorWindow.GetWindow<TextStyleSelectorWindow>();
                rWindow.titleContent = new GUIContent("选择Text样式");
                rWindow.minSize = new Vector2(300, 200);
                rWindow.Show();
            }
        }
    }
}