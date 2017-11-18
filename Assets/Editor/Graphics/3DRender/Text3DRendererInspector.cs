//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Graphics.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Text3DRenderer), true)]
    public class Text3DRendererInspector : UnityEditor.Editor
    {
        private Text3DRenderer     mTarget;

        private SerializedProperty mTextLayoutProp;
        private SerializedProperty mTextProp;
        private SerializedProperty mExtentsProp;
        private SerializedProperty mFontDataProp;
        private SerializedProperty mFontColorProp;

        private SerializedProperty mTextMatProp;

        void OnEnable()
        {
            mTarget = this.target as Text3DRenderer;

            mTextLayoutProp = this.serializedObject.FindProperty("TextLayout");
            mTextProp = this.mTextLayoutProp.FindPropertyRelative("Text");
            mExtentsProp = this.mTextLayoutProp.FindPropertyRelative("Extents");
            mFontDataProp = this.mTextLayoutProp.FindPropertyRelative("FontData");
            mFontColorProp = this.mTextLayoutProp.FindPropertyRelative("FontColor");
            mTextMatProp = this.serializedObject.FindProperty("Mat");
        }

        void OnDestroy()
        {
            mTarget = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            using (var space = new EditorGUILayout.VerticalScope())
            {
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mTextProp);
                EditorGUILayout.PropertyField(this.mExtentsProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildText();
                    return;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mFontDataProp);
                EditorGUILayout.PropertyField(this.mFontColorProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildText();
                    return;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mTextMatProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildMaterial();
                    return;
                }

                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

