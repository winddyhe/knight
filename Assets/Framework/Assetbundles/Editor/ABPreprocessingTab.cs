//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AssetBundles;
using Core;
using Core.Editor;
using Core.WindJson;

namespace UnityEditor.AssetBundles
{
    [System.Serializable]
    public class ABPreprocessingTab
    {
        public class EntryData
        {
            public bool                 IsSettingOpen = false;
            public ABEntry              Entry;
        }

        [SerializeField]
        private Vector2                 mScrollPos;
        private Texture2D               mRefreshTexture;

        private ABEntryConfig           mABEntryConfig;
        private bool                    mAdvancedSettings;

        private List<EntryData>         mEntryDatas;

        public void OnEnable(UnityEngine.Rect rSubPos, EditorWindow rEditorWindow)
        {
            mABEntryConfig = EditorAssists.ReceiveAsset<ABEntryConfig>(ABBuilder.ABEntryConfigPath);
            mEntryDatas = this.ToEntryDatas(mABEntryConfig);
            mRefreshTexture = EditorGUIUtility.FindTexture("Refresh");
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
                using (var space2 = new EditorGUILayout.HorizontalScope("TextField"))
                {
                    EditorGUIUtility.labelWidth = 60;
                    EditorGUILayout.TextField("Target: ", ABBuilder.Instance.CurBuildPlatform.ToString());
                    if (GUILayout.Button(mRefreshTexture, GUILayout.Width(30)))
                    {
                        mABEntryConfig = EditorAssists.ReceiveAsset<ABEntryConfig>(ABBuilder.ABEntryConfigPath);
                        mEntryDatas = this.ToEntryDatas(mABEntryConfig);
                    }
                }
                EditorGUILayout.Space();


                for (int i = 0; i < mEntryDatas.Count; i++)
                {
                    using (var space2 = new EditorGUILayout.VerticalScope("TextField"))
                    {
                        using (var space3 = new EditorGUILayout.HorizontalScope())
                        {
                            mEntryDatas[i].IsSettingOpen = EditorGUILayout.Foldout(mEntryDatas[i].IsSettingOpen, mEntryDatas[i].Entry.abName);
                            if (GUILayout.Button("Del", GUILayout.Width(30)))
                            {
                                mEntryDatas.RemoveAt(i);
                                return;
                            }
                        }

                        EditorGUILayout.Space();
                        if (mEntryDatas[i].IsSettingOpen)
                        {
                            EditorGUIUtility.labelWidth = 150;
                            mEntryDatas[i].Entry.abName = EditorGUILayout.TextField("Assetbundle Name: ", mEntryDatas[i].Entry.abName);
                            mEntryDatas[i].Entry.abVariant = EditorGUILayout.TextField("Assetbundle Variant: ", mEntryDatas[i].Entry.abVariant);
                            mEntryDatas[i].Entry.assetResPath = EditorGUILayout.TextField("Asset Res Path: ", mEntryDatas[i].Entry.assetResPath);
                            mEntryDatas[i].Entry.assetSrcType = (ABEntry.AssetSourceType)EditorGUILayout.EnumPopup("Asset Src Type: ", mEntryDatas[i].Entry.assetSrcType);
                            mEntryDatas[i].Entry.assetType = EditorGUILayout.TextField("Asset Type: ", mEntryDatas[i].Entry.assetType);
                            mEntryDatas[i].Entry.abClassName = EditorGUILayout.TextField("Assetbundle Class: ", mEntryDatas[i].Entry.abClassName);
                            mEntryDatas[i].Entry.abOriginalResPath = EditorGUILayout.TextField("Assetbundle Original Res: ", mEntryDatas[i].Entry.abOriginalResPath);

                            // @TODO: 需要编写方便的数组Editor控件
                            //string rFilterStr = "";
                            //if (mEntryDatas[i].Entry.filerAssets != null) JsonParser.ToJsonNode(mEntryDatas[i].Entry.filerAssets).ToString();
                            //rFilterStr = EditorGUILayout.TextField("Asset Filter: ", rFilterStr);
                            //if (!string.IsNullOrEmpty(rFilterStr))
                            //{
                            //    JsonNode rFilterJsonNode = JsonParser.Parse(rFilterStr);
                            //    mEntryDatas[i].Entry.filerAssets = rFilterJsonNode.ToList<string>();
                            //}
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add"))
            {
                mEntryDatas.Add(new EntryData() { IsSettingOpen = false, Entry = new ABEntry() });
            }
            if (GUILayout.Button("Save"))
            {
                mABEntryConfig = ToEntryConfig(mEntryDatas);
                EditorUtility.SetDirty(mABEntryConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Update All Assets AB Labels"))
            {
                ABBuilder.Instance.UpdateAllAssetsABLabels(ABBuilder.ABEntryConfigPath);
            }
        }

        public List<EntryData> ToEntryDatas(ABEntryConfig rEntryConfig)
        {
            var rEntryDatas = new List<EntryData>();
            if (rEntryConfig == null || rEntryConfig.ABEntries == null) return rEntryDatas;

            foreach (var rPair in rEntryConfig.ABEntries)
            {
                var rEntryData = new EntryData() { IsSettingOpen = false, Entry = rPair };
                rEntryDatas.Add(rEntryData);
            }
            return rEntryDatas;
        }

        public ABEntryConfig ToEntryConfig(List<EntryData> rEntryDatas)
        {
            ABEntryConfig rEntryConfig = EditorAssists.ReceiveAsset<ABEntryConfig>(ABBuilder.ABEntryConfigPath);
            if (rEntryConfig.ABEntries == null) rEntryConfig.ABEntries = new List<ABEntry>();
            rEntryConfig.ABEntries.Clear();
            for (int i = 0; i < rEntryDatas.Count; i++)
            {
                rEntryConfig.ABEntries.Add(rEntryDatas[i].Entry);
            }
            return rEntryConfig;
        }
    }
}
