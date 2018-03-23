//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.AssetBundles
{
    public class ABHistoryEditor : EditorWindow
    {
        private ABHistory   mABHistory;
        private Vector2     mScrollPos;
        private string      mOpenFolderPath;

        [MenuItem("Tools/AssetBundle/AssetBundle History")]
        private static void Init()
        {
            var rABHistoryEditorWindow = EditorWindow.GetWindow<ABHistoryEditor>();
            rABHistoryEditorWindow.Show();

            string rABOutPath = ABBuilder.Instance.GetPathPrefix_Assetbundle();
            rABHistoryEditorWindow.mABHistory = new ABHistory();
            rABHistoryEditorWindow.mABHistory.Initialize(rABOutPath);
        }

        private void OnEnable()
        {
            string rABOutPath = ABBuilder.Instance.GetPathPrefix_Assetbundle();
            mABHistory = new ABHistory();
            mABHistory.Initialize(rABOutPath);
        }

        private void OnGUI()
        {
            if (mABHistory == null) return;

            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
            foreach (var rPair in mABHistory.Entries)
            {
                var rHEntry = rPair.Value;
                if (GUILayout.Button(rHEntry.Time))
                {
                    rHEntry.IsSelected = !rHEntry.IsSelected;
                }
                if (rHEntry.IsSelected)
                {
                    using (var space = new EditorGUILayout.VerticalScope("TextField"))
                    {
                        foreach (var rIncVerEntryPair in rHEntry.IncVer.Entries)
                        {
                            EditorGUIUtility.labelWidth = 30;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("AB: ",  rIncVerEntryPair.Value.Name);
                            EditorGUILayout.LabelField("Ver: ", rIncVerEntryPair.Value.Version.ToString());
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button("upload ab file", GUILayout.Width(120)))
                        {
                            mOpenFolderPath = EditorUtility.OpenFolderPanel("Upload ab file", mOpenFolderPath, "");
                            if (!string.IsNullOrEmpty(mOpenFolderPath))
                            {
                                string rABOutPath = ABBuilder.Instance.GetPathPrefix_Assetbundle();
                                rHEntry.IncVer.SaveIncrement(rABOutPath, mOpenFolderPath);
                                Debug.Log("Upload success!!");
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
