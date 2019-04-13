using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CustomEditor(typeof(ViewModelContainer), true)]
    public class ViewModelContainerInspector : InspectorEditor
    {
        private ViewModelContainer mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as ViewModelContainer;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetAllViewModelDataSources();

            base.OnInspectorGUI();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
