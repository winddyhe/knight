//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Knight.Core; 
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityFx.Async;
using System;
using Object = UnityEngine.Object;

namespace Knight.Framework.AssetBundles
{
    /// <summary>
    /// 加载资源的管理类，用作资源的加载管理
    /// 采用缓存Assetbundle对象的策略，半自动的引用计数
    /// @TODO: 目前该资源加载还有一个问题，那就是场景x.unity不能依赖另一个场景x.unity，要不然会报错加载不出来
    /// </summary>
    public class ABLoader : IAssetLoader
    {
        private int mIsLoadingRefCount = 0;

        public void Initialize()
        {
            this.mIsLoadingRefCount = 0;
            CoroutineManager.Instance.Start(this.Update());
        }

        /// <summary>
        /// 每隔一秒钟检测如果ABLoader没有在加载的话，就把所有引用为0的资源包全部卸载掉
        /// </summary>
        private IEnumerator Update()
        {
            while (true)
            {
                if (this.mIsLoadingRefCount == 0)
                {
                    if (ABLoaderVersion.Instance.Entries == null) continue;

                    foreach (var rPair in ABLoaderVersion.Instance.Entries)
                    {
                        var rABLoadEntry = rPair.Value;
                        this.AutoCheckUnloadAsset(rABLoadEntry);
                    }
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        public void UnloadAsset(string rABPath)
        {
            if (ABPlatform.Instance.IsDevelopeMode()) return;

            ABLoadEntry rAssetLoadEntry = null;
            if (!ABLoaderVersion.Instance.TryGetValue(rABPath, out rAssetLoadEntry))
            {
                Debug.LogErrorFormat("---Can not find assetbundle: -- {0}", rABPath);
                return;
            }

            // 得到该资源的所有依赖项
            var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
            for (int i = 0; i < rABAllDependenceEntries.Count; i++)
            {
                rABAllDependenceEntries[i].RefCount--;
                if (rABAllDependenceEntries[i].RefCount < 0)
                    rABAllDependenceEntries[i].RefCount = 0;
            }
        }

        /// <summary>
        /// 自动检测引用计数
        /// </summary>
        private void AutoCheckUnloadAsset(ABLoadEntry rAssetLoadEntry)
        {
            if (rAssetLoadEntry.RefCount == 0)
            {
                if (rAssetLoadEntry.CacheAsset != null)
                {
                    Debug.LogFormat("---Auto Real unload assetbundle: {0}", rAssetLoadEntry.ABName);
                    rAssetLoadEntry.CacheAsset.Unload(true);
                    rAssetLoadEntry.CacheAsset = null;
                    rAssetLoadEntry.IsLoadCompleted = false;
                    rAssetLoadEntry.IsLoading = false;
                }
            }
        }

        /// <summary>
        /// 计算引用计数
        /// </summary>
        private void CalcRefCount(string rABName, int nDeltaRefCount)
        {
            ABLoadEntry rABLoadEntry = null;
            if (!ABLoaderVersion.Instance.TryGetValue(rABName, out rABLoadEntry))
            {
                return;
            }
            rABLoadEntry.RefCount += nDeltaRefCount;
            for (int i = 0; i < rABLoadEntry.ABDependNames.Length; i++)
            {
                this.CalcRefCount(rABLoadEntry.ABDependNames[i], nDeltaRefCount);
            }
        }

        private List<ABLoadEntry> GetABEntryAllDependencies(ABLoadEntry rABLoadEntry)
        {
            var rABAllDependenceEntries = new List<ABLoadEntry>();
            this.GetABEntryAllDependencies(rABLoadEntry, ref rABAllDependenceEntries);
            return rABAllDependenceEntries;
        }

        private void GetABEntryAllDependencies(ABLoadEntry rABLoadEntry, ref List<ABLoadEntry> rABAllDependenceEntries)
        {
            for (int i = 0; i < rABLoadEntry.ABDependNames.Length; i++)
            {
                ABLoadEntry rDependenceEntry = null;
                if (ABLoaderVersion.Instance.TryGetValue(rABLoadEntry.ABDependNames[i], out rDependenceEntry))
                {
                    this.GetABEntryAllDependencies(rDependenceEntry, ref rABAllDependenceEntries);
                }
            }
            rABAllDependenceEntries.Add(rABLoadEntry);
        }

        public bool IsSumilateMode_Script()
        {
            return ABPlatform.Instance.IsSumilateMode_Script();
        }

        public bool IsSumilateMode_Config()
        {
            return ABPlatform.Instance.IsSumilateMode_Config();
        }

        public bool IsSumilateMode_GUI()
        {
            return ABPlatform.Instance.IsSumilateMode_GUI();
        }

        public bool IsSumilateMode_Avatar()
        {
            return ABPlatform.Instance.IsSumilateMode_Avatar();
        }

        public bool IsSumilateMode_Scene()
        {
            return ABPlatform.Instance.IsSumilateMode_Scene();
        }

        #region async Load 异步加载
        public IAsyncOperation<AssetLoaderRequest> LoadAssetAsync(string rABName, string rAssetName, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, rAssetName, false, bIsSimulate, false);
            return rRequest.Start(this.LoadAssetAsync(rRequest));
        }

        public IAsyncOperation<AssetLoaderRequest> LoadAllAssetsAsync(string rABName, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, string.Empty, false, bIsSimulate, true);
            return rRequest.Start(this.LoadAssetAsync(rRequest));
        }

        public IAsyncOperation<AssetLoaderRequest> LoadSceneAsync(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, rAssetName, rLoadSceneMode, false, bIsSimulate);
            return rRequest.Start(this.LoadAssetAsync(rRequest));
        }

        private IEnumerator LoadAssetAsync(AssetLoaderRequest rRequest)
        {
            this.mIsLoadingRefCount++;

            ABLoadEntry rAssetLoadEntry = null;
            if (!rRequest.IsSimulate)
            {
                if (!ABLoaderVersion.Instance.TryGetValue(rRequest.Path, out rAssetLoadEntry))
                {
                    Debug.LogErrorFormat("---Can not find assetbundle: -- {0}", rRequest.Path);
                    rRequest.Asset = null;
                    this.mIsLoadingRefCount--;
                    rRequest.SetResult(rRequest);
                    yield break;
                }
            }
            else
            {
                rAssetLoadEntry = new ABLoadEntry()
                {
                    ABName = rRequest.Path,
                    ABPath = ABLoaderVersion.Instance.GetABPath_With_Space(LoaderSpace.Streaming, rRequest.Path),
                    ABDependNames = new string[0],
                };
            }

            // 得到该资源的所有依赖项
            var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
            for (int i = 0; i < rABAllDependenceEntries.Count; i++)
            {
                rABAllDependenceEntries[i].RefCount++;
            }

            for (int i = 0; i < rABAllDependenceEntries.Count - 1; i++)
            {
                // 构建依赖项的Request
                var rDependenceLoaderRequest = new AssetLoaderRequest(
                    rABAllDependenceEntries[i].ABPath, string.Empty, true, rRequest.IsSimulate, rRequest.IsLoadAllAssets);
                yield return this.LoadAssetAsync_OneEntry(rDependenceLoaderRequest, rABAllDependenceEntries[i]);
            }
            yield return this.LoadAssetAsync_OneEntry(rRequest, rABAllDependenceEntries[rABAllDependenceEntries.Count - 1]);
            this.mIsLoadingRefCount--;
            rRequest.SetResult(rRequest);
        }

        private IEnumerator LoadAssetAsync_OneEntry(AssetLoaderRequest rRequest, ABLoadEntry rABLoadEntry)
        {
            // 确认未加载完成并且正在被加载、一直等待其加载完成
            while (rABLoadEntry.IsLoading && !rABLoadEntry.IsLoadCompleted)
            {
                // 如果两个都为false，则断开协程停下来
                if (rABLoadEntry.RefCount == 0)
                {
                    yield break;
                }
                yield return 0;
            }

            // 从缓存的Assetbundle里面加载资源
            rABLoadEntry.IsLoading = true;
            rABLoadEntry.IsLoadCompleted = false;
            yield return LoadAssetObjectAsync(rRequest, rABLoadEntry);
            rABLoadEntry.IsLoading = false;
            rABLoadEntry.IsLoadCompleted = true;

            // 如果判断此时的RefCount为0的话，那么就unload掉该项资源
            this.AutoCheckUnloadAsset(rABLoadEntry);
        }

        private IEnumerator LoadAssetObjectAsync(AssetLoaderRequest rRequest, ABLoadEntry rAssetLoadEntry)
        {
            string rAssetLoadUrl = rAssetLoadEntry.ABPath;
            if (rRequest.IsSimulate)
            {
                Debug.Log("---Simulate Load ab: " + rAssetLoadUrl);
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(rRequest.AssetName) && !rRequest.IsScene)
                {
                    if (!rRequest.IsLoadAllAssets)
                    {
                        string[] rAssetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(rAssetLoadEntry.ABName, rRequest.AssetName);
                        if (rAssetPaths.Length == 0)
                        {
                            Debug.LogError("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            yield break;
                        }
                        Object rTargetAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(rAssetPaths[0]);
                        rRequest.Asset = rTargetAsset;
                    }
                    else
                    {
                        string[] rAssetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(rAssetLoadEntry.ABName);
                        if (rAssetPaths.Length == 0)
                        {
                            Debug.LogError("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            yield break;
                        }
                        rRequest.AllAssets = new Object[rAssetPaths.Length];
                        for (int i = 0; i < rAssetPaths.Length; i++)
                        {
                            Object rAssetObj = UnityEditor.AssetDatabase.LoadAssetAtPath(rAssetPaths[i], typeof(Object));
                            if (rAssetObj != null)
                                rRequest.AllAssets[i] = rAssetObj;
                        }
                    }
                }
                else
                {
                    yield return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(
                        rRequest.AssetName, new LoadSceneParameters() { loadSceneMode = rRequest.SceneMode });

                    string rSceneName = Path.GetFileNameWithoutExtension(rRequest.AssetName);
                    rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                    SceneManager.SetActiveScene(rRequest.Scene);
                }
#endif
            }
            else
            {
                // 如果是一个直接的资源，将资源的对象取出来
                if (rAssetLoadEntry.CacheAsset == null)
                {
                    Debug.Log("---Real Load ab: " + rAssetLoadUrl);
                    var rABCreateRequest = AssetBundle.LoadFromFileAsync(rAssetLoadUrl);
                    yield return rABCreateRequest;
                    rAssetLoadEntry.CacheAsset = rABCreateRequest.assetBundle;
                }
                else
                {
                    Debug.Log("---Load asset: " + rAssetLoadUrl);
                }

                // 加载Object
                if (!string.IsNullOrEmpty(rRequest.AssetName))
                {
                    if (!rRequest.IsScene)
                    {
                        if (!rRequest.IsLoadAllAssets)
                        {
                            var rABRequest = rAssetLoadEntry.CacheAsset.LoadAssetAsync(rRequest.AssetName);
                            yield return rABRequest;
                            rRequest.Asset = rABRequest.asset;
                        }
                        else
                        {
                            yield return LoadAllAssets_ByAssetbundle_Async(rRequest, rAssetLoadEntry.CacheAsset);
                        }
                    }
                    else
                    {
                        // 如果不是场景的依赖项
                        rAssetLoadEntry.CacheAsset.GetAllScenePaths();

                        string rSceneName = Path.GetFileNameWithoutExtension(rRequest.AssetName);
                        var rSceneLoadRequest = SceneManager.LoadSceneAsync(rSceneName, rRequest.SceneMode);
                        yield return rSceneLoadRequest;

                        rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                        SceneManager.SetActiveScene(rRequest.Scene);
                    }
                }
            }
        }

        private IEnumerator LoadAllAssets_ByAssetbundle_Async(AssetLoaderRequest rRequest, AssetBundle rAssetbundle)
        {
            var rAllAssetsRequest = rAssetbundle.LoadAllAssetsAsync();
            yield return rAllAssetsRequest;
            rRequest.AllAssets = rAllAssetsRequest.allAssets;
        }
        #endregion

        #region sync Load 同步加载
        public AssetLoaderRequest LoadAsset(string rABName, string rAssetName, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, rAssetName, false, bIsSimulate, false);
            this.LoadAssetSync(rRequest);
            return rRequest;
        }

        public AssetLoaderRequest LoadAsset(string rABName, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, string.Empty, false, bIsSimulate, true);
            this.LoadAssetSync(rRequest);
            return rRequest;
        }

        public AssetLoaderRequest LoadScene(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate)
        {
            var rRequest = new AssetLoaderRequest(rABName, rAssetName, rLoadSceneMode, false, bIsSimulate);
            this.LoadAssetSync(rRequest);
            return rRequest;
        }

        private void LoadAssetSync(AssetLoaderRequest rRequest)
        {
            this.mIsLoadingRefCount++;

            ABLoadEntry rAssetLoadEntry = null;
            if (!rRequest.IsSimulate)
            {
                if (!ABLoaderVersion.Instance.TryGetValue(rRequest.Path, out rAssetLoadEntry))
                {
                    Debug.LogErrorFormat("---Can not find assetbundle: -- {0}", rRequest.Path);
                    rRequest.Asset = null;
                    this.mIsLoadingRefCount--;
                    return;
                }
            }
            else
            {
                rAssetLoadEntry = new ABLoadEntry()
                {
                    ABName = rRequest.Path,
                    ABPath = ABLoaderVersion.Instance.GetABPath_With_Space(LoaderSpace.Streaming, rRequest.Path),
                    ABDependNames = new string[0],
                };
            }

            // 得到该资源的所有依赖项
            var rABAllDependenceEntries = this.GetABEntryAllDependencies(rAssetLoadEntry);
            for (int i = 0; i < rABAllDependenceEntries.Count; i++)
            {
                rABAllDependenceEntries[i].RefCount++;
            }

            for (int i = 0; i < rABAllDependenceEntries.Count - 1; i++)
            {
                // 构建依赖项的Request
                var rDependenceLoaderRequest = new AssetLoaderRequest(
                    rABAllDependenceEntries[i].ABPath, string.Empty, true, rRequest.IsSimulate, rRequest.IsLoadAllAssets);
                this.LoadAssetSync_OneEntry(rDependenceLoaderRequest, rABAllDependenceEntries[i]);
            }
            this.LoadAssetSync_OneEntry(rRequest, rABAllDependenceEntries[rABAllDependenceEntries.Count - 1]);

            this.mIsLoadingRefCount--;
        }

        private void LoadAssetSync_OneEntry(AssetLoaderRequest rRequest, ABLoadEntry rABLoadEntry)
        {
            // 从缓存的Assetbundle里面加载资源
            rABLoadEntry.IsLoading = true;
            rABLoadEntry.IsLoadCompleted = false;
            this.LoadAssetObjectSync(rRequest, rABLoadEntry);
            rABLoadEntry.IsLoading = false;
            rABLoadEntry.IsLoadCompleted = true;

            // 如果判断此时的RefCount为0的话，那么就unload掉该项资源
            this.AutoCheckUnloadAsset(rABLoadEntry);
        }

        private void LoadAssetObjectSync(AssetLoaderRequest rRequest, ABLoadEntry rAssetLoadEntry)
        {
            string rAssetLoadUrl = rAssetLoadEntry.ABPath;
            if (rRequest.IsSimulate)
            {
                Debug.Log("---Simulate Load ab: " + rAssetLoadUrl);
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(rRequest.AssetName) && !rRequest.IsScene)
                {
                    if (!rRequest.IsLoadAllAssets)
                    {
                        string[] rAssetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(rAssetLoadEntry.ABName, rRequest.AssetName);
                        if (rAssetPaths.Length == 0)
                        {
                            Debug.LogError("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            return;
                        }
                        Object rTargetAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(rAssetPaths[0]);
                        rRequest.Asset = rTargetAsset;
                    }
                    else
                    {
                        string[] rAssetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(rAssetLoadEntry.ABName);
                        if (rAssetPaths.Length == 0)
                        {
                            Debug.LogError("---There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
                            return;
                        }
                        rRequest.AllAssets = new Object[rAssetPaths.Length];
                        for (int i = 0; i < rAssetPaths.Length; i++)
                        {
                            Object rAssetObj = UnityEditor.AssetDatabase.LoadAssetAtPath(rAssetPaths[i], typeof(Object));
                            if (rAssetObj != null)
                                rRequest.AllAssets[i] = rAssetObj;
                        }
                    }
                }
                else
                {
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                        rRequest.AssetName, new LoadSceneParameters() { loadSceneMode = rRequest.SceneMode });
                    
                    string rSceneName = Path.GetFileNameWithoutExtension(rRequest.AssetName);
                    rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                    SceneManager.SetActiveScene(rRequest.Scene);
                }
#endif
            }
            else
            {
                if (rAssetLoadEntry.CacheAsset == null)
                {
                    Debug.Log("---Real Load ab: " + rAssetLoadUrl);
                    // 如果是一个直接的资源，将资源的对象取出来
                    rAssetLoadEntry.CacheAsset = AssetBundle.LoadFromFile(rAssetLoadUrl);
                }
                else
                {
                    Debug.Log("---Load asset: " + rAssetLoadUrl);
                }

                // 加载Object
                if (!string.IsNullOrEmpty(rRequest.AssetName))
                {
                    if (!rRequest.IsScene)
                    {
                        if (!rRequest.IsLoadAllAssets)
                        {
                            rRequest.Asset = rAssetLoadEntry.CacheAsset.LoadAsset(rRequest.AssetName);
                        }
                        else
                        {
                            LoadAllAssets_ByAssetbundle_Sync(rRequest, rAssetLoadEntry.CacheAsset);
                        }
                    }
                    else
                    {
                        rAssetLoadEntry.CacheAsset.GetAllScenePaths();

                        string rSceneName = Path.GetFileNameWithoutExtension(rRequest.AssetName);
                        SceneManager.LoadScene(rSceneName, rRequest.SceneMode);

                        rRequest.Scene = SceneManager.GetSceneByName(rSceneName);
                        SceneManager.SetActiveScene(rRequest.Scene);
                    }
                }
            }
        }

        private void LoadAllAssets_ByAssetbundle_Sync(AssetLoaderRequest rRequest, AssetBundle rAssetbundle)
        {
            rRequest.AllAssets = rAssetbundle.LoadAllAssets();
        }
        #endregion
        
        #region NotImplemented
        public IAsyncOperation<ResourcesLoaderRequest> LoadAsset(string rAssetPath, Type rAssetType)
        {
            throw new NotImplementedException();
        }

        public ResourcesLoaderRequest LoadAllAssets(string rAssetFolderPath, Type rAssetType)
        {
            throw new NotImplementedException();
        }
        #endregion // NotImplemented
    }
}