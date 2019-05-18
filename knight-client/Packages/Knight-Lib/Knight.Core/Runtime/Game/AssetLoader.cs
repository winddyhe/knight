using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFx.Async;
using Object = UnityEngine.Object;

namespace Knight.Core
{
    public class AssetLoaderRequest : AsyncRequest<AssetLoaderRequest>
    {
        public Object               Asset;
        public Object[]             AllAssets;
        public Scene                Scene;

        public string               Path;
        public string               AssetName;
        public bool                 IsScene;
        public LoadSceneMode        SceneMode;

        public bool                 IsSimulate;
        public bool                 IsLoadAllAssets;

        public bool                 IsDependence;

        /// <summary>
        /// 加载非场景资源
        /// </summary>
        public AssetLoaderRequest(string rPath, string rAssetName, bool bIsDependence, bool bIsSimulate, bool bIsLoadAllAssets)
        {
            this.Path               = rPath;
            this.AssetName          = rAssetName;
            this.IsScene            = false;
            this.SceneMode          = LoadSceneMode.Single;
            this.IsSimulate         = bIsSimulate;
            this.IsDependence       = bIsDependence;
            this.IsLoadAllAssets    = bIsLoadAllAssets;

            if (this.IsLoadAllAssets)
            {
                this.AssetName = "AllAssets";
            }
        }

        /// <summary>
        /// 加载场景资源
        /// </summary>
        public AssetLoaderRequest(string rPath, string rAssetName, LoadSceneMode rSceneMode, bool bIsDependence, bool bIsSimulate)
        {
            this.Path               = rPath;
            this.AssetName          = rAssetName;
            this.IsScene            = true;
            this.SceneMode          = rSceneMode;
            this.IsSimulate         = bIsSimulate;
            this.IsDependence       = bIsDependence;
            this.IsLoadAllAssets    = false;

            if (this.IsLoadAllAssets)
            {
                this.AssetName = "AllAssets";
            }
        }
    }
	
	public class ResourcesLoaderRequest : AsyncRequest<ResourcesLoaderRequest>
    {
        public Object       Asset;
        public Object[]     AllAssets;

        public string       AssetPath;
        public Type         AssetType;

        public ResourcesLoaderRequest(string rAssetPath, Type rAssetType)
        {
            this.AssetPath = rAssetPath;
        }
    }

    public interface IAssetLoader
    {
        void Initialize();
        void UnloadAsset(string rABPath);

        IAsyncOperation<AssetLoaderRequest> LoadAssetAsync(string rABName, string rAssetName, bool bIsSimulate);
        IAsyncOperation<AssetLoaderRequest> LoadAllAssetsAsync(string rABName, bool bIsSimulate);
        IAsyncOperation<AssetLoaderRequest> LoadSceneAsync(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate);

        AssetLoaderRequest LoadAsset(string rABName, string rAssetName, bool bIsSimulate);
        AssetLoaderRequest LoadAsset(string rABName, bool bIsSimulate);
        AssetLoaderRequest LoadScene(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate);

        bool IsSumilateMode_Script();
        bool IsSumilateMode_Config();
        bool IsSumilateMode_GUI();
        bool IsSumilateMode_Avatar();
        bool IsSumilateMode_Scene();
		
		IAsyncOperation<ResourcesLoaderRequest> LoadAsset(string rAssetPath, Type rAssetType);
        ResourcesLoaderRequest LoadAllAssets(string rAssetFolderPath, Type rAssetType);
    }

    public class AssetLoader
    {
        public static IAssetLoader Instance;
    }
}
