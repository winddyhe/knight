using Knight.Framework.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knight.Framework.Tweening.Editor
{
    [CustomEditor(typeof(CanvasAlphaTweening), true)]
    public class CanvasAlphaTweeningInspector : TweeningAnimationInspector
    {
        private void OnEnable()
        {
            this.mTweening = this.target as CanvasAlphaTweening;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            using (var rVerticalScope = new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Start"),
                    new GUIContent("Start"));

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("End"),
                    new GUIContent("End"));
            }
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
