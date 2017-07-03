//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Framework.Graphics.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Image3DRenderer), true)]
    public class Image3DRendererInspector : UnityEditor.Editor
    {
        private Image3DRenderer    mTarget;

        private SerializedProperty mImageLayoutProp;
        private SerializedProperty mMaterialProp;
        private SerializedProperty mSpriteProp;

        private SerializedProperty mWidthProp;
        private SerializedProperty mHeightProp;
        private SerializedProperty mVertexColorProp;

        private SerializedProperty mTypeProp;
        private SerializedProperty mIsPreserveAspectProp;
        private SerializedProperty mIsFillCenterProp;

        private SerializedProperty mFillMethodProp;
        private SerializedProperty mFillHorizontalOriginProp;
        private SerializedProperty mFillVerticalOriginProp;
        private SerializedProperty mFillRadial90Prop;
        private SerializedProperty mFillRadial180Prop;
        private SerializedProperty mFillRadial360Prop;
        private SerializedProperty mFillAmountProp;
        private SerializedProperty mFillIsClockwiseProp;
        
        void OnEnable()
        {
            mTarget = this.target as Image3DRenderer;

            mImageLayoutProp = this.serializedObject.FindProperty("ImageLayout");
            mSpriteProp = this.serializedObject.FindProperty("Sprite");
            mMaterialProp = this.serializedObject.FindProperty("Mat");

            mWidthProp = this.mImageLayoutProp.FindPropertyRelative("Width");
            mHeightProp = this.mImageLayoutProp.FindPropertyRelative("Height");
            mVertexColorProp = this.mImageLayoutProp.FindPropertyRelative("Color");

            mTypeProp = this.mImageLayoutProp.FindPropertyRelative("ImageType");
            mIsPreserveAspectProp = this.mImageLayoutProp.FindPropertyRelative("IsPreserveAspect");
            mIsFillCenterProp = this.mImageLayoutProp.FindPropertyRelative("IsFillCenter");

            mFillMethodProp = this.mImageLayoutProp.FindPropertyRelative("ImageFillMethod");
            mFillHorizontalOriginProp = this.mImageLayoutProp.FindPropertyRelative("ImageOriginHorizontal");
            mFillVerticalOriginProp = this.mImageLayoutProp.FindPropertyRelative("ImageOriginVertical");
            mFillRadial90Prop = this.mImageLayoutProp.FindPropertyRelative("ImageOrigin90");
            mFillRadial180Prop = this.mImageLayoutProp.FindPropertyRelative("ImageOrigin180");
            mFillRadial360Prop = this.mImageLayoutProp.FindPropertyRelative("ImageOrigin360");
            mFillAmountProp = this.mImageLayoutProp.FindPropertyRelative("FillAmount");
            mFillIsClockwiseProp = this.mImageLayoutProp.FindPropertyRelative("IsClockwise");
        }

        void OnDestroy()
        {
            mTarget = null;
        }

        public override void OnInspectorGUI()
        {
            using (var space = new EditorGUILayout.VerticalScope())
            {
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mSpriteProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildSprite();
                    return;
                }

                EditorGUILayout.PropertyField(this.mMaterialProp);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mWidthProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.AspectRatioSprite_Width(this.mWidthProp.floatValue);
                    this.mTarget.RebuildGemotry();
                    return;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mHeightProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.AspectRatioSprite_Height(this.mHeightProp.floatValue);
                    this.mTarget.RebuildGemotry();
                    return;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.mVertexColorProp);
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildGemotry();
                    return;
                }

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.mTypeProp);
                this.DrawImageTypeGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                    this.mTarget.RebuildGemotry();
                    return;
                }
            }
            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawImageTypeGUI()
        {
            ++EditorGUI.indentLevel;
            {
                int nImageTypeIndex = this.mTypeProp.enumValueIndex;
                if (nImageTypeIndex == 0)
                    DrawImageSimpleGUI();
                else if (nImageTypeIndex == 1)
                    DrawImageSlicedGUI();
                else if (nImageTypeIndex == 2)
                    DrawImageTiledGUI();
                else if (nImageTypeIndex == 3)
                    DrawImageFilledGUI();
            }
            --EditorGUI.indentLevel;
        }

        private void DrawImageSimpleGUI()
        {
            EditorGUIUtility.labelWidth = 130;
            EditorGUILayout.PropertyField(this.mIsPreserveAspectProp);

            using (var space = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUIUtility.labelWidth+4);
                if (GUILayout.Button("SetNativeSize", EditorStyles.miniButton))
                {
                    this.mTarget.SetSpriteNativeSize();
                    this.mTarget.RebuildGemotry();
                }
            }
        }

        private void DrawImageSlicedGUI()
        {
            if (this.mTarget == null || !this.mTarget.HasBorder())
            {
                EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
                return;
            }

            EditorGUIUtility.labelWidth = 130;
            EditorGUILayout.PropertyField(this.mIsFillCenterProp);
        }

        private void DrawImageTiledGUI()
        {
            if (this.mTarget == null || !this.mTarget.HasBorder())
            {
                EditorGUILayout.HelpBox("This Image doesn't have a border. Use image advance setting repeat mode will have a high effective.", MessageType.Warning);
                return;
            }

            EditorGUIUtility.labelWidth = 130;
            EditorGUILayout.PropertyField(this.mIsFillCenterProp);
        }

        private void DrawImageFilledGUI()
        {
            EditorGUILayout.PropertyField(this.mFillMethodProp);

            int nFillMethodIndex = this.mFillMethodProp.enumValueIndex;
            if (nFillMethodIndex == 0)
            {
                EditorGUILayout.PropertyField(this.mFillHorizontalOriginProp);
                this.serializedObject.ApplyModifiedProperties();
                this.mTarget.SetSpriteFillOrgin(this.mFillHorizontalOriginProp.enumValueIndex);
            }
            else if (nFillMethodIndex == 1)
            {
                EditorGUILayout.PropertyField(this.mFillVerticalOriginProp);
                this.serializedObject.ApplyModifiedProperties();
                this.mTarget.SetSpriteFillOrgin(this.mFillVerticalOriginProp.enumValueIndex);
            }
            else if (nFillMethodIndex == 2)
            {
                EditorGUILayout.PropertyField(this.mFillRadial90Prop);
                this.serializedObject.ApplyModifiedProperties();
                this.mTarget.SetSpriteFillOrgin(this.mFillRadial90Prop.enumValueIndex);
            }
            else if (nFillMethodIndex == 3)
            {
                EditorGUILayout.PropertyField(this.mFillRadial180Prop);
                this.serializedObject.ApplyModifiedProperties();
                this.mTarget.SetSpriteFillOrgin(this.mFillRadial180Prop.enumValueIndex);
            }
            else if (nFillMethodIndex == 4)
            {
                EditorGUILayout.PropertyField(this.mFillRadial360Prop);
                this.serializedObject.ApplyModifiedProperties();
                this.mTarget.SetSpriteFillOrgin(this.mFillRadial360Prop.enumValueIndex);
            }

            EditorGUILayout.PropertyField(this.mFillAmountProp);
            if (nFillMethodIndex >= 2)
                EditorGUILayout.PropertyField(this.mFillIsClockwiseProp);

            EditorGUILayout.PropertyField(this.mIsPreserveAspectProp);
            using (var space = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                if (GUILayout.Button("SetNativeSize", EditorStyles.miniButton))
                {
                    this.mTarget.SetSpriteNativeSize();
                    this.mTarget.RebuildGemotry();
                }
            }
        }
    }
}

