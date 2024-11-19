using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Knight.Core;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Knight.Framework.Assetbundle.Editor
{
    public enum ABPackageType
    {
        HotfixFullPackage,  // 热更整包
        HotfixPackage,      // 热更资源包
    }

    public class ABBuilderConfig
    {
        public bool IsClearFolder;
        public bool IsCopyToStreamingAssets;
        public ABPackageType PackageType;
    }

    public class ABBuilderPanel
    {
        private VisualElement mRootVisualElement;
        private ABBuilderConfig mABBuildEntryConfig;

        public void Build(VisualElement rRootVisualElement)
        {
            this.mRootVisualElement = rRootVisualElement;
            var rVisualTreeXML = ABVisualTreeAssetManager.ABBuilderPanelAsset;
            rVisualTreeXML.CloneTree(this.mRootVisualElement);

            var rLibaryPath = PathTool.GetParentPath(Application.dataPath) + "/Library";
            var rABBuildConfigFilePath = rLibaryPath + "/ABBuildConfig.json";
            if (!File.Exists(rABBuildConfigFilePath))
            {
                this.mABBuildEntryConfig = new ABBuilderConfig();
                this.mABBuildEntryConfig.IsClearFolder = false;
                this.mABBuildEntryConfig.IsCopyToStreamingAssets = false;
                PathTool.WriteFile(rABBuildConfigFilePath, JsonConvert.SerializeObject(this.mABBuildEntryConfig));
            }

            var rPlatformTarget = this.mRootVisualElement.Q<TextField>("TargetText");
            rPlatformTarget.value = EditorUserBuildSettings.activeBuildTarget.ToString();

            this.mABBuildEntryConfig = JsonConvert.DeserializeObject<ABBuilderConfig>(File.ReadAllText(rABBuildConfigFilePath));

            var rOutputPath = this.mRootVisualElement.Q<TextField>("OutputPathText");
            rOutputPath.value = "./" + ABPlatform.Instance.GetCurPlatformABDir();

            var rIsClearFolder = this.mRootVisualElement.Q<Toggle>("ClearFolderToggle");
            rIsClearFolder.value = this.mABBuildEntryConfig.IsClearFolder;
            rIsClearFolder.RegisterCallback<ChangeEvent<bool>>((rChangeEvent) =>
            {
                this.mABBuildEntryConfig.IsClearFolder = rChangeEvent.newValue;
                File.WriteAllText(rABBuildConfigFilePath, JsonConvert.SerializeObject(this.mABBuildEntryConfig));
            });
            var rIsCopyToStreamingAssets = this.mRootVisualElement.Q<Toggle>("CopyStreamingToggle");
            rIsCopyToStreamingAssets.value = this.mABBuildEntryConfig.IsCopyToStreamingAssets;
            rIsCopyToStreamingAssets.RegisterCallback<ChangeEvent<bool>>((rChangeEvent) =>
            {
                this.mABBuildEntryConfig.IsCopyToStreamingAssets = rChangeEvent.newValue;
                File.WriteAllText(rABBuildConfigFilePath, JsonConvert.SerializeObject(this.mABBuildEntryConfig));
            });
            var rBuildPackageTypeEnum = this.mRootVisualElement.Q<EnumField>("BuildPackageTypeEnum");
            rBuildPackageTypeEnum.value = this.mABBuildEntryConfig.PackageType;
            rBuildPackageTypeEnum.RegisterCallback<ChangeEvent<Enum>>((rChangeEvent) => 
            {
                this.mABBuildEntryConfig.PackageType = (ABPackageType)rChangeEvent.newValue;
                File.WriteAllText(rABBuildConfigFilePath, JsonConvert.SerializeObject(this.mABBuildEntryConfig));
            });
            var rBuildButton = this.mRootVisualElement.Q<Button>("BuildButton");
            rBuildButton.clicked += this.OnBuildButtonClicked;
        }

        private void OnBuildButtonClicked()
        {
            var rOutputPath = "./" + ABPlatform.Instance.GetCurPlatformABDir();
            if (this.mABBuildEntryConfig.IsClearFolder)
            {
                var rOutputDirInfo = new DirectoryInfo(rOutputPath);
                if (rOutputDirInfo.Exists)
                {
                    rOutputDirInfo.Delete(true);
                }
            }
            var rOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            ABBuilder.Instance.Build(rOutputPath, this.mABBuildEntryConfig.PackageType, rOptions, EditorUserBuildSettings.activeBuildTarget);
            if (this.mABBuildEntryConfig.IsCopyToStreamingAssets)
            {
                ABBuilder.Instance.SyncAssetbundleToStreamingAssets(rOutputPath);
            }
        }
    }
}