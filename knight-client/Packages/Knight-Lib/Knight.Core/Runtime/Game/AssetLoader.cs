using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFx.Async;

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
        public bool                 IsSimulate;
        public bool                 IsLoadAllAssets;

        public AssetLoaderRequest(string rPath, string rAssetName, bool bIsScene, bool bIsSimulate, bool bIsLoadAllAssets)
        {
            this.Path               = rPath;
            this.AssetName          = rAssetName;
            this.IsScene            = bIsScene;
            this.IsSimulate         = bIsSimulate;
            this.IsLoadAllAssets    = bIsLoadAllAssets;
        }
    }

    public interface IAssetLoader
    {
        IAsyncOperation<AssetLoaderRequest> LoadAsset(string rAssetPath);
        IAsyncOperation<AssetLoaderRequest> LoadAllAssets(string rAssetsPath);
        IAsyncOperation<AssetLoaderRequest> LoadScene(string rScenePath);

        void UnloadAsset(string rAssetPath);
    }
}
