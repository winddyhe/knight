using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CustomEditor(typeof(WText), true)]
    public class WTextInspector : TextEditor
    {
        private SerializedProperty  mIsUseMultiLang;
        private SerializedProperty  mMultiLangID;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.mIsUseMultiLang = this.serializedObject.FindProperty("mIsUseMultiLang");
            this.mMultiLangID = this.serializedObject.FindProperty("mMultiLangID");

            EditorGUILayout.PropertyField(this.mIsUseMultiLang);
            if (this.mIsUseMultiLang.boolValue)
            {
                EditorGUILayout.PropertyField(this.mMultiLangID);
            }
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
