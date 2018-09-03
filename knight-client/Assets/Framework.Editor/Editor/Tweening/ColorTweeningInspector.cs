using Knight.Framework.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorTweening), true)]
public class ColorTweeningInspector : TweeningAnimationInspector
{
    private void OnEnable()
    {
        this.mTweening = this.target as ColorTweening;
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