using Knight.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Knight.Framework.Assetbundle
{
    public class ABLoaderManager : IAssetLoader
    {
        private ABLoader mABLoader;
        private Queue<IAssetLoaderRequest> mAssetRequestQueue;
        private List<IAssetLoaderRequest> mAssetRequestLoadingList;

        private int mMaxQueueCount = 1000;
        private bool mIsInitialized = false;

        public bool IsDevelopMode => this.mABLoader != null ? this.mABLoader.IsDevelopMode : true;
        public bool IsHotfixDebugMode => this.mABLoader != null ? this.mABLoader.IsHotfixDebugMode : true;

        public void Initialize(string rSimulateConfigPath, string rABBuilderConfigPath)
        {
            this.mAssetRequestQueue = new Queue<IAssetLoaderRequest>();
            this.mAssetRequestLoadingList = new List<IAssetLoaderRequest>();

            this.mABLoader = new ABLoader();
            this.mABLoader.Initialize(rSimulateConfigPath, rABBuilderConfigPath);

            this.mIsInitialized = true;
        }

        public AssetLoaderRequest<T> LoadAssetAsync<T>(string rAssetPath, string rAssetName, bool bIsSimulate) where T : Object
        {
            var rRequest = new AssetLoaderRequest<T>(rAssetPath, rAssetName, false, bIsSimulate, false, this.mABLoader.LoadAssetAsync);
            this.mAssetRequestQueue.Enqueue(rRequest);
            return rRequest;
        }

        public AssetLoaderRequest<T> LoadAllAssetAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object
        {
            var rRequest = new AssetLoaderRequest<T>(rAssetPath, "", true, bIsSimulate, false, this.mABLoader.LoadAssetAsync);
            this.mAssetRequestQueue.Enqueue(rRequest);
            return rRequest;
        }

        public AssetLoaderRequest<T> LoadSceneAsync<T>(string rAssetPath, string rAssetName, LoadSceneMode rSceneMode, bool bIsSimulate) where T : Object
        {
            var rRequest = new AssetLoaderRequest<T>(rAssetPath, rAssetName, rSceneMode, false, bIsSimulate, false, this.mABLoader.LoadAssetAsync);
            this.mAssetRequestQueue.Enqueue(rRequest);
            return rRequest;
        }

        public AssetLoaderRequest<T> LoadAllSceneAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object
        {
            var rRequest = new AssetLoaderRequest<T>(rAssetPath, "", LoadSceneMode.Additive, true, bIsSimulate, false, this.mABLoader.LoadAssetAsync);
            this.mAssetRequestQueue.Enqueue(rRequest);
            return rRequest;
        }

        public bool IsSimulate(ABSimuluateType rType)
        {
            if (this.mABLoader == null) return false;
            return this.mABLoader.IsSimulate(rType);
        }

        public void Update()
        {
            if (!this.mIsInitialized)
            {
                return;
            }

            this.mABLoader?.Update();
            this.LoadBatchesUpdate();
        }

        public void Unload<T>(AssetLoaderRequest<T> rRequest) where T : Object
        {
            this.mABLoader?.Unload(rRequest);
        }

        public void AddGlobalABEntries(List<string> rGlobalABEntries)
        {
            this.mABLoader?.AddGlobalABEntries(rGlobalABEntries);
        }

        public void RemoveGlobalABEntries(List<string> rGlobalABEntries)
        {
            this.mABLoader?.RemoveGlobalABEntries(rGlobalABEntries);
        }

        public void ForceUnloadAllAssets()
        {
            this.mABLoader?.ForceUnloadAllAssets();
        }

        public string GetLoadVersionABPath(string rABPath)
        {
            if (ABLoadVersion.Instance.TryGetValue(rABPath, out var rABLoadEntry))
            {
                return rABLoadEntry.ABPath;
            }
            return string.Empty;
        }

        public bool ExistsAssetBundle(string rABName, bool bIsSimulate)
        {
            return this.mABLoader.ExistsAssetBundle(rABName, bIsSimulate);
        }

        public bool ExistsAsset(string rABName, string rAssetName, bool bIsSimulate)
        {
            return this.mABLoader.ExistsAsset(rABName, rAssetName, bIsSimulate);
        }

        /// <summary>
        /// 分批加载更新检测
        /// </summary>
        private void LoadBatchesUpdate()
        {
            int nLoadedCount = 0;
            for (int i = 0; i < this.mAssetRequestLoadingList.Count; i++)
            {
                if (!this.mAssetRequestLoadingList[i].IsLoading && this.mAssetRequestLoadingList[i].IsLoadComplete)
                {
                    nLoadedCount++;
                }
            }
            if (nLoadedCount == this.mAssetRequestLoadingList.Count)
            {
                this.mAssetRequestLoadingList.Clear();
                int nCount = 0;
                while (nCount < this.mMaxQueueCount)
                {
                    if (this.mAssetRequestQueue.Count == 0)
                    {
                        break;
                    }
                    var rRequest = this.mAssetRequestQueue.Dequeue();
                    if (rRequest != null && !rRequest.IsUnload) // 还有元素，并且没有被卸载
                    {
                        this.mAssetRequestLoadingList.Add(rRequest);
                        rRequest.Start();
                    }
                    nCount++;
                }
            }
        }
    }
}
