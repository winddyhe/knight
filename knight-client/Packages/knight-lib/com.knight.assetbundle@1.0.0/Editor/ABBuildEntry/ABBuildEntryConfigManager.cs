using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Knight.Core;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABBuildEntryConfigManager : TSingleton<ABBuildEntryConfigManager>
    {
        private ABBuildEntryConfig mABBuildEntryConfig;
        private string mABBuilderConfigPath = "Assets/Game.Editor/Editor/Assetbundle/ABBuilderConfig.asset";

        public List<ABBuildEntry> ABBuildEntries => this.mABBuildEntryConfig.ABBuildEntries;

        private ABBuildEntryConfigManager()
        {
        }

        public void Initialize()
        {
            this.mABBuildEntryConfig = AssetDatabase.LoadAssetAtPath<ABBuildEntryConfig>(mABBuilderConfigPath);
            if (this.mABBuildEntryConfig == null)
            {
                this.mABBuildEntryConfig = ScriptableObject.CreateInstance<ABBuildEntryConfig>();
                AssetDatabase.CreateAsset(this.mABBuildEntryConfig, mABBuilderConfigPath);
                AssetDatabase.SaveAssets();
            }
        }

        public void AddABBuildEntry(ABBuildEntry rABBuildEntry)
        {
            this.mABBuildEntryConfig.ABBuildEntries.Add(rABBuildEntry);
            this.Save();
        }

        public void RemoveABBuildEntry(ABBuildEntry rABBuildEntry)
        {
            this.mABBuildEntryConfig.ABBuildEntries.Remove(rABBuildEntry);
            this.Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(this.mABBuildEntryConfig);
            AssetDatabase.SaveAssets();
            this.mABBuildEntryConfig = AssetDatabase.LoadAssetAtPath<ABBuildEntryConfig>(mABBuilderConfigPath);
        }
    }
}