using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NaughtyAttributes.Editor;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(DataBindingOneWay), true)]
    public class DataBindingOneWayInspector : InspectorEditor
    {
        private DataBindingOneWay mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as DataBindingOneWay;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetAllModelPaths();
            this.mTarget.GetAllViewPaths();

            base.OnInspectorGUI();
        }
    }
}
