// ABSimulateConfig的Inspector界面
using UnityEditor;
using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Assetbundle
{
    [CustomEditor(typeof(ABSimulateConfig))]
    public class ABSimulateConfigInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ABSimulateConfig rConfig = this.target as ABSimulateConfig;
            if (rConfig == null) return;

            var rIsDeplopModeProp = this.serializedObject.FindProperty("IsDevelopMode");
            EditorGUILayout.PropertyField(rIsDeplopModeProp, new GUIContent ("IsDevelopMode"));

            var rIsHotfixDebugModeProp = this.serializedObject.FindProperty("IsHotfixDebugMode");
            EditorGUILayout.PropertyField(rIsHotfixDebugModeProp, new GUIContent ("IsHotfixABMode"));

            var rSimulateTypeProp = this.serializedObject.FindProperty("SimulateType");
            var rSimulateType = (ABSimuluateType)rSimulateTypeProp.intValue;
            rSimulateType = (ABSimuluateType)EditorGUILayout.EnumFlagsField("SimulateType", rSimulateType);
            rSimulateTypeProp.intValue = (int)rSimulateType;

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}