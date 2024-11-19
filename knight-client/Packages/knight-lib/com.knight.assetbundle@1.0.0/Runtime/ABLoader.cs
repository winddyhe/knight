using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Knight.Framework.Assetbundle
{
    public class ABLoader
    {
        private ABSimulateConfig mConfig;
        private ABEditorAssetDatabase mAssetDatabase;

        private int mIsLoadingRefCount = 0;
        private HashSet<ABLoadEntry> mCheckUnloadABList = new HashSet<ABLoadEntry>();
        private float mCurTime;
        private bool mIsInitialized = false;

        public bool IsDevelopMode => this.mConfig.IsDevelopMode;
        public bool IsHotfixDebugMode => this.mConfig.IsHotfixDebugMode;

        public void Initialize(string rSimulateConfigPath, string rABBuilderConfigPath)
        {
#if UNITY_EDITOR
            this.mConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<ABSimulateConfig>(rSimulateConfigPath);
#else
            this.mConfig = ScriptableObject.CreateInstance<ABSimulateConfig>();
#endif
            this.mAssetDatabase = new ABEditorAssetDatabase();
            this.mAssetDatabase.Initialize(rABBuilderConfigPath);

            Knight.Core.LogManager.LogFormat("IsHotfixDebugModeKey: {0}", this.IsHotfixDebugMode);

            this.mIsInitialized = true;
            this.mCurTime = 0.0f;
        }

        public void Update()
        {
            if (!this.mIsInitialized)
            {
                return;
            }
            if (this.mCurTime >= 1.0f)
            {
                this.mCurTime = 0.0f;
                if (this.mIsLoadingRefCount == 0)
                {
                    this.ForceUnloadAllAssets();
                }
                return;
            }
            this.mCurTime += Time.deltaTime;
        }

        public bool IsSimulate(ABSimuluateType rType)
        {
            return this.mConfig.IsDevelopMode && this.mConfig.SimulateType.HasFlag(rType);
        }

        public void Unload<T>(AssetLoaderRequest<T> rRequest) where T : Object
        {
            // 开发者模式，不卸载，直接返回
            if (AssetLoader.Instance.IsDevelopMode) return;

            // 如果已经被卸载了，那么就直接返回
            if (rRequest == null || rRequest.IsUnload) return;
            rRequest.IsUnload = true;

            if (!ABLoadVersion.Instance.TryGetValue(rRequest.ABPath, out var rAssetLoadEntry))
            {
#if UNITY_EDITOR
                Knight.Core.LogManager.LogWarningFormat("---Can not find assetbundle: -- {0}", rRequest.ABPath);
#endif
                return;
            }
            // 得到该资源的所有依赖项 减引用计数
            var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
            for (int i = 0; i < rABAllDependenceEntries.Count; i++)
            {
                var rABAllDependenceEntrie = rABAllDependenceEntries[i];
                rABAllDependenceEntrie.RefCount--;
                if (rABAllDependenceEntrie.RefCount < 0)
                {
                    LogManager.LogError($"资源卸载错误，引用计数异常 RefCoung:{rABAllDependenceEntrie.RefCount} ABName:{rABAllDependenceEntrie.ABName}");
                    rABAllDependenceEntrie.RefCount = 0;
                }
                if (rABAllDependenceEntrie.RefCount == 0)
                {
                    this.mCheckUnloadABList.Add(rABAllDependenceEntrie);
                }
            }
        }

        public void ForceUnloadAllAssets()
        {
            if (this.mCheckUnloadABList == null) return;

            bool bUnloadRes = false;
            foreach (var rABLoadEntry in this.mCheckUnloadABList)
            {
                bUnloadRes |= this.AutoCheckUnloadAsset(rABLoadEntry);
            }
            this.mCheckUnloadABList.Clear();
        }

        public void AddGlobalABEntries(List<string> rGlobalABEntries)
        {
            for (int i = 0; i < rGlobalABEntries.Count; i++)
            {
                if (ABLoadVersion.Instance.TryGetValue(rGlobalABEntries[i], out var rAssetLoadEntry))
                {
                    var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
                    for (int j = 0; j < rABAllDependenceEntries.Count; j++)
                    {
                        rABAllDependenceEntries[j].IsLock = true;
                    }
                }
            }
        }

        public void RemoveGlobalABEntries(List<string> rGlobalABEntries)
        {
            for (int i = 0; i < rGlobalABEntries.Count; i++)
            {
                if (ABLoadVersion.Instance.TryGetValue(rGlobalABEntries[i], out var rAssetLoadEntry))
                {
                    var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
                    for (int j = 0; j < rABAllDependenceEntries.Count; j++)
                    {
                        rABAllDependenceEntries[j].IsLock = false;
                    }
                }
            }
        }

        public bool ExistsAssetBundle(string rABName, bool bIsSimulate)
        {
            if (!bIsSimulate)
            {
                return ABLoadVersion.Instance.TryGetValue(rABName, out var __);
            }
            else
            {
                var rAssetPaths = this.EditorGetAssetPaths(rABName);
                return rAssetPaths != null && rAssetPaths.Count > 0;
            }
        }

        public bool ExistsAsset(string rABName, string rAssetName, bool bIsSimulate)
        {
            if (!bIsSimulate)
            {
                if (ABLoadVersion.Instance.TryGetValue(rABName, out var rAssetLoadEntry))
                {
                    return rAssetLoadEntry.AssetList.Contains(rAssetName);
                }
                return false;
            }
            else
            {
                var rAssetPath = this.EditorGetAssetPaths(rABName, rAssetName);
                return !string.IsNullOrEmpty(rAssetPath);
            }
        }

        public async UniTask LoadAssetAsync<T>(AssetLoaderRequest<T> rRequest) where T : Object
        {
            this.mIsLoadingRefCount++;

            ABLoadEntry rAssetLoadEntry = null;
            if (!rRequest.IsSimulate)
            {
                if (!ABLoadVersion.Instance.TryGetValue(rRequest.ABPath, out rAssetLoadEntry))
                {
                    LogManager.LogWarningFormat("---Can not find assetbundle: -- {0}", rRequest.ABPath);
                    rRequest.Asset = null;
                    this.mIsLoadingRefCount--;
                    rRequest.Complete();
                    return;
                }
            }
            else
            {
                rAssetLoadEntry = new ABLoadEntry()
                {
                    ABName = rRequest.ABPath,
                    ABPath = ABLoadVersion.Instance.GetABPathWithSpace(ABLoadSpace.Streaming, rRequest.ABPath),
                    Dependencies = new List<string>(),
                };
            }

            // 得到该资源的所有依赖项
            var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
            for (int i = 0; i < rABAllDependenceEntries.Count; i++)
            {
                rABAllDependenceEntries[i].RefCount++;
            }
            
            // 加载单项依赖资源
            for (int i = 0; i < rABAllDependenceEntries.Count - 1; i++)
            {
                // 构建依赖项的Request
                var rDependenceLoaderRequest = new AssetLoaderRequest<T>(rABAllDependenceEntries[i].ABPath, string.Empty, true, rRequest.IsSimulate, true, null);
                if (!rABAllDependenceEntries[i].IsLoadCompleted)
                {
                    await this.LoadAssetAsync_OneEntry<T>(rDependenceLoaderRequest, rABAllDependenceEntries[i]);
                }
            }
            // 加载主资源
            await this.LoadAssetAsync_OneEntry<T>(rRequest, rABAllDependenceEntries[rABAllDependenceEntries.Count - 1]);
            
            this.mIsLoadingRefCount--;
            rRequest.Complete();
        }


        private async UniTask LoadAssetAsync_OneEntry<T>(AssetLoaderRequest<T> rRequest, ABLoadEntry rABLoadEntry) where T : Object
        {
            // 确认未加载完成并且正在被加载、一直等待其加载完成
            while (rABLoadEntry.IsLoading && !rABLoadEntry.IsLoadCompleted)
            {
                // 如果两个都为false，则断开协程停下来
                if (rABLoadEntry.RefCount == 0)
                {
                    return;
                }
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }

            // 从缓存的Assetbundle里面加载资源
            rABLoadEntry.IsLoading = true;
            rABLoadEntry.IsLoadCompleted = false;
            if (!rRequest.IsDependence || rABLoadEntry.CacheAsset == null)
            {
                await this.LoadAssetObjectAsync(rRequest, rABLoadEntry);
            }
            rABLoadEntry.IsLoading = false;
            rABLoadEntry.IsLoadCompleted = true;

            // 如果判断此时的RefCount为0的话，那么就unload掉该项资源
            this.AutoCheckUnloadAsset(rABLoadEntry);
        }

        private async UniTask LoadAssetObjectAsync<T>(AssetLoaderRequest<T> rRequest, ABLoadEntry rAssetLoadEntry) where T : Object
        {
            string rAssetLoadUrl = rAssetLoadEntry.ABPath;
            if (rRequest.IsSimulate)
            {
                //Knight.Core.LogManager.Log("---Simulate Load ab: " + rAssetLoadUrl);
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(rRequest.AssetName) && !rRequest.IsScene)
                {
                    if (!rRequest.IsAllAssets)
                    {
                        string rAssetPath = this.EditorGetAssetPaths(rAssetLoadEntry.ABName, rRequest.AssetName);
                        if (string.IsNullOrEmpty(rAssetPath))
                        {
                            Knight.Core.LogManager.LogWarning("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            return;
                        }
                        Object rTargetAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(rAssetPath);
                        rRequest.Asset = rTargetAsset as T;
                    }
                    else
                    {
                        var rAssetPaths = this.EditorGetAssetPaths(rAssetLoadEntry.ABName);
                        if (rAssetPaths == null || rAssetPaths.Count == 0)
                        {
                            Knight.Core.LogManager.LogWarning("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            return;
                        }
                        rRequest.AllAssets = new T[rAssetPaths.Count];
                        for (int i = 0; i < rAssetPaths.Count; i++)
                        {
                            Object rAssetObj = UnityEditor.AssetDatabase.LoadAssetAtPath(rAssetPaths[i], typeof(T));
                            if (rAssetObj != null)
                                rRequest.AllAssets[i] = rAssetObj as T;
                        }
                    }
                }
                else
                {
                    if (!rRequest.IsAllAssets)
                    {
                        string rAssetPath = this.EditorGetAssetPaths(rAssetLoadEntry.ABName, rRequest.AssetName);
                        if (rAssetPath == null)
                        {
                            Knight.Core.LogManager.LogWarning("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            return;
                        }

                        await UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(
                            rAssetPath, new LoadSceneParameters() { loadSceneMode = rRequest.SceneMode });

                        string rSceneName = rRequest.AssetName;
                        rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                        SceneManager.SetActiveScene(rRequest.Scene);
                    }
                    else
                    {
                        var rAssetPaths = this.EditorGetAssetPaths(rAssetLoadEntry.ABName);
                        if (rAssetPaths == null)
                            return;

                        rRequest.AllScenes = new List<Scene>();
                        for (int i = 0; i < rAssetPaths.Count; i++)
                        {
                            await UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(
                                rAssetPaths[i], new LoadSceneParameters() { loadSceneMode = rRequest.SceneMode });

                            string rSceneName = Path.GetFileNameWithoutExtension(rAssetPaths[i]);
                            var rScene = SceneManager.GetSceneByName(rSceneName);
                            var rSceneObjs = rScene.GetRootGameObjects();
                            for (int j = 0; j < rSceneObjs.Length; j++)
                            {
                                rSceneObjs[j].SetActive(false);
                            }
                            rRequest.AllScenes.Add(rScene);
                        }
                    }
                }
#endif
            }
            else
            {
                // 如果是一个直接的资源，将资源的对象取出来
                if (rAssetLoadEntry.CacheAsset == null)
                {
                    //Knight.Core.LogManager.Log("---Real Load ab: " + rAssetLoadUrl);
                    uint nABBuildKey = 982310;
                    var nOffsetLength = nABBuildKey % 255 ^ rAssetLoadEntry.MD5[4] + 10;
                    var rABCreateRequest = AssetBundle.LoadFromFileAsync(rAssetLoadEntry.ABPath, 0, (ulong)nOffsetLength);
                    await rABCreateRequest;
                    rAssetLoadEntry.CacheAsset = rABCreateRequest.assetBundle;
                }

                // 加载Object
                if (!string.IsNullOrEmpty(rRequest.AssetName))
                {
                    if (!rRequest.IsScene)
                    {
                        if (!rRequest.IsAllAssets)
                        {
                            var rABRequest = rAssetLoadEntry.CacheAsset.LoadAssetAsync(rRequest.AssetName, typeof(T));
                            await rABRequest;
                            rRequest.Asset = rABRequest.asset as T;
                        }
                        else
                        {
                            // 依赖项不用LoadAllAssets
                            if (!rRequest.IsDependence)
                            {
                                var rABRequest = rAssetLoadEntry.CacheAsset.LoadAllAssetsAsync();
                                await rABRequest;
                                rRequest.AllAssets = new T[rABRequest.allAssets.Length];
                                for (int i = 0; i < rABRequest.allAssets.Length; i++)
                                {
                                    rRequest.AllAssets[i] = rABRequest.allAssets[i] as T;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!rRequest.IsAllAssets)
                        {
                            // 如果不是场景的依赖项
                            rAssetLoadEntry.CacheAsset.GetAllScenePaths();

                            string rSceneName = Path.GetFileNameWithoutExtension(rRequest.AssetName);
                            var rSceneLoadRequest = SceneManager.LoadSceneAsync(rSceneName, rRequest.SceneMode);
                            await rSceneLoadRequest;

                            rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                            SceneManager.SetActiveScene(rRequest.Scene);
                        }
                        else
                        {
                            var rScenePaths = rAssetLoadEntry.CacheAsset.GetAllScenePaths();

                            rRequest.AllScenes = new List<Scene>();
                            for (int i = 0; i < rScenePaths.Length; i++)
                            {
                                string rSceneName = Path.GetFileNameWithoutExtension(rScenePaths[i]);
                                var rSceneLoadRequest = SceneManager.LoadSceneAsync(rSceneName, rRequest.SceneMode);
                                await rSceneLoadRequest;

                                var rScene = SceneManager.GetSceneByName(rSceneName);
                                var rSceneObjs = rScene.GetRootGameObjects();
                                for (int j = 0; j < rSceneObjs.Length; j++)
                                {
                                    rSceneObjs[j].SetActive(false);
                                }
                                rRequest.AllScenes.Add(rScene);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 此接口获取到的值包含AB包本身，容易理解错误
        /// </summary>
        private List<ABLoadEntry> GetABEntryAllDependencies(ABLoadEntry rABLoadEntry)
        {
            var rABAllDependenceEntries = new List<ABLoadEntry>();
            //发现情况：字体文件fzcthjt依赖fzjthjt,fzjthjt反过来也依赖fzcthjt
            //不同unity版本会有不一样的依赖计算行为
            var preventCyclicRefs = new Dictionary<string, bool>();
            this.GetABEntryAllDependencies(rABLoadEntry, ref rABAllDependenceEntries, preventCyclicRefs);
            return rABAllDependenceEntries;
        }

        private void GetABEntryAllDependencies(ABLoadEntry rABLoadEntry, ref List<ABLoadEntry> rABAllDependenceEntries, Dictionary<string, bool> rPreventCyclicRefs)
        {
            for (int i = 0; i < rABLoadEntry.Dependencies.Count; i++)
            {
                var n = rABLoadEntry.Dependencies[i];
                if (rPreventCyclicRefs.ContainsKey(n))
                {
                    continue;
                }
                rPreventCyclicRefs.Add(n, true);

                if (ABLoadVersion.Instance.TryGetValueMD5Path(rABLoadEntry.Dependencies[i], out var rDependenceEntry))
                {
                    this.GetABEntryAllDependencies(rDependenceEntry, ref rABAllDependenceEntries, rPreventCyclicRefs);
                }
            }
            rABAllDependenceEntries.Add(rABLoadEntry);
        }

        private string EditorGetAssetPaths(string rABName, string rAssetName)
        {
            if (this.mAssetDatabase == null) return null;
            return this.mAssetDatabase.EditorGetAssetPaths(rABName, rAssetName);
        }

        private List<string> EditorGetAssetPaths(string rABName)
        {
            if (this.mAssetDatabase == null) return null;
            return this.mAssetDatabase.EditorGetAssetPaths(rABName);
        }

        /// <summary>
        /// 自动检测引用计数并释放资源
        /// </summary>
        private bool AutoCheckUnloadAsset(ABLoadEntry rABLoadEntry)
        {
            // 如果引用计数为0，并且没有资源没有被引用锁定，那么就真正删除资源
            if (rABLoadEntry.RefCount == 0 && !rABLoadEntry.IsLock)
            {
                if (rABLoadEntry.CacheAsset != null)
                {
                    //Knight.Core.LogManager.LogFormat("---Auto Real unload assetbundle: {0}", rAssetLoadEntry.ABName);
                    rABLoadEntry.CacheAsset.Unload(true);
                    rABLoadEntry.CacheAsset = null;
                    rABLoadEntry.IsLoadCompleted = false;
                    rABLoadEntry.IsLoading = false;
                    return true;
                }
            }
            return false;
        }
    }
}
