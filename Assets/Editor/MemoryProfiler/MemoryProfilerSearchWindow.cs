using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core.Editor;

namespace MemoryProfilerWindow
{
    public class MemoryProfilerSearchWindow : EditorWindow
    {
        private MemoryProfilerDiffWindow            _diffWindow;

        private static MemoryProfilerSearchWindow   __instance;
        public  static MemoryProfilerSearchWindow   Instance { get { return __instance; } }

        private string                              mSearchText;
        private List<string>                        mObjectTypes;
        private int                                 mSelectedType;
        private string                              mSearchTypeText;
        private List<NativeUnityEngineObject>       mSearchResults;
        private List<NativeUnityEngineObject>       mSearchFilterResults;
        private Vector2                             mScrollPos;
        private bool                                mNeedFilterType;

        private int                                 mPageCount = 500;
        private int                                 mCurPage   = 1;

        [MenuItem("Window/MemoryProfilerSearch")]
        public static void Create()
        {
            __instance = EditorWindow.GetWindow<MemoryProfilerSearchWindow>() as MemoryProfilerSearchWindow;
        }

        public void OnEnable()
        {
            if (this._diffWindow == null)
            {
                this._diffWindow = EditorWindow.GetWindow<MemoryProfilerDiffWindow>() as MemoryProfilerDiffWindow;
            }
        }

        void OnGUI()
        {
            if (this._diffWindow == null || this._diffWindow.unpackedCrawl == null)
            {
                using (var rScope = new EditorGUILayout.VerticalScope())
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Has not snap memory...");
                    GUI.color = Color.white;
                }
                return;
            }

            var rUnpackedCrawl = this._diffWindow.unpackedCrawl;
            this.mObjectTypes = new List<string>();
            for (int i = 0; i < rUnpackedCrawl.nativeTypes.Length; i++)
            {
                if (string.IsNullOrEmpty(rUnpackedCrawl.nativeTypes[i].name)) continue;
                this.mObjectTypes.Add(rUnpackedCrawl.nativeTypes[i].name);
            }

            using (var rScope = new EditorGUILayout.VerticalScope())
            {
                EditorGUIUtility.labelWidth = 100;
                using (var rScope1 = new EditorGUILayout.HorizontalScope())
                {
                    mSelectedType = EditorGUILayout.Popup("Object Types: ", mSelectedType, this.mObjectTypes.ToArray());
                    mNeedFilterType = EditorGUILayout.Toggle("NeedFilterType: ", mNeedFilterType, GUILayout.Width(120));
                    mSearchTypeText = this.mObjectTypes[mSelectedType];
                    mSearchTypeText = EditorGUILayout.TextField(mSearchTypeText, GUILayout.Width(90));
                    this.SearchType_Contain();
                    
                    if (GUILayout.Button("Search", GUILayout.Width(90)))
                    {
                        this.Search(rUnpackedCrawl);
                    }
                }
            }

            if (mSearchResults != null)
            {
                this.mSearchFilterResults = this.SearchFilterContent();
                EditorGUILayout.LabelField(string.Format("Search results: {0}/{1}", mSearchFilterResults.Count, mSearchResults.Count));

                using (var rScope1 = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 150;
                    mSearchText = EditorGUILayout.TextField("Filter Search Content: ", mSearchText);
                    if (GUILayout.Button("Clear", GUILayout.Width(90)))
                    {
                        if (mSearchResults != null)       mSearchResults.Clear();
                        if (mSearchFilterResults != null) mSearchFilterResults.Clear();
                    }
                }

                EditorGUILayout.Space();
                mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
                for (int i = (mCurPage - 1) * mPageCount; i < mCurPage * mPageCount; i++)
                {
                    if (i >= this.mSearchFilterResults.Count) continue;

                    using (var rScope1 = new EditorGUILayout.HorizontalScope())
                    {
                        string result = string.Format(
                            "classname:{0}, name: {1}, size: {2}",
                            mSearchFilterResults[i].className,
                            mSearchFilterResults[i].name,
                            EditorAssists.ToMemorySize(mSearchFilterResults[i].size));

                        EditorGUILayout.LabelField(result);

                        if (GUILayout.Button("Select", GUILayout.Width(50)))
                        {
                            this._diffWindow.SelectThing(mSearchFilterResults[i]);
                            this._diffWindow.Repaint();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();

                using (var rScope1 = new EditorGUILayout.HorizontalScope())
                {
                    int nTotalPage = this.mSearchFilterResults.Count / mPageCount + 1;

                    if (GUILayout.Button("上一页"))
                    {
                        mCurPage--;
                        mCurPage = Mathf.Clamp(mCurPage, 1, nTotalPage);
                    }
                    EditorGUILayout.LabelField(string.Format("{0}/{1}", mCurPage, nTotalPage), GUILayout.Width(40));
                    if (GUILayout.Button("下一页"))
                    {
                        mCurPage++;
                        mCurPage = Mathf.Clamp(mCurPage, 1, nTotalPage);
                    }
                }
            }
        }

        private void SearchType_Contain()
        {
            mSelectedType = this.mObjectTypes.FindIndex((rItem) => { return rItem.ToLower().Contains(mSearchTypeText.ToLower()); });
            if (mSelectedType < 0) mSelectedType = 0;
        }

        private string SearchType_Equals()
        {
            return this.mObjectTypes.Find((rItem) => { return rItem.ToLower().Equals(mSearchTypeText.ToLower()); });
        }

        private void Search(CrawledMemorySnapshot rUnpackedCrawl)
        {
            string rSearchType = SearchType_Equals();

            mSearchResults = new List<NativeUnityEngineObject>();
            for (int i = 0; i < rUnpackedCrawl.nativeObjects.Length; i++)
            {
                var rNativeObj = rUnpackedCrawl.nativeObjects[i];
                if ((mNeedFilterType && rNativeObj.className.Equals(rSearchType)) || !mNeedFilterType)
                {
                    this.mSearchResults.Add(rNativeObj);
                }
            }
            this.mSearchResults.Sort((rItem1, rItem2) => { return rItem2.size.CompareTo(rItem1.size); });
        }

        private List<NativeUnityEngineObject> SearchFilterContent()
        {
            var rSearchFilterResults = new List<NativeUnityEngineObject>();
            for (int i = 0; i < this.mSearchResults.Count; i++)
            {
                if (string.IsNullOrEmpty(mSearchText) || 
                    (!string.IsNullOrEmpty(mSearchText) && this.mSearchResults[i].name.ToLower().Contains(mSearchText.ToLower())))
                {
                    rSearchFilterResults.Add(this.mSearchResults[i]);
                }
            }
            return rSearchFilterResults;
        }
    }
}
