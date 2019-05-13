using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ViewControllerContainer), true)]
    public class ViewModelContainerInspector : InspectorEditor
    {
        private ViewControllerContainer mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as ViewControllerContainer;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetAllViewModelDataSources();

            base.OnInspectorGUI();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
