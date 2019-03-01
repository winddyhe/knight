//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace Knight.Framework.Hotfix.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HotfixEventTrigger), true)]
    public class HotfixEventTriggerInspector : UnityEditor.Editor
    {
        private HotfixEventTrigger  mTarget;
        private SerializedProperty  mEventObj;

        void OnEnable()
        {
            mTarget = this.target as HotfixEventTrigger;
            mEventObj = this.serializedObject.FindProperty("EventObj");
        }

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 100;
            mTarget.EventTypeMask = (HotfixEventTrigger.TriggerType)EditorGUILayout.EnumFlagsField("EventTypeMask: ", mTarget.EventTypeMask);
            EditorGUILayout.PropertyField(mEventObj);
             
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
