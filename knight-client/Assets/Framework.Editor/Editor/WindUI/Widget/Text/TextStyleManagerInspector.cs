using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(TextStyleManager), true)]
    public class TextStyleManagerInspector : Editor
    {
        private TextStyleManager mTarget;
        private SerializedProperty mTextStyles;

        private void OnEnable()
        {
            mTarget = this.target as TextStyleManager;
            mTextStyles = this.serializedObject.FindProperty("TextStyles");
        }

        public override void OnInspectorGUI()
        {
            var nLength = mTextStyles.arraySize;
            for (int i = 0; i < nLength; i++)
            {
                var rTextStyleProp = mTextStyles.GetArrayElementAtIndex(i);
                if (!this.DrawTextStyle(rTextStyleProp, i)) return;
            }

            if (GUILayout.Button("Add"))
            {
                mTextStyles.InsertArrayElementAtIndex(nLength);
                var rProp = mTextStyles.GetArrayElementAtIndex(nLength);
                //Default Value
                var rFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                rProp.FindPropertyRelative("Font").objectReferenceValue = rFont;
                rProp.FindPropertyRelative("FontSize").intValue = 21;
                rProp.FindPropertyRelative("OutlineDistance").vector2Value = new Vector2(1, -1);
                rProp.FindPropertyRelative("OutlineGraphicAlpha").boolValue = true;
                rProp.FindPropertyRelative("ShadowDistance").vector2Value = new Vector2(1, -1);
                rProp.FindPropertyRelative("ShadowGraphicAlpha").boolValue = true;
                //Default Color
                rProp.FindPropertyRelative("Color").colorValue = new Color(0, 0, 0, 255);
                rProp.FindPropertyRelative("TopColor").colorValue = new Color(0, 0, 0, 255);
                rProp.FindPropertyRelative("BottomColor").colorValue = new Color(0, 0, 0, 255);
                rProp.FindPropertyRelative("OutlineColor").colorValue = new Color(0, 0, 0, 255);
                rProp.FindPropertyRelative("ShadowColor").colorValue = new Color(0, 0, 0, 255);
            }
            this.serializedObject.ApplyModifiedProperties();
        }

        private bool DrawTextStyle(SerializedProperty rTextStyleProp, int nIndex)
        {
            var rIsOpenProp = rTextStyleProp.FindPropertyRelative("IsEditorOpen");
            var rIDProp = rTextStyleProp.FindPropertyRelative("ID");
            var rDescProp = rTextStyleProp.FindPropertyRelative("Description");

            var rUseGradientProp = rTextStyleProp.FindPropertyRelative("UseGradient");
            var rUseOutlineProp = rTextStyleProp.FindPropertyRelative("UseOutline");
            var rUseShadowProp = rTextStyleProp.FindPropertyRelative("UseShadow");

            rIDProp.intValue = nIndex;
            using (var rScope = new EditorGUILayout.HorizontalScope())
            {
                if (string.IsNullOrEmpty(rDescProp.stringValue))
                    rDescProp.stringValue = "填写文字样式描述";

                rIsOpenProp.boolValue = EditorGUILayout.Foldout(rIsOpenProp.boolValue, rDescProp.stringValue);
                if (GUILayout.Button("Del", GUILayout.Width(30)))
                {
                    mTextStyles.DeleteArrayElementAtIndex(nIndex);
                    this.serializedObject.ApplyModifiedProperties();
                    return false;
                }
            }

            if (rIsOpenProp.boolValue)
            {
                using (var rScope = new EditorGUILayout.VerticalScope("TextField"))
                {
                    EditorGUILayout.PropertyField(rIDProp);
                    EditorGUILayout.PropertyField(rDescProp);

                    EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("Font"));
                    EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("Color"));
                    EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("FontSize"));

                    EditorGUILayout.PropertyField(rUseGradientProp);
                    if (rUseGradientProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("TopColor"));
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("BottomColor"));
                    }

                    EditorGUILayout.PropertyField(rUseOutlineProp);
                    if (rUseOutlineProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("OutlineColor"));
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("OutlineDistance"));
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("OutlineGraphicAlpha"));
                    }

                    EditorGUILayout.PropertyField(rUseShadowProp);
                    if (rUseShadowProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("ShadowColor"));
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("ShadowDistance"));
                        EditorGUILayout.PropertyField(rTextStyleProp.FindPropertyRelative("ShadowGraphicAlpha"));
                    }
                }
            }

            EditorGUILayout.Space();
            return true;
        }
    }
}
