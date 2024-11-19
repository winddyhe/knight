using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using Knight.Core;

namespace Knight.Framework.UI.Editor
{
    [CustomEditor(typeof(ViewControllerDataSource))]
    public class ViewControllerDataSourceInspector : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var rViewControlllerDataSource = this.target as ViewControllerDataSource;
            if (rViewControlllerDataSource == null) return;

            if (GUILayout.Button("Generate"))
            {
                ViewPrefabAutoSave.AutoSave(rViewControlllerDataSource);
            }
        }
    }
}
