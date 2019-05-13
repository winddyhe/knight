//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace Knight.Framework.Editor
{
    /// <summary>
    /// 资源预处理类
    /// </summary>
    public class CustomAssetPreprocess : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            PreprocessTexture_Role_Scene();
            PreprocessTexture_UI();
        }

        private void PreprocessTexture_Role_Scene()
        {
            if (!this.assetPath.Contains("OriginalRes/Role/") &&
                !this.assetPath.Contains("OriginalRes/Scene/"))
                return;

            TextureImporter rTexImporter = this.assetImporter as TextureImporter;
            TextureImporterPlatformSettings rSettings = new TextureImporterPlatformSettings();
            rSettings.name = "Standalone";
            rSettings.maxTextureSize = 1024;
            rSettings.compressionQuality = 0;
            rSettings.textureCompression = TextureImporterCompression.CompressedHQ;
            rTexImporter.SetPlatformTextureSettings(rSettings);
        }

        private void PreprocessTexture_UI()
        {
            if (!this.assetPath.Contains("GameAsset/GUI/Textures")) return;

            TextureImporter rTexImporter = this.assetImporter as TextureImporter;
            if (rTexImporter == null) return;

            rTexImporter.textureType = TextureImporterType.Sprite;
            rTexImporter.spriteImportMode = SpriteImportMode.Single;
            rTexImporter.alphaIsTransparency = true;
            rTexImporter.mipmapEnabled = false;
            rTexImporter.isReadable = false;
            rTexImporter.wrapMode = TextureWrapMode.Clamp;
            // set tag
            string rDirName = Path.GetFileName(Path.GetDirectoryName(this.assetPath));
            rTexImporter.spritePackingTag = rDirName;
            // set compress
            rTexImporter.textureCompression = TextureImporterCompression.CompressedHQ;
            // set border arg
            Vector4 rBorderArg = ParseBorderArg(this.assetPath);
            rTexImporter.spriteBorder = rBorderArg;
        }

        private Vector4 ParseBorderArg(string rAssetPath)
        {
            string rFileName = Path.GetFileNameWithoutExtension(rAssetPath);
            Match rRegexMatch = Regex.Match(rFileName, @"\[([^\{^\}]*)\]");
            string rBorderArgStr = rRegexMatch.Value;
            if (!string.IsNullOrEmpty(rBorderArgStr))
            {
                rBorderArgStr = rBorderArgStr.Trim('[', ']');
                string[] rBorderArgs = rBorderArgStr.Split(',');
                if (rBorderArgs.Length != 4)
                {
                    Debug.LogError("Border arg format error: " + rAssetPath);
                    return Vector4.zero;
                }
                int[] rBorderArgValues = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    int.TryParse(rBorderArgs[i].Trim(), out rBorderArgValues[i]);
                }
                return new Vector4(rBorderArgValues[2], rBorderArgValues[0], rBorderArgValues[3], rBorderArgValues[1]);
            }
            return Vector4.zero;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
        }
    }
}