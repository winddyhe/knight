using NaughtyAttributes.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ImageAssist), true)]
    public class ImageAssistInspector : InspectorEditor
    {
        private ImageAssist         mTarget;

        private SerializedProperty  mAdjustValueProp;
        private SerializedProperty  mBlendSrcProp;
        private SerializedProperty  mBlendDstProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.mTarget = this.target as ImageAssist;

            this.mAdjustValueProp = this.serializedObject.FindProperty("AdjustValue");
            this.mBlendSrcProp = this.serializedObject.FindProperty("BlendSrc");
            this.mBlendDstProp = this.serializedObject.FindProperty("BlendDst");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (var space = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("模式一"))
                {
                    this.mAdjustValueProp.floatValue = 0.2f;
                    this.mBlendSrcProp.intValue = 5;
                    this.mBlendDstProp.intValue = 10;
                    this.mTarget.UpdateParams();
                }
                if (GUILayout.Button("模式二"))
                {
                    this.mAdjustValueProp.floatValue = 2.0f;
                    this.mBlendSrcProp.intValue = 5;
                    this.mBlendDstProp.intValue = 1;
                    this.mTarget.UpdateParams();
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}