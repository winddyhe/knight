using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Core.Editor;

namespace MemoryProfilerWindow
{
    public class MemorySnapDiffWindow : EditorWindow
    {
        private MemoryProfilerDiffWindow        _diffWindow;
        private static MemorySnapDiffWindow     __instance;
        public static MemorySnapDiffWindow      Instance { get { return __instance; } }

        [SerializeField]
        private NativeUnityEngineObject[]       _extraObjs;
        private bool                            _needCompareInstanceID = false;
        private Vector2                         _scrollPosition;
        private long                            _memorySize;

        [MenuItem("Window/MemorySnapDiffWindow")]
        public static void Create()
        {
            __instance = EditorWindow.GetWindow<MemorySnapDiffWindow>() as MemorySnapDiffWindow; 
        }

        public void OnEnable()
        {
            if (this._diffWindow == null)
            {
                this._diffWindow = EditorWindow.GetWindow<MemoryProfilerDiffWindow>() as MemoryProfilerDiffWindow;
            }
        }

        public void SetSnapshot(MemoryProfilerDiffWindow diffWindow)
        {
            this._diffWindow = diffWindow;
            this._extraObjs = null;
        }

        void OnGUI()
        {
            using (var scope = new EditorGUILayout.VerticalScope())
            {
                using (var scope1 = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Compare", GUILayout.Width(70)))
                    {
                        if (this._diffWindow.unpackedCrawlPrev == null || this._diffWindow.unpackedCrawl == null) return;

                        CompareSnapshot(this._diffWindow.unpackedCrawlPrev, this._diffWindow.unpackedCrawl);
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(50)))
                    {
                        if (this._extraObjs != null) 
                        {
                            this._extraObjs = null;
                        }
                    }
                    EditorGUIUtility.labelWidth = 160;
                    this._needCompareInstanceID = EditorGUILayout.Toggle("Need compare InstanceID: ", this._needCompareInstanceID);
                }

                if (this._extraObjs != null)
                { 
                    EditorGUILayout.LabelField(string.Format("Result: {0}, TotalMemory: {1}", this._extraObjs.Length, EditorAssists.ToMemorySize(this._memorySize)));
                    this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);
                    for (int i = 0; i < this._extraObjs.Length; i++)
                    {
                        DrawNativeObject(this._extraObjs[i]);
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void DrawNativeObject(NativeUnityEngineObject nativeObject)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                string result = string.Format("classname:{0}, name: {1}, size: {2}", nativeObject.className, nativeObject.name, EditorAssists.ToMemorySize(nativeObject.size));
                EditorGUILayout.LabelField(result, GUILayout.MaxWidth(500));
                EditorGUILayout.Space();
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    this._diffWindow.SelectThing(nativeObject);
                    this._diffWindow.Repaint();
                }
            }
        }
        
        /// <summary>
        /// 找出那些当前Snapshot中比上一次SnapShot多出来的Object
        ///     即是找出unpackedCrawl中在unpackedCrawlPrev不存在的对象
        /// </summary>
        private void CompareSnapshot(CrawledMemorySnapshot unpackedCrawlPrev, CrawledMemorySnapshot unpackedCrawl)
        {
            var tempObjs = new List<NativeUnityEngineObject>();
            this._memorySize = 0;

            var nativeObjs = unpackedCrawl.nativeObjects;
            var nativeObjsPrev = unpackedCrawlPrev.nativeObjects;
            
            EditorUtility.DisplayProgressBar("Tips", "Comparing...", 0.5f);
            for (int i = 0; i < nativeObjs.Length; i++)
            {
                bool hasSame = false;
                for (int k = 0; k < nativeObjsPrev.Length; k++)
                {
                    // 找到相同的
                    if (CompareNativeUnityEngineObject(nativeObjs[i], nativeObjsPrev[k]))
                    {
                        hasSame = true;
                        break;
                    }
                }
                if (!hasSame)
                {
                    tempObjs.Add(nativeObjs[i]);
                    this._memorySize += nativeObjs[i].size;
                }
            }
            tempObjs.Sort((item1, item2) => { return item2.size.CompareTo(item1.size); });
            this._extraObjs = tempObjs.ToArray();

            EditorUtility.ClearProgressBar();
        }

        private bool CompareNativeUnityEngineObject(NativeUnityEngineObject obj1, NativeUnityEngineObject obj2)
        {
            return  ((obj1.instanceID == obj2.instanceID && this._needCompareInstanceID) || !this._needCompareInstanceID) && 
                    obj1.size                   == obj2.size                &&
                    obj1.classID                == obj2.classID             &&
                    obj1.className              == obj2.className           &&
                    obj1.name                   == obj2.name                && 
                    obj1.isPersistent           == obj2.isPersistent        &&
                    obj1.isDontDestroyOnLoad    == obj2.isDontDestroyOnLoad &&
                    obj1.isManager              == obj2.isManager           &&
                    obj1.hideFlags              == obj2.hideFlags;
        }
    }
}

