using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.AssetBundles
{
    [System.Serializable]
    public class AssetbundlePreprocessingTab
    {
        [SerializeField]
        private Vector2     mScrollPos;

        public void OnEnable(UnityEngine.Rect rSubPos, EditorWindow rEditorWindow)
        {
        }

        public void Update()
        {
        }

        public void OnGUI(UnityEngine.Rect rect)
        {
            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
            EditorGUILayout.Space();

            using (var space1 = new EditorGUILayout.VerticalScope())
            {

            }

            EditorGUILayout.EndScrollView();
        }
    }
}
