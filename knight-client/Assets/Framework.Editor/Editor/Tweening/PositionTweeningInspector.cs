using Knight.Framework.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionTweening), true)]
public class PositionTweeningInspector : TweeningAnimationInspector
{
    private void OnEnable()
    {
        this.mTweening = this.target as PositionTweening;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        using (var rVerticalScope = new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsLocal"),
                new GUIContent("IsLocal"));

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Start"),
                new GUIContent("Start"));

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("End"),
                new GUIContent("End"));
        }
        this.serializedObject.ApplyModifiedProperties();
    }
}