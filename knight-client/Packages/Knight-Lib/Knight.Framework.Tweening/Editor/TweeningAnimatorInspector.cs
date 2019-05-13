using Knight.Core;
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
        protected GUIContent            mStartContent;
        protected GUIContent            mEndContent;

        private void OnEnable()
        {
            this.mLoopContent = new GUIContent("Loop: ");
            this.mLoopTypeContent = new GUIContent("Type");
            this.mLoopCountContent = new GUIContent("Count");
            this.mDurationContent = new GUIContent("Duration: ");
            this.mTimeCureveContent = new GUIContent("TimeCurve: ");
            this.mStartContent = new GUIContent("Start: ");
            this.mEndContent = new GUIContent("End: ");

            this.mTweeningAnimator = this.target as TweeningAnimator;
            this.mActionsProp = this.serializedObject.FindProperty("Actions");
            this.BuildReorderableActionList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            using (var rHorizontalScope = new EditorGUILayout.HorizontalScope("box"))
            {
                
                if (GUILayout.Button("Play"))
                {
                    this.mTweeningAnimator.Play();
                }

                if (GUILayout.Button("Pause"))
                {
                    this.mTweeningAnimator.Pause();
                }

                if (GUILayout.Button("Stop"))
                {
                    this.mTweeningAnimator.Stop();
                }
            }
            using (var rHorizontalScope = new EditorGUILayout.HorizontalScope("box")) {
                EditorGUIUtility.labelWidth = 65;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsIgnoreTimeScale"),
                    new GUIContent("TimeScale: "), GUILayout.Width(75));
                EditorGUIUtility.labelWidth = 35;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsUseFixedUpdate"),
                    new GUIContent("Fixed: "), GUILayout.Width(45));
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsAutoExecute"),
                    new GUIContent("Auto: "), GUILayout.Width(45));
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsLoopAnimator"),
                    new GUIContent("Loop: "), GUILayout.Width(45));
            }
            this.mReorderableActionList.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();
        }

        private void BuildReorderableActionList()
        {
            this.mReorderableActionList = new ReorderableList(this.serializedObject, this.mActionsProp, true, true, true, true);
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
                var rActionType = (TweeningActionType)rTypeProp.enumValueIndex;
                SerializedProperty rStartProp = null;
                SerializedProperty rEndProp = null;
                switch (rActionType)
                {
                    case TweeningActionType.Position:
                    case TweeningActionType.LocalPosition:
                    case TweeningActionType.Rotate:
                    case TweeningActionType.LocalRotate:
                    case TweeningActionType.LocalScale:
                        rStartProp = rElementProp.FindPropertyRelative("StartV3");
                        rEndProp = rElementProp.FindPropertyRelative("EndV3");
                        break;
                    case TweeningActionType.Color:
                        rStartProp = rElementProp.FindPropertyRelative("StartCol");
                        rEndProp = rElementProp.FindPropertyRelative("EndCol");
                        break;
                    case TweeningActionType.CanvasAlpha:
                        rStartProp = rElementProp.FindPropertyRelative("StartF");
                        rEndProp = rElementProp.FindPropertyRelative("EndF");
                        break;
                }
                if (rStartProp != null)
                {
                    // 画矩形背景
                    var rTmpRect = new Rect(rRect.x + 25, rRect.y + 20 * 5 + 48, 350, 40);
                    EditorGUI.DrawRect(rTmpRect, new Color(0.2f, 0.2f, 0.2f, 0.2f));
                    // 第6行 画Start Prop
                    rCurRect = rRect;
                    rCurRect.y += 20 * 5 + 50;
                    rCurRect.x += 25;
                    rCurRect.width = 280;
                    rCurRect.height = 16;
                    EditorGUIUtility.labelWidth = 70;
                    EditorGUI.PropertyField(rCurRect, rStartProp, this.mStartContent);
                    // 第7行 画End Prop
                    rCurRect = rRect;
                    rCurRect.y += 20 * 6 + 50;
                    rCurRect.x += 25;
                    rCurRect.width = 280;
                    rCurRect.height = 16;
                    EditorGUIUtility.labelWidth = 70;
                    EditorGUI.PropertyField(rCurRect, rEndProp, this.mEndContent);
                }
            }

            GUI.enabled = true;
            this.serializedObject.ApplyModifiedProperties();
        }

        private float ReorderableActionList_ElementHeight(int nIndex)
        {
            var rElementProp = this.mActionsProp.GetArrayElementAtIndex(nIndex);
            var rIsFoldProp = rElementProp.FindPropertyRelative("IsFold");
            var rTypeProp = rElementProp.FindPropertyRelative("Type");
            if (rIsFoldProp.boolValue)
            {
                var rActionType = (TweeningActionType)rTypeProp.enumValueIndex;
                switch (rActionType)
                {
                    case TweeningActionType.Position:
                    case TweeningActionType.LocalPosition:
                    case TweeningActionType.Rotate:
                    case TweeningActionType.LocalRotate:
                    case TweeningActionType.LocalScale:
                    case TweeningActionType.Color:
                    case TweeningActionType.CanvasAlpha:
                        return 194;
                }
                return 154f;
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