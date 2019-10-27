using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(DelayClickEvent), true)]
    public class LongClickButtonInspector : ButtonEditor
    {
        private SerializedProperty mLongClickLimitTime;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.mLongClickLimitTime = this.serializedObject.FindProperty("mLongClickLimitTime");

            EditorGUILayout.PropertyField(this.mLongClickLimitTime);
            this.serializedObject.ApplyModifiedProperties();
        }
    } 
}
