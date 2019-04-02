using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Knight.Framework.Tweening;
using System;

namespace Knight.Framework.Tweening.Editor
{
    [CustomEditor(typeof(TweeningAnimation), true)]
    public class TweeningAnimationInspector : UnityEditor.Editor
    {
        protected TweeningAnimation mTweening;

        private void OnEnable()
        {
            this.mTweening = this.target as TweeningAnimation;
        }

        public override void OnInspectorGUI()
        {
            this.DrawCommonProperties();
            this.serializedObject.ApplyModifiedProperties();
        }

        protected void DrawCommonProperties()
        {
            using (var rHorizontalScope = new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUIUtility.labelWidth = 65;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsIgnoreTimeScale"),
                    new GUIContent("TimeScale"), GUILayout.Width(75));

                EditorGUIUtility.labelWidth = 35;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsUseFixedUpdate"),
                    new GUIContent("Fixed"), GUILayout.Width(45));
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsAutoExecute"),
                    new GUIContent("Auto"), GUILayout.Width(45));

                if (GUILayout.Button("Play"))
                    mTweening.Play();

                if (GUILayout.Button("Stop"))
                    mTweening.Stop();
            }

            EditorGUIUtility.labelWidth = 120;

            using (var rVerticalScope = new EditorGUILayout.VerticalScope("box"))
            {
                GUILayout.Space(7);
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("TimeCurve"),
                    GUILayout.Width(190), GUILayout.Height(60));
                GUILayout.Space(7);

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsLoop"));
                if (serializedObject.FindProperty("IsLoop").boolValue)
                {
                    EditorGUIUtility.labelWidth = 106;
                    using (var rHorizontalScope = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(15);
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LoopType"), GUILayout.Width(170));
                    }
                    using (var rHorizontalScope = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(15);
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LoopCount"), GUILayout.Width(170));
                    }
                    EditorGUIUtility.labelWidth = 120;
                }

                using (var rHorizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Duration"), GUILayout.Width(170));
                    GUILayout.Label("seconds");
                }

                using (var rHorizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Delay"), GUILayout.Width(170));
                    GUILayout.Label("seconds");
                }
            }
        }
    }
}