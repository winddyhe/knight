using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

namespace MemoryProfilerWindow
{
    public class ObjectSearchWindow : EditorWindow
    {
        [SerializeField]
        private List<Object>    mSearchObjs;
        [SerializeField]
        private string          mSearchObjType = "UnityEngine.Object";
        [SerializeField]
        private HideFlags       mObjectHideFlags = HideFlags.None;

        private Vector2         mScrollPos;
        private int             mResultCount = 0;
        private bool            mSelectedAll = false;
        private string          mSearchName  = "";
        
        [MenuItem("Window/Objects search")]
        static void Create()
        {
            EditorWindow.GetWindow<ObjectSearchWindow>();
        }

        void OnGUI()
        {
            using (var scope = new EditorGUILayout.VerticalScope())
            {
                using (var scope1 = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 40;
                    mSearchObjType = EditorGUILayout.TextField("Type: ", mSearchObjType);
                    Type rSearchType = null;
                    if (GUILayout.Button("Search", GUILayout.Width(80)))
                    {
                        var assembies = AppDomain.CurrentDomain.GetAssemblies();
                        for (int i = 0; i < assembies.Length; i++)
                        {
                            rSearchType = assembies[i].GetType(mSearchObjType);
                            if (rSearchType != null) break;
                        }
                        if (rSearchType != null)
                            mSearchObjs = new List<Object>(Resources.FindObjectsOfTypeAll(rSearchType));
                    }
                }

                using (var scope1 = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 65;
                    mObjectHideFlags = (HideFlags)EditorGUILayout.EnumPopup("HideFlags: ", mObjectHideFlags, GUILayout.MaxWidth(500));
                    EditorGUILayout.Space();
                    EditorGUIUtility.labelWidth = 25;
                    mSelectedAll = EditorGUILayout.Toggle("All: ", mSelectedAll);
                }

                EditorGUIUtility.labelWidth = 65;
                mSearchName = EditorGUILayout.TextField("Name: ", mSearchName);

                if (mSearchObjs != null)
                {
                    EditorGUILayout.LabelField(string.Format("Result: {0}/{1}", mResultCount, mSearchObjs.Count));
                    mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
                    mResultCount = 0;
                    for (int i = 0; i < mSearchObjs.Count; i++)
                    {
                        if (mSearchObjs[i] == null) continue;

                        if (mSearchObjs[i].hideFlags != mObjectHideFlags && !mSelectedAll) continue;

                        if (!mSearchObjs[i].name.ToLower().Contains(mSearchName.ToLower()) && !string.IsNullOrEmpty(mSearchName)) continue;

                        EditorGUILayout.ObjectField(mSearchObjs[i], mSearchObjs[i].GetType(), true);
                        mResultCount++;
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }
    }
}

