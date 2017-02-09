//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Core;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// 资源的加载信息
    /// </summary>
    public class AssetLoadInfo
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        public string       abName;
        /// <summary>
        /// 资源的路径名
        /// </summary>
        public string       abPath;
        /// <summary>
        /// 该资源的依赖项
        /// </summary>
        public string[]     abDependNames;
        /// <summary>
        /// 是否处于加载中
        /// </summary>
        public bool         isLoading = false;
        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool         isLoadCompleted = false;
        /// <summary>
        /// 缓存的Cache
        /// </summary>
        public AssetBundle  cacheAsset;
        /// <summary>
        /// 该资源包的引用计数
        /// </summary>
        public int          refCount = 0;
    }
    
    /// <summary>
    /// 加载资源的管理类，用作资源的加载管理
    /// </summary>
    public class AssetLoadManager : MonoBehaviour
    {
        public class LoaderRequest : BaseCoroutineRequest<LoaderRequest>
        {
            public Object asset;
            public Scene  scene;
            public string path;
            public string assetName;
            public bool   isScene;

            public LoaderRequest(string rPath, string rAssetName, bool bIsScene)
            {
                this.path = rPath;
                this.assetName = rAssetName;
                this.isScene = bIsScene;
            }
        }

        private static AssetLoadManager     __instance;
        public  static AssetLoadManager     Instance { get { return __instance; } }
    
        /// <summary>
        /// 所有资源的Manifest
        /// </summary>
        private AssetBundleManifest         manifest;
        /// <summary>
        /// Manifest是否加载完成
        /// </summary>
        private bool                        isManifestLoadCompleted;
        /// <summary>
        /// 加载的资源信息
        /// </summary>
        private Dict<string, AssetLoadInfo> assetLoadInfos;

        private List<string>                loadedAssetbundles;

        private List<string>                loadedScenebundles;

        public bool IsMainfestLoadCompleted { get { return isManifestLoadCompleted; } }
    
        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
    
                this.loadedAssetbundles = new List<string>();
                this.loadedScenebundles = new List<string>();
            }
        }
    
        public Coroutine LoadManifest()
        {
            string rManifestUrl = AssetPlatformManager.Instance.GetAssetbundleManifestUrl();
            return this.StartCoroutine(LoadManifest_Async(rManifestUrl));
        }
    
        private IEnumerator LoadManifest_Async(string rManifrestUrl)
        {
            isManifestLoadCompleted = false;
    
            WWW www = WWWAssist.Load(rManifrestUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                isManifestLoadCompleted = true;
                yield break;
            }

            this.manifest = www.assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            WWWAssist.Destroy(ref www);
    
            string[] rALLAB = this.manifest.GetAllAssetBundles();
            assetLoadInfos = new Dict<string, AssetLoadInfo>();
            for (int i = 0; i < rALLAB.Length; i++)
            {
                // 开始构建AssetLoadInfo
                AssetLoadInfo rAssetLoadInfo = new AssetLoadInfo();
                rAssetLoadInfo.abPath = rALLAB[i];
                rAssetLoadInfo.abName = rALLAB[i];
                string[] rDependABs = this.manifest.GetDirectDependencies(rALLAB[i]);
                rAssetLoadInfo.abDependNames = rDependABs;
                rAssetLoadInfo.isLoading = false;
                rAssetLoadInfo.isLoadCompleted = false;
                rAssetLoadInfo.cacheAsset = null;
                rAssetLoadInfo.refCount = 0;
                assetLoadInfos.Add(rAssetLoadInfo.abName, rAssetLoadInfo);
            }
    
            isManifestLoadCompleted = true;
        }
    
        public LoaderRequest LoadAsset(string rAssetbundleName, string rAssetName)
        {
            if (!this.loadedAssetbundles.Contains(rAssetbundleName))
                this.loadedAssetbundles.Add(rAssetbundleName);

            LoaderRequest rRequest = new LoaderRequest(rAssetbundleName, rAssetName, false);
            rRequest.Start(LoadAsset_Async(rRequest));
            return rRequest;
        }

        public LoaderRequest LoadScene(string rAssetbundleName, string rScenePath)
        {
            if (!this.loadedScenebundles.Contains(rAssetbundleName))
                this.loadedScenebundles.Add(rAssetbundleName);

            LoaderRequest rSceneRequest = new LoaderRequest(rAssetbundleName, rScenePath, true);
            rSceneRequest.Start(LoadAsset_Async(rSceneRequest));
            return rSceneRequest;
        }
        
        public void UnloadAllLoadedAssetbundles()
        {
            for (int i = 0; i < this.loadedAssetbundles.Count; i++)
            {
                UnloadAsset(this.loadedAssetbundles[i]);
            }
            this.loadedAssetbundles.Clear();
            Resources.UnloadUnusedAssets();
        }
    
        public void UnloadAsset(string rAssetbundleName)
        {
            AssetLoadInfo rAssetLoadInfo = null;
            if (!assetLoadInfos.TryGetValue(rAssetbundleName, out rAssetLoadInfo))
            {
                Debug.LogErrorFormat("找不到该资源 -- {0}", rAssetbundleName);
                return;
            }
    
            // 递归遍历该资源的依赖项
            for (int i = 0; i < rAssetLoadInfo.abDependNames.Length; i++)
            {
                UnloadAsset(rAssetLoadInfo.abDependNames[i]);
            }
    
            // 引用计数减1
            rAssetLoadInfo.refCount--;
    
            //确定该Info的引用计数是否为0，如果为0则删除它
            if (rAssetLoadInfo.refCount == 0)
            {
                if (rAssetLoadInfo.cacheAsset != null)
                {
                    rAssetLoadInfo.cacheAsset.Unload(false);
                    rAssetLoadInfo.cacheAsset = null;
                }
                rAssetLoadInfo.isLoadCompleted = false;
                rAssetLoadInfo.isLoading = false;
            }
        }
    
        private IEnumerator LoadAsset_Async(LoaderRequest rRequest)
        {
            // 确认Manifest已经加载完成
            while (!isManifestLoadCompleted)
            {
                yield return 0;
            }
    
            AssetLoadInfo rAssetLoadInfo = null;
            if (!assetLoadInfos.TryGetValue(rRequest.path, out rAssetLoadInfo))
            {
                Debug.LogErrorFormat("找不到该资源 -- {0}", rRequest.path);
                rRequest.asset = null;
                yield break;
            }
            
            // 确认该资源是否已经加载完成或者正在被加载
            while (rAssetLoadInfo.isLoading && !rAssetLoadInfo.isLoadCompleted)
            {
                yield return 0;
            }
    
            //引用计数加1
            rAssetLoadInfo.refCount++;
    
            // 如果该资源加载完成了
            if (!rAssetLoadInfo.isLoading && rAssetLoadInfo.isLoadCompleted)
            {
                if (!string.IsNullOrEmpty(rRequest.assetName))
                {
                    AssetBundleRequest rABRequest = rAssetLoadInfo.cacheAsset.LoadAssetAsync(rRequest.assetName);
                    yield return rABRequest;
                    rRequest.asset = rABRequest.asset;
                }
                yield break;
            }
    
            // 开始加载资源依赖项
            if (rAssetLoadInfo.abDependNames != null)
            {
                for (int i = rAssetLoadInfo.abDependNames.Length - 1; i >= 0; i--)
                {
                    string rDependABPath = rAssetLoadInfo.abDependNames[i];
                    string rDependABName = rDependABPath;

                    LoaderRequest rDependAssetRequest = new LoaderRequest(rDependABName, "", false);
                    rDependAssetRequest.Start(LoadAsset_Async(rDependAssetRequest));
                    yield return rDependAssetRequest.Coroutine;
                }
            }
    
            //开始加载当前的资源包
            rAssetLoadInfo.isLoading = true;
            rAssetLoadInfo.isLoadCompleted = false;
            
            string rAssetLoadUrl = AssetPlatformManager.Instance.GetStreamingUrl_CurPlatform(rAssetLoadInfo.abPath);
            WWW www = new WWW(rAssetLoadUrl);
            yield return www;
    
            // 如果是一个直接的资源，将资源的对象取出来
            rAssetLoadInfo.cacheAsset = www.assetBundle;
            
            WWWAssist.Destroy(ref www);
    
            // 加载Object
            if (!string.IsNullOrEmpty(rRequest.assetName))
            {
                if (!rRequest.isScene)
                {
                    AssetBundleRequest rABRequest = rAssetLoadInfo.cacheAsset.LoadAssetAsync(rRequest.assetName);
                    yield return rABRequest;
                    rRequest.asset = rABRequest.asset;
                }
                else
                {
                    rAssetLoadInfo.cacheAsset.GetAllScenePaths();
                }
            }

            rAssetLoadInfo.isLoading = false;
            rAssetLoadInfo.isLoadCompleted = true;
        }
    }
}