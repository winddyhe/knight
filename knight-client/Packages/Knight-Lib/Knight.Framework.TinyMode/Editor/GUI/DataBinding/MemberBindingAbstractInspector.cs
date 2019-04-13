using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using NaughtyAttributes.Editor;
using UnityEditor;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CustomEditor(typeof(MemberBindingAbstract), true)]
    public class MemberBindingAbstractInspector : InspectorEditor
    {
        private MemberBindingAbstract mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as MemberBindingAbstract;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetPaths();
            base.OnInspectorGUI();

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
