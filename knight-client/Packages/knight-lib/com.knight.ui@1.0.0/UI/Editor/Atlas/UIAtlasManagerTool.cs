using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Knight.Framework.UI.Editor
{
    public class UIAtlasManagerTool
    {
        private static readonly string TextureRootPath = "Assets/GameAssets/GUI/Textures/";

        private static readonly string TexConfigPath = "Assets/GameAssets/GUI/Textures/Config";
        private static readonly string TexAtlasPath = "Assets/GameAssets/GUI/Textures/Atlas";
        private static readonly string TexIconPath = "Assets/GameAssets/GUI/Textures/Icons";
        private static readonly string TexFullBGPath = "Assets/GameAssets/GUI/Textures/FullBG";
        private static readonly string SpriteAtlasPath = "Assets/GameAssets/GUI/Textures/SpriteAtlas/";

        private static readonly string UIPrefabPath = "Assets/GameAssets/GUI/Prefabs";

        private static readonly string TexIconABPath = "game/gui/textures/icons/";
        private static readonly string TexAtlasABPath = "game/gui/textures/atlas/";

        [MenuItem("Tools/UI/Generate Atlas Config")]
        public static void Generate()
        {
            GenerateAtlasConfig();
            GenerateSpriteAtlas(TextureRootPath, "Atlas", SpriteAtlasPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);
        }

        private static void GenerateAtlasConfig()
        {
            {
                // 生成AtlasIconData.asset
                var rUIAtlasIconDataPath = TexConfigPath + "/" + "AtlasIconData.asset";
                var rUIAtlasIconData = AssetDatabase.LoadAssetAtPath(rUIAtlasIconDataPath, typeof(UIAtlasIconData)) as UIAtlasIconData;
                if (rUIAtlasIconData == null)
                {
                    rUIAtlasIconData = ScriptableObject.CreateInstance<UIAtlasIconData>();
                    AssetDatabase.CreateAsset(rUIAtlasIconData, rUIAtlasIconDataPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    rUIAtlasIconData = AssetDatabase.LoadAssetAtPath(rUIAtlasIconDataPath, typeof(UIAtlasIconData)) as UIAtlasIconData;
                }
                if (rUIAtlasIconData.AtlasIconLinks == null)
                    rUIAtlasIconData.AtlasIconLinks = new List<UIAtlasIconLink>();
                rUIAtlasIconData.AtlasIconLinks.Clear();
                var rAtlasIconDirectoryInfo = new DirectoryInfo(TexIconPath);
                var rAllAtlasIconInfos = new List<DirectoryInfo>(rAtlasIconDirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly));

                var rFullBGDirectorInfo = new DirectoryInfo(TexFullBGPath);
                rAllAtlasIconInfos.Add(rFullBGDirectorInfo);
                for (int i = 0; i < rAllAtlasIconInfos.Count; i++)
                {
                    var rAtlasIconName = rAllAtlasIconInfos[i].Name.ToLower();
                    var rAtlasIconLink = new UIAtlasIconLink() { ABName = TexIconABPath + rAtlasIconName + ".ab" };
                    rAtlasIconLink.IconNames = new List<string>();

                    var rAllFileInfos = rAllAtlasIconInfos[i].GetFiles("*.png", SearchOption.AllDirectories);
                    for (int j = 0; j < rAllFileInfos.Length; j++)
                    {
                        rAtlasIconLink.IconNames.Add(Path.GetFileNameWithoutExtension(rAllFileInfos[j].Name));
                    }
                    rUIAtlasIconData.AtlasIconLinks.Add(rAtlasIconLink);
                }
                EditorUtility.SetDirty(rUIAtlasIconData);
                AssetDatabase.SaveAssets();
            }
            {
                // PrefabLinkAtlas.asset
                var rUIPrefabLinkAtlasPath = TexConfigPath + "/" + "PrefabLinkAtlas.asset";
                var rUIPrefabLinkAtlas = AssetDatabase.LoadAssetAtPath(rUIPrefabLinkAtlasPath, typeof(UIPrefabLinkAtlas)) as UIPrefabLinkAtlas;
                if (rUIPrefabLinkAtlas == null)
                {
                    rUIPrefabLinkAtlas = ScriptableObject.CreateInstance<UIPrefabLinkAtlas>();
                    AssetDatabase.CreateAsset(rUIPrefabLinkAtlas, rUIPrefabLinkAtlasPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    rUIPrefabLinkAtlas = AssetDatabase.LoadAssetAtPath(rUIPrefabLinkAtlasPath, typeof(UIPrefabLinkAtlas)) as UIPrefabLinkAtlas;
                }
                if (rUIPrefabLinkAtlas.LinkAtlasList == null)
                    rUIPrefabLinkAtlas.LinkAtlasList = new List<UILinkAtlas>();
                rUIPrefabLinkAtlas.LinkAtlasList.Clear();

                var rUIPrefabDirInfo = new DirectoryInfo(UIPrefabPath);
                var rAllPrefabDirInfos = rUIPrefabDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < rAllPrefabDirInfos.Length; i++)
                {
                    var rAllFileInfos = rAllPrefabDirInfos[i].GetFiles("*.prefab", SearchOption.AllDirectories);
                    for (int j = 0; j < rAllFileInfos.Length; j++)
                    {
                        var rUILinkAtlas = new UILinkAtlas();
                        rUILinkAtlas.PrefabName = Path.GetFileNameWithoutExtension(rAllFileInfos[j].FullName);
                        rUILinkAtlas.LinkAtlas = new List<string>();
                        rUILinkAtlas.LinkAtlas.Add(TexAtlasABPath + rAllPrefabDirInfos[i].Name.ToLower() + ".ab");
                        rUIPrefabLinkAtlas.LinkAtlasList.Add(rUILinkAtlas);
                    }
                }
                EditorUtility.SetDirty(rUIPrefabLinkAtlas);
                AssetDatabase.SaveAssets();
            }
        }

        private static void GenerateSpriteAtlas(string rRootDir, string rSpriteDir, string rSpriteAtlasRootDir)
        {
            string rAtlasRootDir = rRootDir + rSpriteDir;
            string[] rDirs = Directory.GetDirectories(rAtlasRootDir);
            for (int i = 0; i < rDirs.Length; ++i)
            {
                string rName = Path.GetFileNameWithoutExtension(rDirs[i]);
                GenerateSpriteAtlas(rRootDir, rSpriteDir, rName, rSpriteAtlasRootDir);
            }
        }

        private static void GenerateSpriteAtlas(string rRootDir, string rSpriteDir, string rName, string rSpriteAtlasRootDir)
        {
            string rSpriteAtlasFilePath = rSpriteAtlasRootDir + rSpriteDir + "_" + rName + ".spriteatlas";
            SpriteAtlas rSpriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(rSpriteAtlasFilePath);
            if (rSpriteAtlas == null)
            {
                rSpriteAtlas = new SpriteAtlas();
                SpriteAtlasPackingSettings rPackingSetting = rSpriteAtlas.GetPackingSettings();
                rPackingSetting.enableRotation = false;
                rPackingSetting.enableTightPacking = false;
                rPackingSetting.padding = 2;
                rSpriteAtlas.SetPackingSettings(rPackingSetting);

                AssetDatabase.CreateAsset(rSpriteAtlas, rSpriteAtlasFilePath);
                rSpriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(rSpriteAtlasFilePath);
            }

            SetPlatformSettings(rSpriteAtlas, "Standalone");
            SetPlatformSettings(rSpriteAtlas, "Android");
            SetPlatformSettings(rSpriteAtlas, "iPhone");

            string rAtlasDirPath = rRootDir + rSpriteDir + "/" + rName;
            UnityEngine.Object[] rPackables = SpriteAtlasExtensions.GetPackables(rSpriteAtlas);
            if (rPackables == null || rPackables.Length == 0)
            {
                rPackables = new UnityEngine.Object[1];
                rPackables[0] = AssetDatabase.LoadAssetAtPath(rAtlasDirPath, typeof(UnityEngine.Object));
                SpriteAtlasExtensions.Add(rSpriteAtlas, rPackables);
            }
        }

        private static void SetPlatformSettings(SpriteAtlas rSpriteAtlas, string rPlatform)
        {
            var rSetting = rSpriteAtlas.GetPlatformSettings(rPlatform);
            rSetting.overridden = true;
            rSetting.textureCompression = TextureImporterCompression.CompressedHQ;
            if (rPlatform == "Standalone")
            {
                rSetting.format = TextureImporterFormat.DXT5;
            }
            else
            {
                rSetting.format = TextureImporterFormat.ASTC_4x4;
            }
            rSpriteAtlas.SetPlatformSettings(rSetting);
        }
    }
}
