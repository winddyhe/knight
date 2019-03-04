using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Knight.Framework.AssetBundles;

namespace Knight.Framework.AssetBundles.Editor
{
    public class ABRefCountEditor : EditorWindow
    {
        private Vector2 mScrollPos;

        [MenuItem("Tools/AssetBundle/Assetbundle RefCount Debugger")]
        public static void Initialize()
        {
            var rWindow = GetWindow<ABRefCountEditor>(false, "ABRefCountDebugger", false);
            rWindow.autoRepaintOnSceneChange = true;
            rWindow.Show(true);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("UnloadUnusedAssets"))
            {
                Resources.UnloadUnusedAssets();
            }
            EditorGUILayout.LabelField("AB RefCount: ");

            if (ABLoaderVersion.Instance == null) return;

            this.mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
            using (var space = new EditorGUILayout.VerticalScope())
            {
                var rEntries = ABLoaderVersion.Instance.Entries;
                foreach (var rPair in rEntries)
                {
                    using (var space1 = new EditorGUILayout.HorizontalScope("TextField"))
                    {
                        EditorGUILayout.LabelField(rPair.Value.ABName);
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(rPair.Value.RefCount.ToString(), GUILayout.Width(50));
                    }
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
