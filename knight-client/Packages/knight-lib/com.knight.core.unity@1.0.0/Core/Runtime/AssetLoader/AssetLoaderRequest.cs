using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Knight.Core
{
    public interface IAssetLoaderRequest
    {
        public bool IsUnload { get; set; }
        bool IsLoading { get; set; }
        bool IsLoadComplete { get; set; }
        void Start();
        UniTask<IAssetLoaderRequest> Task();
        void Complete();
        void Unload();
    }

    public class AssetLoaderRequest<T> : IAssetLoaderRequest where T : Object
    {
        public string ABPath;
        public string AssetName;
        public bool IsAllAssets;
        public bool IsSimulate;
        public bool IsDependence;

        public T Asset;
        public T[] AllAssets;

        public Scene Scene;
        public List<Scene> AllScenes;
        public bool IsScene;
        public LoadSceneMode SceneMode;

        public bool IsUnload { get; set; }
        public bool IsLoading { get; set; }
        public bool IsLoadComplete { get; set; }

        public UniTaskCompletionSource<IAssetLoaderRequest> LoadTCS;
        public Func<AssetLoaderRequest<T>, UniTask> LoadFunc;

        public AssetLoaderRequest(string rAssetPath, string rAssetName, bool bIsAllAssets, bool bIsSimulate, bool bIsDependence, Func<AssetLoaderRequest<T>, UniTask> rLoadFunc)
        {
            this.ABPath = rAssetPath;
            this.AssetName = rAssetName;
            this.IsAllAssets = bIsAllAssets;
            this.IsSimulate = bIsSimulate;
            this.IsDependence = bIsDependence;
            this.IsScene = false;
            this.SceneMode = LoadSceneMode.Single;
            this.IsUnload = false;
            this.IsLoading = false;
            this.IsLoadComplete = false;
            this.LoadFunc = rLoadFunc;
            this.LoadTCS = new UniTaskCompletionSource<IAssetLoaderRequest>();
            if (this.IsAllAssets)
            {
                this.AssetName = "AllAssets";
            }
        }

        public AssetLoaderRequest(string rAssetPath, string rAssetName, LoadSceneMode rSceneMode, bool bIsAllAssets, bool bIsSimulate, bool bIsDependence, Func<AssetLoaderRequest<T>, UniTask> rLoadFunc)
        {
            this.ABPath = rAssetPath;
            this.AssetName = rAssetName;
            this.IsAllAssets = bIsAllAssets;
            this.IsSimulate = bIsSimulate;
            this.IsDependence = bIsDependence;
            this.IsScene = true;
            this.SceneMode = rSceneMode;
            this.IsUnload = false;
            this.IsLoading = false;
            this.IsLoadComplete = false;
            this.LoadFunc = rLoadFunc;
            this.LoadTCS = new UniTaskCompletionSource<IAssetLoaderRequest>();
            if (this.IsAllAssets)
            {
                this.AssetName = "AllAssets";
            }
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
            AssetLoader.Instance.Unload(this);
        }
    }
}
