using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Hotfix.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviourContainer), true)]
    public class MonoBehaviourContainerInspector : UnityEditor.Editor
    {
        public static MonoBehaviourContainerInspector   Instance;

        private SerializedProperty                      mHotfixName;
        private SerializedProperty                      mObjects;

        void OnEnable()
        {
            Instance = this;

            this.mHotfixName = this.serializedObject.FindProperty("mHotfixName");
            this.mObjects = this.serializedObject.FindProperty("mObjects");
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public override void OnInspectorGUI()
        {
            if (this.mHotfixName == null) return;

            using (var space = new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(this.mHotfixName, new GUIContent("Hotfix Class Name: "));

                EditorGUILayout.LabelField("Objects: ");
                for (int i = 0; i < this.mObjects.arraySize; i++)
                {
                    using (var space1 = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label(i.ToString()+": ", GUILayout.Width(15));

                        var rElementProperty = this.mObjects.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(rElementProperty, new GUIContent(""));

                        GameObject rElementGo = rElementProperty.objectReferenceValue as GameObject;
                        

                        if (GUILayout.Button("Del", GUILayout.Width(30)))
                        {
                            this.mObjects.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }

                if (GUILayout.Button("Add"))
                {
                    this.mObjects.InsertArrayElementAtIndex(0);
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
