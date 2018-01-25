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
        private string          mSearchName  = "";
        
        [MenuItem("Window/Objects search")]
        static void Create()
        {
            var rWindow = EditorWindow.GetWindow<ObjectSearchWindow>();
            rWindow.titleContent = new GUIContent("ObjectSearchWindow");
        }

        void OnGUI()
        {
            using (var scope = new EditorGUILayout.VerticalScope())
            {
                using (var scope1 = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 65;
                    mObjectHideFlags = (HideFlags)EditorGUILayout.EnumFlagsField("HideFlags: ", mObjectHideFlags, GUILayout.MaxWidth(500));
                    EditorGUILayout.Space();
                }

                using (var scope1 = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 65;
                    mSearchObjType = EditorGUILayout.TextField("Type: ", mSearchObjType);
                    Type rSearchType = null;

                    if (GUILayout.Button("Search", GUILayout.Width(80)))
                    {
                        mSearchObjs = new List<Object>();
                        var assembies = AppDomain.CurrentDomain.GetAssemblies();
                        for (int i = 0; i < assembies.Length; i++)
                        {
                            rSearchType = assembies[i].GetType(mSearchObjType);
                            if (rSearchType != null) break;
                        }
                        if (rSearchType != null)
                        {
                            mSearchObjs = new List<Object>();
                            var rResultObjs = Resources.FindObjectsOfTypeAll(rSearchType);
                            for (int i = 0; i < rResultObjs.Length; i++)
                            {
                                if ((int)mObjectHideFlags != -1 && (rResultObjs[i].hideFlags & mObjectHideFlags) == mObjectHideFlags) continue;

                                mSearchObjs.Add(rResultObjs[i]);
                            }
                            rResultObjs = null;
                        }
                    }
                }

                EditorGUIUtility.labelWidth = 65;
                mSearchName = EditorGUILayout.TextField("Name: ", mSearchName);

                if (GUILayout.Button("Clear"))
                {
                    if (mSearchObjs != null)
                        mSearchObjs.Clear();
                }
                if (GUILayout.Button("UnloadUnusedAssets"))
                {
                    Resources.UnloadUnusedAssets();
                }

                if (mSearchObjs != null)
                {
                    EditorGUILayout.LabelField(string.Format("Result: {0}/{1}", mResultCount, mSearchObjs.Count));
                    mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
                    mResultCount = 0;
                    for (int i = 0; i < mSearchObjs.Count; i++)
                    {
                        if (mSearchObjs[i] == null)
                        {
                            EditorGUILayout.ObjectField(mSearchObjs[i], typeof(UnityEngine.Object), true);
                        }
                        else
                        {
                            if (!mSearchObjs[i].name.ToLower().Contains(mSearchName.ToLower()) && !string.IsNullOrEmpty(mSearchName)) continue;
                            EditorGUILayout.ObjectField(mSearchObjs[i], mSearchObjs[i].GetType(), true);
                        }
                        mResultCount++;
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }
    }
}

