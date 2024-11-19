using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Knight.Core
{
    /// <summary>
    /// AssetBundle 模拟类型
    /// 带Mask的枚举类型，可以用于标记
    /// </summary>
    [System.Flags]
    public enum ABSimuluateType
    {
        Scene = 1 << 0,
        Avatar = 1 << 1,
        Effect = 1 << 2,
        Script = 1 << 3,
        GUI = 1 << 4,
        Sound = 1 << 5,
        Config = 1 << 6,
    }

    public interface IAssetLoader
    {
        public void Initialize(string rSimulateConfigPath, string rABBuilderConfigPath);
        public void Update();
        public bool IsDevelopMode { get; }
        public bool IsHotfixDebugMode { get; }
        public bool IsSimulate(ABSimuluateType rType);
        bool ExistsAssetBundle(string rABName, bool bIsSimulate);
        bool ExistsAsset(string rABName, string rAssetName, bool bIsSimulate);
        public AssetLoaderRequest<T> LoadAssetAsync<T>(string rAssetPath, string rAssetName, bool bIsSimulate) where T : Object;
        public AssetLoaderRequest<T> LoadAllAssetAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object;
        public AssetLoaderRequest<T> LoadSceneAsync<T>(string rAssetPath, string rAssetName, LoadSceneMode rSceneMode, bool bIsSimulate) where T : Object;
        public AssetLoaderRequest<T> LoadAllSceneAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object;
        public void Unload<T>(AssetLoaderRequest<T> rRequest) where T : Object;
        public void AddGlobalABEntries(List<string> rGlobalABEntries);
        public void RemoveGlobalABEntries(List<string> rGlobalABEntries);
        public void ForceUnloadAllAssets();
        public string GetLoadVersionABPath(string rABPath);
    }

    public class AssetLoader
    {
        public static IAssetLoader Instance;
    }
}
