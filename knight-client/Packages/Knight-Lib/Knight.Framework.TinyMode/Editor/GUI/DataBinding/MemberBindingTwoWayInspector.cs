using System;
using System.Collections.Generic;
using UnityEngine.UI;
using NaughtyAttributes.Editor;
using UnityEditor;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CustomEditor(typeof(MemberBindingTwoWay), true)]
    public class MemberBindingTwoWayInspector : MemberBindingAbstractInspector
    {
        private MemberBindingTwoWay     mTargetTwoWay;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mTargetTwoWay = this.target as MemberBindingTwoWay;
        }

        public override void OnInspectorGUI()
        {
            this.mTargetTwoWay.GetEventPaths();
            base.OnInspectorGUI();
        }
    }
}
