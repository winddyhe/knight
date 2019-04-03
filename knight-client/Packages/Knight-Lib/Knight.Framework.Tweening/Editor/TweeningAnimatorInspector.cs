using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Knight.Framework.Tweening.Editor
{
    [CustomEditor(typeof(TweeningAnimator), true)]
    public class TweeningAnimatorInspector : UnityEditor.Editor
    {
        protected TweeningAnimator      mTweeningAnimator;
        protected SerializedProperty    mActionsProp;
        protected ReorderableList       mReorderableActionList;

        protected GUIContent            mLoopContent;
        protected GUIContent            mLoopTypeContent;
        protected GUIContent            mLoopCountContent;
        protected GUIContent            mDurationContent;
        protected GUIContent            mTimeCureveContent;

        private void OnEnable()
        {
            this.mLoopContent = new GUIContent("Loop: ");
            this.mLoopTypeContent = new GUIContent("Type");
            this.mLoopCountContent = new GUIContent("Count");
            this.mDurationContent = new GUIContent("Duration: ");
            this.mTimeCureveContent = new GUIContent("TimeCurve: ");

            this.mTweeningAnimator = this.target as TweeningAnimator;
            this.mActionsProp = this.serializedObject.FindProperty("Actions");
            this.BuildReorderableActionList();
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Add Tweening Action"))
            {
                this.AddNewAction();
            }

            this.mReorderableActionList.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
        }

        private void BuildReorderableActionList()
        {
            this.mReorderableActionList = new ReorderableList(this.serializedObject, this.mActionsProp, true, true, false, true);
            this.mReorderableActionList.drawHeaderCallback = this.ReorderableActionList_DrawHeader;
            this.mReorderableActionList.drawElementCallback = this.ReorderableActionList_DrawElement;
            this.mReorderableActionList.elementHeightCallback = this.ReorderableActionList_ElementHeight;
        }

        private void ReorderableActionList_DrawHeader(Rect rRect)
        {
            EditorGUI.LabelField(rRect, "Tweening Actions: ");
        }

        private void ReorderableActionList_DrawElement(Rect rRect, int nIndex, bool bIsActive, bool bIsFocused)
        {
            var rElementProp = this.mActionsProp.GetArrayElementAtIndex(nIndex);
            var rIsEnableProp = rElementProp.FindPropertyRelative("IsEnable");
            var rIsFoldProp = rElementProp.FindPropertyRelative("IsFold");
            var rTypeProp = rElementProp.FindPropertyRelative("Type");
            var rIsLoopProp = rElementProp.FindPropertyRelative("IsLoop");
            var rLoopTypeProp = rElementProp.FindPropertyRelative("LoopType");
            var rLoopCountProp = rElementProp.FindPropertyRelative("LoopCount");
            var rDurationProp = rElementProp.FindPropertyRelative("Duration");
            var rTimeCurveProp = rElementProp.FindPropertyRelative("TimeCurve");

            var rCurRect = rRect;
            rCurRect.y += 2;
            using (var space = new EditorGUILayout.HorizontalScope())
            {
                rCurRect.x += 10;
                rCurRect.height = 16;
                rCurRect.width = 15;
                var bIsFold = EditorGUI.Foldout(rCurRect, rIsFoldProp.boolValue, nIndex.ToString());
                rIsFoldProp.boolValue = bIsFold;

                rCurRect.x += 15;
                rCurRect.width = 15;
                EditorGUI.PropertyField(rCurRect, rIsEnableProp, GUIContent.none);

                GUI.enabled = rIsEnableProp.boolValue;

                rCurRect.x += 20;
                rCurRect.width = 90;
                EditorGUI.PropertyField(rCurRect, rTypeProp, GUIContent.none);
            }

            if (rIsFoldProp.boolValue)
            {
                // 第2行 画Duration
                rCurRect = rRect;
                rCurRect.y += 22;
                rCurRect.x += 25;
                rCurRect.width = 150;
                rCurRect.height = 16;
                EditorGUIUtility.labelWidth = 70f;
                EditorGUI.PropertyField(rCurRect, rDurationProp, this.mDurationContent);

                // 第3行 画Loop
                rCurRect = rRect;
                rCurRect.y += 20 * 2 + 2;
                rCurRect.x += 25;
                rCurRect.width = 85;
                rCurRect.height = 16;
                EditorGUIUtility.labelWidth = 70;
                EditorGUI.PropertyField(rCurRect, rIsLoopProp, this.mLoopContent);
                if (rIsLoopProp.boolValue)
                {
                    var rTmpRect = new Rect(rRect.x + 25, rCurRect.y-2, 350, 20);
                    EditorGUI.DrawRect(rTmpRect, new Color(0.2f, 0.2f, 0.2f, 0.2f));

                    rCurRect.x += 155;
                    rCurRect.width = 90;
                    rCurRect.height = 16;
                    EditorGUIUtility.labelWidth = 35f;
                    EditorGUI.PropertyField(rCurRect, rLoopTypeProp, this.mLoopTypeContent);

                    rCurRect.x += 100;
                    rCurRect.width = 90;
                    rCurRect.height = 16;
                    EditorGUIUtility.labelWidth = 40f;
                    EditorGUI.PropertyField(rCurRect, rLoopCountProp, this.mLoopCountContent);
                }

                // 第4行 画曲线抬头
                rCurRect = rRect;
                rCurRect.y += 20 * 3 + 2;
                rCurRect.x += 25;
                rCurRect.width = 75;
                rCurRect.height = 16;
                EditorGUI.LabelField(rCurRect, this.mTimeCureveContent);
                // 第5行 画曲线
                rCurRect = rRect;
                rCurRect.y += 20 * 4;
                rCurRect.x += 95;
                rCurRect.width = 75;
                rCurRect.height = 60;
                EditorGUI.PropertyField(rCurRect, rTimeCurveProp, GUIContent.none);

                // 后面的画动画类型参数
            }

            GUI.enabled = true;
            this.serializedObject.ApplyModifiedProperties();
        }

        private float ReorderableActionList_ElementHeight(int nIndex)
        {
            var rElementProp = this.mActionsProp.GetArrayElementAtIndex(nIndex);
            var rIsFoldProp = rElementProp.FindPropertyRelative("IsFold");
            if (rIsFoldProp.boolValue)
            {
                return 144f;
            }
            else
            {
                return 22f;
            }
        }

        private void AddNewAction()
        {
            var rLastIndex = this.mActionsProp.arraySize;
            this.mActionsProp.InsertArrayElementAtIndex(rLastIndex);
        }
    }
}