using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Knight.Framework.UI.Editor
{
    public class UIAssetPreprocess : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!this.assetPath.Contains("GameAssets/GUI/Textures")) return;

            var rTexImporter = this.assetImporter as TextureImporter;
            if (rTexImporter == null) return;

            if (this.assetPath.Contains("GameAssets/GUI/Textures/FullBG"))
            {
                rTexImporter.textureType = TextureImporterType.Default;
                rTexImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                rTexImporter.spriteBorder = Vector4.zero;
            }
            else
            {
                rTexImporter.textureType = TextureImporterType.Sprite;
                rTexImporter.spriteImportMode = SpriteImportMode.Single;
                rTexImporter.alphaIsTransparency = true;
                rTexImporter.mipmapEnabled = false;
                rTexImporter.isReadable = false;
                rTexImporter.wrapMode = TextureWrapMode.Clamp;
                // set border arg
                Vector4 rBorderArg = this.ParseBorderArg(this.assetPath);
                rTexImporter.spriteBorder = rBorderArg;
            }

            if (this.assetPath.Contains("GameAssets/GUI/Textures/FullBG") || 
                this.assetPath.Contains("GameAssets/GUI/Textures/Icons"))
            {
                // set compress
                rTexImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                this.SetTexturePlatformOverrideSetting(rTexImporter, true, 2048);
            }
            else
            {
                // set compress
                rTexImporter.textureCompression = TextureImporterCompression.Uncompressed;
                this.SetTexturePlatformOverrideSetting(rTexImporter, false, 2048);
            }
        }

        // 分平台设置图片格式
        private void SetTexturePlatformOverrideSetting(TextureImporter rTexImporter, bool bIsCompressed, int nMaxTextureSize = -1)
        {
            TextureImporterPlatformSettings rStandalone = rTexImporter.GetPlatformTextureSettings("Standalone");
            rStandalone.overridden = true;

            TextureImporterPlatformSettings rAndroid = rTexImporter.GetPlatformTextureSettings("Android");
            rAndroid.overridden = true;

            TextureImporterPlatformSettings rIOS = rTexImporter.GetPlatformTextureSettings("iPhone");
            rIOS.overridden = true;

            // 贴图大小设置
            if (rTexImporter.textureType != TextureImporterType.Sprite)
            {
                if (nMaxTextureSize > 0)
                {
                    rStandalone.maxTextureSize = nMaxTextureSize;
                    rAndroid.maxTextureSize = nMaxTextureSize;
                    rIOS.maxTextureSize = nMaxTextureSize;
                }
            }

            // 压缩格式设置
            if (rStandalone.textureCompression == TextureImporterCompression.Uncompressed)
                rStandalone.textureCompression = TextureImporterCompression.Compressed;
            if (rAndroid.textureCompression == TextureImporterCompression.Uncompressed)
                rAndroid.textureCompression = TextureImporterCompression.Compressed;
            if (rIOS.textureCompression == TextureImporterCompression.Uncompressed)
                rIOS.textureCompression = TextureImporterCompression.Compressed;

            if (bIsCompressed)
            {
                // windows/android纹理格式默认设置
                if (rStandalone.format != TextureImporterFormat.DXT1 && rStandalone.format != TextureImporterFormat.BC7)
                    rStandalone.format = TextureImporterFormat.DXT5;
                // 全部修改为ASTC_4X4
                rAndroid.format = TextureImporterFormat.ASTC_4x4;
                rIOS.format = TextureImporterFormat.ASTC_4x4;
            }
            else
            {
                // windows/android纹理格式默认设置
                rStandalone.format = TextureImporterFormat.RGBA32;
                rAndroid.format = TextureImporterFormat.RGBA32;
                rIOS.format = TextureImporterFormat.RGBA32;
            }
            rTexImporter.SetPlatformTextureSettings(rStandalone);
            rTexImporter.SetPlatformTextureSettings(rAndroid);
            rTexImporter.SetPlatformTextureSettings(rIOS);
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
    }
}
