
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABBuilderConfigPanel
    {
        private VisualElement mRootVisualElement;

        public void Build(VisualElement rRootVisualElement)
        {
            this.mRootVisualElement = rRootVisualElement;

            ABBuildEntryConfigManager.Instance.Initialize();

            var rVisualTreeXML = ABVisualTreeAssetManager.ABBuilderConfigPanelAsset;
            rVisualTreeXML.CloneTree(this.mRootVisualElement);

            this.BuildButtonRegion();
            this.BuildABBuildEntryList();
        }

        private void BuildButtonRegion()
        {
            var rAddFileButton = this.mRootVisualElement.Q<Button>("AddButton");
            rAddFileButton.clicked += this.OnAddButtonClicked;

            var rSaveButton = this.mRootVisualElement.Q<Button>("SaveButton");
            rSaveButton.clicked += this.OnSaveButtonClicked;
        }

        private void OnAddButtonClicked()
        {
            var rABBuildEntry = new ABBuildEntry();
            rABBuildEntry.ABVariant = "ab";
            rABBuildEntry.ABBuildPreprocessor = "ABBuildPreprocessorDefault";
            ABBuildEntryConfigManager.Instance.AddABBuildEntry(rABBuildEntry);
            var rABBuildEntryList = this.mRootVisualElement.Q<ListView>("ABBuildEntryList");
            rABBuildEntryList.Rebuild();
        }

        private void OnSaveButtonClicked()
        {
            ABBuildEntryConfigManager.Instance.Save();
        }

        private void BuildABBuildEntryList()
        {
            var rABBuildEntryList = this.mRootVisualElement.Q<ListView>("ABBuildEntryList");
            rABBuildEntryList.itemsSource = ABBuildEntryConfigManager.Instance.ABBuildEntries;
            rABBuildEntryList.makeItem = () =>
            {
                return this.CreateVisualElement();
            };
            rABBuildEntryList.bindItem = (rVisualElement, nIndex) =>
            {
                if (nIndex >= 0 && nIndex < ABBuildEntryConfigManager.Instance.ABBuildEntries.Count)
                {
                    var rABBuildEntry = ABBuildEntryConfigManager.Instance.ABBuildEntries[nIndex];
                    this.BindVisualElement(rVisualElement, rABBuildEntry);
                }
            };
        }

        public VisualElement CreateVisualElement()
        {
            var rRootVisualElement = new VisualElement();
            var rVisualTreeXML = ABVisualTreeAssetManager.ABBuildEntryTreeAsset;
            rVisualTreeXML.CloneTree(rRootVisualElement);
            return rRootVisualElement;
        }

        public void BindVisualElement(VisualElement rVisualElement, ABBuildEntry rABBuildEntry)
        {
            var rABBuildType = rVisualElement.Q<EnumField>("ABBuildType");
            rABBuildType.value = rABBuildEntry.ABBuildType;
            rABBuildType.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ABBuildType = (ABBuildType)evt.newValue;
            });

            var rABNameText = rVisualElement.Q<TextField>("ABNameText");
            rABNameText.value = rABBuildEntry.ABName;
            rABNameText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ABName = evt.newValue;

                var rItemRoot = rVisualElement.Q<Foldout>("ItemRoot");
                rItemRoot.text = rABBuildEntry.ABFullName;
            });

            var rABAliasSuffix = rVisualElement.Q<TextField>("ABAliasSuffixText");
            rABAliasSuffix.value = rABBuildEntry.ABAliasSuffix;
            rABAliasSuffix.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ABAliasSuffix = evt.newValue;
                var rItemRoot = rVisualElement.Q<Foldout>("ItemRoot");
                rItemRoot.text = rABBuildEntry.ABFullName;
            });

            var rABVariantText = rVisualElement.Q<TextField>("ABVariantText");
            rABVariantText.value = rABBuildEntry.ABVariant;
            rABVariantText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ABVariant = evt.newValue;

                var rItemRoot = rVisualElement.Q<Foldout>("ItemRoot");
                rItemRoot.text = rABBuildEntry.ABFullName;
            });

            var rResPathText = rVisualElement.Q<TextField>("ResPathText");
            rResPathText.value = rABBuildEntry.ResPath;
            rResPathText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ResPath = evt.newValue;
            });

            var rFilterTypeText = rVisualElement.Q<TextField>("FilterTypeText");
            rFilterTypeText.value = rABBuildEntry.FilterType;
            rFilterTypeText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.FilterType = evt.newValue;
            });

            var rFilterPatternText = rVisualElement.Q<TextField>("FilterPatternText");
            rFilterPatternText.value = rABBuildEntry.FilterPattern;
            rFilterPatternText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.FilterPattern = evt.newValue;
            });

            var rABBuildPreprocessorText = rVisualElement.Q<TextField>("ABBuildPreprocessorText");
            rABBuildPreprocessorText.value = rABBuildEntry.ABBuildPreprocessor;
            rABBuildPreprocessorText.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.ABBuildPreprocessor = evt.newValue;
            });

            var rIsAssetbundleToggle = rVisualElement.Q<Toggle>("IsAssetbundle");
            rIsAssetbundleToggle.value = rABBuildEntry.IsAssetBundle;
            rIsAssetbundleToggle.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.IsAssetBundle = evt.newValue;
            });

            var rIsNeedAssetListToggle = rVisualElement.Q<Toggle>("IsNeedAssetList");
            rIsNeedAssetListToggle.value = rABBuildEntry.IsNeedAssetList;
            rIsNeedAssetListToggle.RegisterValueChangedCallback((evt) =>
            {
                rABBuildEntry.IsNeedAssetList = evt.newValue;
            });

            var rItemRoot = rVisualElement.Q<Foldout>("ItemRoot");
            rItemRoot.text = rABBuildEntry.ABFullName;

            var rSetButton = rVisualElement.Q<Button>("ResetButton");
            rSetButton.clickable.clicked += () => { this.OnSetButtonClicked(rVisualElement, rABBuildEntry); };

            var rDelButton = rVisualElement.Q<Button>("DelButton");
            rDelButton.clickable.clicked += () => { this.OnDelButtonClicked(rABBuildEntry); };

            var rUpButton = rVisualElement.Q<Button>("UpButton");
            rUpButton.clickable.clicked += () => { this.OnUpButtonClicked(rABBuildEntry); };

            var rDownButton = rVisualElement.Q<Button>("DownButton");
            rDownButton.clickable.clicked += () => { this.OnDownButtonClicked(rABBuildEntry); };
        }

        private void OnSetButtonClicked(VisualElement rVisualElement, ABBuildEntry rABBuildEntry)
        {
            var rSelectDirectory = "";
            if (rABBuildEntry.ABBuildType != ABBuildType.File)
            {
                rSelectDirectory = EditorUtility.OpenFolderPanel("Select ABBuildEntry", Application.dataPath + "/GameAssets", "");

                var rResPath = rSelectDirectory.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                rABBuildEntry.ResPath = rResPath;
                rABBuildEntry.ABName = rResPath.Replace("Assets/GameAssets/", "game/").ToLower();
            }
            else
            {
                var rSelectFullFile = EditorUtility.OpenFilePanel("Select ABBuildEntry", Application.dataPath + "/GameAssets", "");
                rSelectDirectory = rSelectFullFile.Replace(Path.GetExtension(rSelectFullFile), "");
                
                var rResPath = rSelectFullFile.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                rABBuildEntry.ResPath = rResPath;

                var rDirPath = rSelectDirectory.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                rABBuildEntry.ABName = rDirPath.Replace("Assets/GameAssets/", "game/").ToLower();
            }
            if (string.IsNullOrEmpty(rSelectDirectory))
            {
                return;
            }
            var rABNameText = rVisualElement.Q<TextField>("ABNameText");
            rABNameText.value = rABBuildEntry.ABName;
            var rResPathText = rVisualElement.Q<TextField>("ResPathText");
            rResPathText.value = rABBuildEntry.ResPath;
        }

        private void OnDelButtonClicked(ABBuildEntry rABBuildEntry)
        {
            ABBuildEntryConfigManager.Instance.RemoveABBuildEntry(rABBuildEntry);
            var rABBuildEntryList = this.mRootVisualElement.Q<ListView>("ABBuildEntryList");
            rABBuildEntryList.Rebuild();
        }

        private void OnUpButtonClicked(ABBuildEntry rABBuildEntry)
        {
            var nIndex = ABBuildEntryConfigManager.Instance.ABBuildEntries.IndexOf(rABBuildEntry);
            if (nIndex == 0) return;

            var rUpABBuildEntry = ABBuildEntryConfigManager.Instance.ABBuildEntries[nIndex - 1];
            var rTempABBuildEntry = new ABBuildEntry();
            rTempABBuildEntry.Clone(rABBuildEntry);
            rABBuildEntry.Clone(rUpABBuildEntry);
            rUpABBuildEntry.Clone(rTempABBuildEntry);

            var rABBuildEntryList = this.mRootVisualElement.Q<ListView>("ABBuildEntryList");
            rABBuildEntryList.Rebuild();
        }

        private void OnDownButtonClicked(ABBuildEntry rABBuildEntry)
        {
            var nIndex = ABBuildEntryConfigManager.Instance.ABBuildEntries.IndexOf(rABBuildEntry);
            if (nIndex == ABBuildEntryConfigManager.Instance.ABBuildEntries.Count - 1) return;

            var rDownABBuildEntry = ABBuildEntryConfigManager.Instance.ABBuildEntries[nIndex + 1];
            var rTempABBuildEntry = new ABBuildEntry();
            rTempABBuildEntry.Clone(rABBuildEntry);
            rABBuildEntry.Clone(rDownABBuildEntry);
            rDownABBuildEntry.Clone(rTempABBuildEntry);

            var rABBuildEntryList = this.mRootVisualElement.Q<ListView>("ABBuildEntryList");
            rABBuildEntryList.Rebuild();
        }
    }
}

