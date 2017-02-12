//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 资源预处理类
/// </summary>
public class CustomAssetPreprocess : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (this.assetPath.Contains("OriginalRes/Role/") || this.assetPath.Contains("OriginalRes/Scene/"))
        {
            TextureImporter rTexImporter = this.assetImporter as TextureImporter;

            int nMaxTextureSize = 1024;
            int nCompressQuality = 0;
            TextureImporterFormat rTexFormat = TextureImporterFormat.AutomaticCompressed;
            rTexImporter.SetPlatformTextureSettings("Standalone", nMaxTextureSize, rTexFormat, nCompressQuality, false);
        }
    }

    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            if (!importedAssets[i].Contains("Assets/Game/Knight/GameAsset/Hotfix/Libs")) continue;

            PluginImporter rPluginImporter = AssetImporter.GetAtPath(importedAssets[i]) as PluginImporter;
            if (rPluginImporter == null) continue;

            rPluginImporter.SetCompatibleWithAnyPlatform(false);
        }
    }
}
