using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using NaughtyAttributes.Editor;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(EventBinding), true)]
    public class EventBindingInspector : InspectorEditor
    {
        private EventBinding mTargetAbstract;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTargetAbstract = this.target as EventBinding;
        }

        public override void OnInspectorGUI()
        {
            this.mTargetAbstract.GetPaths();
            base.OnInspectorGUI();
        }
    }
}
