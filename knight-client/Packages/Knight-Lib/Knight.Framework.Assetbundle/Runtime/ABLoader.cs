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

namespace Knight.Framework.AssetBundles
{    
    /// <summary>
    /// 加载资源的管理类，用作资源的加载管理
    /// 采用缓存Assetbundle对象的策略，半自动的引用计数
    /// </summary>
    public class ABLoader : TSingleton<ABLoader>
    {
        private List<string>    LoadedAssetbundles;
        private List<string>    LoadedScenebundles;

        private ABLoader()
        {
            this.LoadedAssetbundles = new List<string>();
            this.LoadedScenebundles = new List<string>();
        }

        public IAsyncOperation<AssetLoaderRequest> LoadAssetbundle(string rAssetbundleName, bool bIsSimulate)
        {
            return LoadAsset(rAssetbundleName, string.Empty, bIsSimulate);
        }
        
        public IAsyncOperation<AssetLoaderRequest> LoadAsset(string rAssetbundleName, string rAssetName, bool bIsSimulate)
        {
            if (!this.LoadedAssetbundles.Contains(rAssetbundleName))
                this.LoadedAssetbundles.Add(rAssetbundleName);

            var rRequest = new AssetLoaderRequest(rAssetbundleName, rAssetName, false, bIsSimulate, false);
            return rRequest.Start(LoadAsset_Async(rRequest));
        }

        public IAsyncOperation<AssetLoaderRequest> LoadAllAssets(string rAssetbundleName, bool bIsSimulate)
        {
            if (!this.LoadedAssetbundles.Contains(rAssetbundleName))
                this.LoadedAssetbundles.Add(rAssetbundleName);

            var rRequest = new AssetLoaderRequest(rAssetbundleName, "AllAssets", false, bIsSimulate, true);
            return rRequest.Start(LoadAsset_Async(rRequest));
        }

        public IAsyncOperation<AssetLoaderRequest> LoadScene(string rAssetbundleName, string rScenePath)
        {
            if (!this.LoadedScenebundles.Contains(rAssetbundleName))
                this.LoadedScenebundles.Add(rAssetbundleName);
            
            var rSceneRequest = new AssetLoaderRequest(rAssetbundleName, rScenePath, true, false, false);
            return rSceneRequest.Start(LoadAsset_Async(rSceneRequest));
        }
        
        public void UnloadAllLoadedAssetbundles()
        {
            for (int i = 0; i < this.LoadedAssetbundles.Count; i++)
            {
                UnloadAsset(this.LoadedAssetbundles[i]);
            }
            this.LoadedAssetbundles.Clear();
            Resources.UnloadUnusedAssets();
        }
    
        public void UnloadAsset(string rAssetbundleName)
        {
            ABLoadEntry rAssetLoadEntry = null;
            if (!ABLoaderVersion.Instance.TryGetValue(rAssetbundleName, out rAssetLoadEntry))
            {
                Debug.LogErrorFormat("Can not find assetbundle: -- {0}", rAssetbundleName);
                return;
            }
    
            // 递归遍历该资源的依赖项
            for (int i = 0; i < rAssetLoadEntry.ABDependNames.Length; i++)
            {
                UnloadAsset(rAssetLoadEntry.ABDependNames[i]);
            }
    
            // 引用计数减1
            rAssetLoadEntry.RefCount--;
            
            //确定该Info的引用计数是否为0，如果为0则删除它
            if (rAssetLoadEntry.RefCount == 0)
            {
                if (rAssetLoadEntry.CacheAsset != null)
                {
                    Debug.LogFormat("-- Real unload assetbundle: {0}", rAssetbundleName);
                    rAssetLoadEntry.CacheAsset.Unload(true);
                    rAssetLoadEntry.CacheAsset = null;
                }
                rAssetLoadEntry.IsLoadCompleted = false;
                rAssetLoadEntry.IsLoading = false;
            }
        }

        public bool IsSumilateMode_GUI()
        {
            return ABPlatform.Instance.IsSumilateMode_GUI();
        }
        
        private IEnumerator LoadAsset_Async(AssetLoaderRequest rRequest)
        {
            ABLoadEntry rAssetLoadEntry = null;
            if (!ABLoaderVersion.Instance.TryGetValue(rRequest.Path, out rAssetLoadEntry))
            {
                Debug.LogErrorFormat("Can not find assetbundle: -- {0}", rRequest.Path);
                rRequest.Asset = null;
                rRequest.SetResult(rRequest);
                yield break;
            }

            //引用计数加1
            rAssetLoadEntry.RefCount++;

            // 确认未加载完成并且正在被加载、一直等待其加载完成
            while (rAssetLoadEntry.IsLoading && !rAssetLoadEntry.IsLoadCompleted)
            {
                yield return new WaitForEndOfFrame();
            }
    
            // 如果该资源加载完成了
            if (!rAssetLoadEntry.IsLoading && rAssetLoadEntry.IsLoadCompleted)
            {
                // 从缓存的Assetbundle里面加载资源
                yield return LoadAssetObject(rRequest, rAssetLoadEntry, false);
                rRequest.SetResult(rRequest);
                yield break;
            }
            
            // 开始加载资源依赖项
            if (rAssetLoadEntry.ABDependNames != null && !rRequest.IsSimulate)
            {
                for (int i = rAssetLoadEntry.ABDependNames.Length - 1; i >= 0; i--)
                {
                    string rDependABPath = rAssetLoadEntry.ABDependNames[i];
                    string rDependABName = rDependABPath;

                    var rDependAssetRequest = new AssetLoaderRequest(rDependABName, "", false, rRequest.IsSimulate, false);
                    yield return LoadAsset_Async(rDependAssetRequest);
                }
            }

            //开始加载当前的资源包
            rAssetLoadEntry.IsLoading = true;
            rAssetLoadEntry.IsLoadCompleted = false;

            // 真正的从AB包里面加载资源
            yield return LoadAssetObject(rRequest, rAssetLoadEntry, true);

            rRequest.SetResult(rRequest);

            rAssetLoadEntry.IsLoading = false;
            rAssetLoadEntry.IsLoadCompleted = true;
        }

        private IEnumerator LoadAssetObject(AssetLoaderRequest rRequest, ABLoadEntry rAssetLoadEntry, bool bRealLoad)
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
                            Debug.LogError("There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
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
                            Debug.LogError("There is no asset with name \"" + rRequest.AssetName + "\" in " + rAssetLoadEntry.ABName);
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
#endif
            }
            else
            {
                if (bRealLoad)
                {
                    Debug.Log("---Real Load ab: " + rAssetLoadUrl);
                    
                    // 如果是一个直接的资源，将资源的对象取出来
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
                            yield return LoadAllAssets_ByAssetbundle(rRequest, rAssetLoadEntry.CacheAsset);
                        }
                    }
                    else
                    {
                        rAssetLoadEntry.CacheAsset.GetAllScenePaths();
                    }
                }
            }
        }

        private IEnumerator LoadAllAssets_ByAssetbundle(AssetLoaderRequest rRequest, AssetBundle rAssetbundle)
        {
            var rAllAssetsRequest = rAssetbundle.LoadAllAssetsAsync();
            yield return rAllAssetsRequest;
            rRequest.AllAssets = rAllAssetsRequest.allAssets;
        }
    }
}