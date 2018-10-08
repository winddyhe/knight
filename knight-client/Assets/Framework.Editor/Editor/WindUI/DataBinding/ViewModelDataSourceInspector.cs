using System;
using System.Collections.Generic;
using UnityEngine.UI;
using NaughtyAttributes.Editor;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ViewModelDataSource), true)]
    public class ViewModelDataSourceInspector : InspectorEditor
    {
        private ViewModelDataSource mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as ViewModelDataSource;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetPaths();
            base.OnInspectorGUI();
        }
    }
}
