using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knight.Core
{
    public class InstantiateRequest<T> : IAssetLoaderRequest where T : Object
    {
        public int Count;
        public T Asset;

        public T Result;
        public T[] Results;
        
        public bool IsUnload { get; set; }
        public bool IsLoading { get; set; }
        public bool IsLoadComplete { get; set; }

        public UniTaskCompletionSource<IAssetLoaderRequest> LoadTCS;
        public Func<InstantiateRequest<T>, UniTask> LoadFunc;

        public InstantiateRequest(T rAsset, int nCount, Func<InstantiateRequest<T>, UniTask> rLoadFunc)
        {
            this.Asset = rAsset;
            this.Count = nCount;
            this.Result = null;
            this.Results = null;
            this.IsUnload = false;
            this.IsLoading = false;
            this.IsLoadComplete = false;
            this.LoadFunc = rLoadFunc;
            this.LoadTCS = new UniTaskCompletionSource<IAssetLoaderRequest>();
        }

        public void Start()
        {
            this.IsLoading = true;
            this.IsLoadComplete = false;
            this.LoadFunc?.Invoke(this);
        }

        public UniTask<IAssetLoaderRequest> Task()
        {
            return this.LoadTCS.Task;
        }

        public void Complete()
        {
            this.LoadTCS?.TrySetResult(this);
            this.IsLoading = false;
            this.IsLoadComplete = true;
        }

        public void Unload()
        {
            if (this.Results != null)
            {
                foreach (var rResult in this.Results)
                {
                    UtilUnityTool.SafeDestroy(rResult);
                }
            }
            this.Result = null;
            this.Results = null;
            this.IsUnload = true;
            this.IsLoadComplete = false;
            this.IsLoading = false;
        }
    }

    public class InstantiateManager : TSingleton<InstantiateManager>
    {
        private Queue<IAssetLoaderRequest> mAssetRequestQueue;
        private List<IAssetLoaderRequest> mAssetRequestLoadingList;

        private int mMaxQueueCount = 1000;
        private bool mIsInitialized = false;

        private InstantiateManager() 
        {
        }

        public void Initialize()
        {
            if (this.mIsInitialized) return;

            this.mAssetRequestQueue = new Queue<IAssetLoaderRequest>();
            this.mAssetRequestLoadingList = new List<IAssetLoaderRequest>();
            this.mIsInitialized = true;
        }

        public InstantiateRequest<T> CreateAsync<T>(T rAsset, int nCount) where T : Object
        {
            var rRequest = new InstantiateRequest<T>(rAsset, nCount, this.CreateInstantiateRequest);
            this.mAssetRequestQueue.Enqueue(rRequest);
            return rRequest;
        }

        public void Destroy(IAssetLoaderRequest rRequest)
        {
            rRequest.IsUnload = true;
            // 如果加载完了
            if (!rRequest.IsLoading && rRequest.IsLoadComplete)
            {
                rRequest.Unload();
            }
        }

        public void Update()
        {
            if (!this.mIsInitialized)
            {
                return;
            }
            this.LoadBatchesUpdate();
        }

        private async UniTask CreateInstantiateRequest<T>(InstantiateRequest<T> rRequest) where T : Object
        {
            rRequest.IsLoading = true;
            rRequest.IsLoadComplete = false;
            var rInstantiateRequest = await Object.InstantiateAsync<T>(rRequest.Asset, rRequest.Count);
            if (rInstantiateRequest == null || rInstantiateRequest.Length == 0)
            {
                rRequest.Result = null;
                rRequest.Results = null;
                rRequest.IsLoading = false;
                rRequest.IsLoadComplete = true;
                rRequest.Complete();
                return;
            }
            rRequest.Result = rInstantiateRequest[0];
            rRequest.Results = rInstantiateRequest;
            rRequest.IsLoading = false;
            rRequest.IsLoadComplete = true;
            
            // 处理异步卸载逻辑
            if (rRequest.IsUnload)
            {
                foreach (var rResult in rRequest.Results)
                {
                    UtilUnityTool.SafeDestroy(rResult);
                }
                rRequest.Results = null;
                rRequest.Result = null;
                rRequest.IsUnload = true;
                rRequest.IsLoadComplete = false;
                rRequest.IsLoading = false;
            }
            rRequest.Complete();
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

