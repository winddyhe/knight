using Cysharp.Threading.Tasks;
using Knight.Core;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class UIAtlasSpriteCache
    {
        public HashSet<AssetLoaderRequest<Sprite>> CachedSprites = new HashSet<AssetLoaderRequest<Sprite>>();
        public Dictionary<string, HashSet<AssetLoaderRequest<Sprite>>> ABSpriteDict = new Dictionary<string, HashSet<AssetLoaderRequest<Sprite>>>();

        public HashSet<AssetLoaderRequest<Texture2D>> CachedTextures = new HashSet<AssetLoaderRequest<Texture2D>>();
        public Dictionary<string, HashSet<AssetLoaderRequest<Texture2D>>> ABTextureDict = new Dictionary<string, HashSet<AssetLoaderRequest<Texture2D>>>();
    }

    /// <summary>
    /// 只有Icon和FullBG支持多语言，如果Atlas中有图片需要支持多语言，那么需要将其移到Icon中。
    /// </summary>
    public class UIAtlasManager : TSingleton<UIAtlasManager>
    {
        private UIAtlasSpriteCache mAtlasSpriteCache;

        private Dictionary<string, int> mIconLinkDict;
        private Dictionary<string, int> mPrefabLinkAtlasDict;
        private Dictionary<string, int> mFullBGAtlasDict;
        private Dictionary<string, string> mFullBGAtlasNameDict;
        private Dictionary<int, string> mABPathDict;

        public string mCommonABName = "game/gui/textures/atlas/common.ab";
        public string mFullBGABNamePrefix = "game/gui/textures/fullbg/";

        public string IconsRoot_Editor = "Assets/Game/GameAsset/GUI/Textures/Icons";
        public string FullBGRoot_Editor = "Assets/Game/GameAsset/GUI/Textures/FullBG";

        private UIAtlasManager()
        {
        }

        public async UniTask Initialize(string rAtlasConfigABName)
        {
            this.mAtlasSpriteCache = new UIAtlasSpriteCache();

            bool bIsSimulate = AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI);
            if (!bIsSimulate)
            {
                await this.Initialize_Assetbundle(rAtlasConfigABName);
            }
            else
            {
#if UNITY_EDITOR
                this.Initialize_Editor();
#endif
            }
        }

        private async UniTask Initialize_Assetbundle(string rAtlasConfigABName)
        {
            // 处理Icon和FullGB 因为他们名字唯一，所有可以通过字典来进行映射
            var rAtlasLoadRequest = AssetLoader.Instance.LoadAssetAsync<UIAtlasIconData>(rAtlasConfigABName, "AtlasIconData", false);
            await rAtlasLoadRequest.Task();
            if (rAtlasLoadRequest.Asset == null)
            {
                AssetLoader.Instance.Unload(rAtlasLoadRequest);
                return;
            }
            var rAtlasIconData = rAtlasLoadRequest.Asset as UIAtlasIconData;
            this.mIconLinkDict = new Dictionary<string, int>();
            this.mFullBGAtlasDict = new Dictionary<string, int>();
            this.mFullBGAtlasNameDict = new Dictionary<string, string>();
            this.mABPathDict = new Dictionary<int, string>();
            int nCount = 0;
            for (int i = 0; i < rAtlasIconData.AtlasIconLinks.Count; i++)
            {
                if (!rAtlasIconData.AtlasIconLinks[i].ABName.Equals("game/gui/textures/icons/fullbg.ab"))
                {
                    for (int j = 0; j < rAtlasIconData.AtlasIconLinks[i].IconNames.Count; j++)
                    {
                        var rIconName = rAtlasIconData.AtlasIconLinks[i].IconNames[j];
                        var rIconABName = rAtlasIconData.AtlasIconLinks[i].ABName;
                        var bIsResult = false;
                        var nIndex = nCount;
                        foreach (var rPair in this.mABPathDict)
                        {
                            if (rPair.Value.Equals(rIconABName))
                            {
                                bIsResult = true;
                                nIndex = rPair.Key;
                                break;
                            }
                        }
                        if (bIsResult)
                        {
                            this.mIconLinkDict.Add(rIconName, nIndex);
                        }
                        else
                        {
                            this.mABPathDict.Add(nIndex, rIconABName);
                            this.mIconLinkDict.Add(rIconName, nIndex);
                            nCount++;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < rAtlasIconData.AtlasIconLinks[i].IconNames.Count; j++)
                    {
                        var rFullBGName = rAtlasIconData.AtlasIconLinks[i].IconNames[j];
                        var rParam = Regex.Match(rFullBGName, @"\(.*?\)");
                        if (rParam.Success)
                        {
                            var rRealName = rFullBGName.Replace(rParam.Value, string.Empty);
                            if (this.mFullBGAtlasNameDict.ContainsKey(rRealName))
                            {
                                LogManager.LogError($"FullBG中名字为{rRealName}的有多个：{rFullBGName}与{this.mFullBGAtlasNameDict[rRealName]}，请改名！！！");
                            }
                            else
                            {
                                this.mFullBGAtlasNameDict.Add(rRealName, rFullBGName);
                            }
                        }
                        var rFullBGABName = this.mFullBGABNamePrefix + rFullBGName.ToLower() + ".ab";
                        var nIndex = nCount;
                        this.mFullBGAtlasDict.Add(rFullBGName, nIndex);
                        this.mABPathDict.Add(nIndex, rFullBGABName);
                        nCount++;
                    }
                }
            }
            AssetLoader.Instance.Unload(rAtlasLoadRequest);

            // 处理PrefabLink
            var rPrefabLoadRequest = AssetLoader.Instance.LoadAssetAsync<UIPrefabLinkAtlas>(rAtlasConfigABName, "PrefabLinkAtlas", false);
            await rPrefabLoadRequest.Task();
            if (rPrefabLoadRequest.Asset == null)
            {
                AssetLoader.Instance.Unload(rPrefabLoadRequest);
                return;
            }
            var rPrefabLinkAtlas = rPrefabLoadRequest.Asset as UIPrefabLinkAtlas;
            this.mPrefabLinkAtlasDict = new Dictionary<string, int>();
            for (int i = 0; i < rPrefabLinkAtlas.LinkAtlasList.Count; i++)
            {
                var rPrefabName = rPrefabLinkAtlas.LinkAtlasList[i].PrefabName;
                var rAtlasName = rPrefabLinkAtlas.LinkAtlasList[i].LinkAtlas[0];

                var bIsResult = false;
                var nIndex = nCount;
                foreach (var rPair in this.mABPathDict)
                {
                    if (rPair.Value.Equals(rAtlasName))
                    {
                        bIsResult = true;
                        nIndex = rPair.Key;
                        break;
                    }
                }
                if (bIsResult)
                {
                    this.mPrefabLinkAtlasDict.Add(rPrefabName, nIndex);
                }
                else
                {
                    this.mPrefabLinkAtlasDict.Add(rPrefabName, nIndex);
                    this.mABPathDict.Add(nIndex, rAtlasName);
                    nCount++;
                }
            }
            AssetLoader.Instance.Unload(rPrefabLoadRequest);
        }

#if UNITY_EDITOR
        private void Initialize_Editor()
        {
            this.mIconLinkDict = new Dictionary<string, int>();
            this.mABPathDict = new Dictionary<int, string>();
            var rGUIDs = UnityEditor.AssetDatabase.FindAssets("t:Folder", new string[] { "Assets/Game/GameAsset/GUI/Textures/Icons" });
            int nCount = 0;
            for (int i = 0; i < rGUIDs.Length; i++)
            {
                var rAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                var rDirInfo = new DirectoryInfo(rAssetPath);
                var rAllFileInfos = rDirInfo.GetFiles("*.png", SearchOption.AllDirectories);
                var rDirABName = "game/gui/textures/icons/" + rDirInfo.Name.ToLower() + ".ab";

                for (int j = 0; j < rAllFileInfos.Length; j++)
                {
                    var rIconName = Path.GetFileNameWithoutExtension(rAllFileInfos[j].Name);
                    if (!this.mIconLinkDict.ContainsKey(rIconName))
                    {
                        var bIsResult = false;
                        var nIndex = nCount;
                        foreach (var rPair in this.mABPathDict)
                        {
                            if (rPair.Value.Equals(rDirABName))
                            {
                                bIsResult = true;
                                nIndex = rPair.Key;
                                break;
                            }
                        }
                        if (bIsResult)
                        {
                            this.mIconLinkDict.Add(rIconName, nIndex);
                        }
                        else
                        {
                            this.mABPathDict.Add(nIndex, rDirABName);
                            this.mIconLinkDict.Add(rIconName, nIndex);
                            nCount++;
                        }
                    }
                    else
                    {
                        LogManager.LogError($"图标名称重复 IconName:{rIconName} FilePath:{rAllFileInfos[j].FullName}");
                    }
                }
            }

            this.mFullBGAtlasDict = new Dictionary<string, int>();
            this.mFullBGAtlasNameDict = new Dictionary<string, string>();
            rGUIDs = UnityEditor.AssetDatabase.FindAssets("t:Texture", new string[] { "Assets/Game/GameAsset/GUI/Textures/FullBG" });
            for (int i = 0; i < rGUIDs.Length; i++)
            {
                var rAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                var rFileName = Path.GetFileNameWithoutExtension(rAssetPath);
                var rParam = Regex.Match(rFileName, @"\(.*?\)");
                if (rParam.Success)
                {
                    var rRealName = rFileName.Replace(rParam.Value, string.Empty);
                    if (this.mFullBGAtlasNameDict.ContainsKey(rRealName))
                    {
                        LogManager.LogError($"FullBG中名字为{rRealName}的有多个：{rFileName}与{this.mFullBGAtlasNameDict[rRealName]}，请改名！！！");
                    }
                    else
                    {
                        this.mFullBGAtlasNameDict.Add(rRealName, rFileName);
                    }
                }
                var rFileABName = "game/gui/textures/fullbg/" + rFileName.ToLower() + ".ab";
                var nIndex = nCount;
                this.mFullBGAtlasDict.Add(rFileName, nIndex);
                this.mABPathDict.Add(nIndex, rFileABName);
                nCount++;
            }

            this.mPrefabLinkAtlasDict = new Dictionary<string, int>();
            rGUIDs = UnityEditor.AssetDatabase.FindAssets("t:Folder", new string[] { "Assets/Game/GameAsset/GUI/Prefabs" });
            for (int i = 0; i < rGUIDs.Length; i++)
            {
                var rAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                var rDirInfo = new DirectoryInfo(rAssetPath);
                var rAllFileInfos = rDirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
                var rDirABName = "game/gui/textures/atlas/" + rDirInfo.Name.ToLower() + ".ab";
                for (int j = 0; j < rAllFileInfos.Length; j++)
                {
                    var rPrefabName = Path.GetFileNameWithoutExtension(rAllFileInfos[j].Name);

                    var bIsResult = false;
                    var nIndex = nCount;
                    foreach (var rPair in this.mABPathDict)
                    {
                        if (rPair.Value.Equals(rDirABName))
                        {
                            bIsResult = true;
                            nIndex = rPair.Key;
                            break;
                        }
                    }
                    if (bIsResult)
                    {
                        this.mPrefabLinkAtlasDict.Add(rPrefabName, nIndex);
                    }
                    else
                    {
                        this.mPrefabLinkAtlasDict.Add(rPrefabName, nIndex);
                        this.mABPathDict.Add(nIndex, rDirABName);
                        nCount++;
                    }
                }
            }
        }
#endif

        public AssetLoaderRequest<Sprite> LoadMultiIcon(string rSpriteName)
        {
            if (string.IsNullOrEmpty(rSpriteName)) return null;

            var rMultiLanguageSpriteName = LocalizationTool.Instance.GetLocalizationSuffix(rSpriteName);
            if (!this.mIconLinkDict.TryGetValue(rMultiLanguageSpriteName, out var rAtlasABPathIndex))
            {
                LogManager.LogError($"Cannot find icon {rMultiLanguageSpriteName}, use orginal icon replace.");
                rMultiLanguageSpriteName = rSpriteName;
            }
            if (!this.mABPathDict.TryGetValue(rAtlasABPathIndex, out var rAtlasABPath))
            {
                LogManager.LogError($"Cannot find icon {rMultiLanguageSpriteName}, use orginal icon replace.");
                rMultiLanguageSpriteName = rSpriteName;
            }
            return this.LoadIcon(rMultiLanguageSpriteName);
        }

        public AssetLoaderRequest<Sprite> LoadIcon(string rSpriteName)
        {
            if (string.IsNullOrEmpty(rSpriteName)) return null;

            if (!this.mIconLinkDict.TryGetValue(rSpriteName, out var rAtlasABPathIndex))
            {
                return null;
            }
            if (!this.mABPathDict.TryGetValue(rAtlasABPathIndex, out var rAtlasABPath))
            {
                return null;
            }

            if (!this.mAtlasSpriteCache.ABSpriteDict.TryGetValue(rAtlasABPath, out var rSpriteRequestSet))
            {
                rSpriteRequestSet = new HashSet<AssetLoaderRequest<Sprite>>();
                this.mAtlasSpriteCache.ABSpriteDict.Add(rAtlasABPath, rSpriteRequestSet);
            }
            bool bIsSimulate = AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI);
            var rABLoadRequest = AssetLoader.Instance.LoadAssetAsync<Sprite>(rAtlasABPath, rSpriteName, bIsSimulate);
            rSpriteRequestSet.Add(rABLoadRequest);
            this.mAtlasSpriteCache.CachedSprites.Add(rABLoadRequest);
            return rABLoadRequest;
        }

        public AssetLoaderRequest<Sprite> LoadSprite(string rSpriteName, string rPrefabName)
        {
            if (string.IsNullOrEmpty(rSpriteName)) return null;

            string rAtlasABPath = this.GetAtlasABPath(rSpriteName, rPrefabName);
            if (string.IsNullOrEmpty(rAtlasABPath))
            {
                LogManager.LogError($"Cannot find {rSpriteName} Prefab:{rPrefabName} image in any atlas.");
                return null;
            }

            if (!this.mAtlasSpriteCache.ABSpriteDict.TryGetValue(rAtlasABPath, out var rSpriteRequestSet))
            {
                rSpriteRequestSet = new HashSet<AssetLoaderRequest<Sprite>>();
                this.mAtlasSpriteCache.ABSpriteDict.Add(rAtlasABPath, rSpriteRequestSet);
            }
            bool bIsSimulate = AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI);
            var rABLoadRequest = AssetLoader.Instance.LoadAssetAsync<Sprite>(rAtlasABPath, rSpriteName, bIsSimulate);
            this.mAtlasSpriteCache.CachedSprites.Add(rABLoadRequest);
            rSpriteRequestSet.Add(rABLoadRequest);
            return rABLoadRequest;
        }

        public void UnloadSprite(AssetLoaderRequest<Sprite> rLoaderRequest)
        {
            if (rLoaderRequest == null) return;

            if (this.mAtlasSpriteCache.ABSpriteDict.TryGetValue(rLoaderRequest.ABPath, out var rSpriteRequestSet))
            {
                rSpriteRequestSet.Remove(rLoaderRequest);
            }
            this.mAtlasSpriteCache.CachedSprites.Remove(rLoaderRequest);
            AssetLoader.Instance.Unload(rLoaderRequest);
        }

        public AssetLoaderRequest<Texture2D> LoadMultiTexture(string rTextureName)
        {
            if (this.mFullBGAtlasDict == null || this.mFullBGAtlasNameDict == null)
            {
                return null;
            }

            var rMultiLanguageTextureName = LocalizationTool.Instance.GetLocalizationSuffix(rTextureName);
            if (!this.mFullBGAtlasNameDict.ContainsKey(rMultiLanguageTextureName))
            {
                LogManager.LogError($"Cannot find fullbg texture {rMultiLanguageTextureName}, use orginal texture replace.");
                rMultiLanguageTextureName = rTextureName;
            }
            return this.LoadTexture(rMultiLanguageTextureName);
        }

        public AssetLoaderRequest<Texture2D> LoadTexture(string rTextureName)
        {
            if (this.mFullBGAtlasDict == null)
            {
                return null;
            }
            if (this.mFullBGAtlasNameDict != null && this.mFullBGAtlasNameDict.TryGetValue(rTextureName, out var rTextureRealName))
            {
                rTextureName = rTextureRealName;
            }
            if (!this.mFullBGAtlasDict.TryGetValue(rTextureName, out var rFullBGABNameIndex))
            {
                return null;
            }
            if (!this.mABPathDict.TryGetValue(rFullBGABNameIndex, out var rFullBGABName))
            {
                return null;
            }

            if (!this.mAtlasSpriteCache.ABTextureDict.TryGetValue(rTextureName, out var rTextureRequestSet))
            {
                rTextureRequestSet = new HashSet<AssetLoaderRequest<Texture2D>>();
                this.mAtlasSpriteCache.ABTextureDict.Add(rTextureName, rTextureRequestSet);
            }
            var rLoadRequest = AssetLoader.Instance.LoadAssetAsync<Texture2D>(rFullBGABName, rTextureName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            this.mAtlasSpriteCache.CachedTextures.Add(rLoadRequest);
            rTextureRequestSet.Add(rLoadRequest);
            return rLoadRequest;
        }

        public void UnloadTexture(AssetLoaderRequest<Texture2D> rLoaderRequest)
        {
            if (rLoaderRequest == null) return;

            if (this.mAtlasSpriteCache.ABTextureDict.TryGetValue(rLoaderRequest.AssetName, out var rTextureRequestSet))
            {
                rTextureRequestSet.Remove(rLoaderRequest);
            }
            this.mAtlasSpriteCache.CachedTextures.Remove(rLoaderRequest);
            AssetLoader.Instance.Unload(rLoaderRequest);
        }

        private string GetAtlasABPath(string rSpriteName, string rPrefabName)
        {
            bool bIsSimulate = AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI);
            // 检查是否是Icon图标
            string rAtlasABPath = string.Empty;
            if (this.mIconLinkDict.TryGetValue(rSpriteName, out var rAtlasABPathIndex))
            {
                if (this.mABPathDict.TryGetValue(rAtlasABPathIndex, out rAtlasABPath))
                {
                    return rAtlasABPath;
                }
            }
            // 检查是否对应在哪个Atlas图集中
            if (this.mPrefabLinkAtlasDict.TryGetValue(rPrefabName, out var rPrefabAtlasABPathIndex))
            {
                if (this.mABPathDict.TryGetValue(rPrefabAtlasABPathIndex, out var rPrefabAtlasABPath))
                {
                    if (AssetLoader.Instance.ExistsAsset(rPrefabAtlasABPath, rSpriteName, bIsSimulate))
                    {
                        return rPrefabAtlasABPath;
                    }
                }
            }
            if (AssetLoader.Instance.ExistsAsset(this.mCommonABName, rSpriteName, bIsSimulate))
            {
                return this.mCommonABName;
            }
            return rAtlasABPath;
        }
    }
}
