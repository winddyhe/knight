using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;
using UnityFx.Async;
using System;
using Object = UnityEngine.Object;

namespace Knight.Framework
{
    public class ResourcesLoader : TSingleton<ResourcesLoader>
    {
        public class LoaderRequest : AsyncRequest<LoaderRequest>
        {
            public Object   Asset;
            public Object[] AllAssets;

            public string   AssetPath;
            public Type     AssetType;

            public LoaderRequest(string rAssetPath, Type rAssetType)
            {
                this.AssetPath = rAssetPath;
            }
        }

        private ResourcesLoader()
        {
        }

        public IAsyncOperation<LoaderRequest> LoadAsset(string rAssetPath, Type rAssetType)
        {
            var rRequest = new LoaderRequest(rAssetPath, rAssetType);
            return rRequest.Start(LoadAsset_Async(rRequest, rAssetType));
        }

        public LoaderRequest LoadAllAssets(string rAssetFolderPath, Type rAssetType)
        {
            var rRequest = new LoaderRequest(rAssetFolderPath, rAssetType);
            rRequest.AllAssets = Resources.LoadAll(rAssetFolderPath, rAssetType);
            return rRequest;
        }

        private IEnumerator LoadAsset_Async(LoaderRequest rRequest, Type rAssetType)
        {
            var rResourceRequest = Resources.LoadAsync(rRequest.AssetPath, rAssetType);
            yield return rResourceRequest;

            rRequest.Asset = rResourceRequest.asset;
            rRequest.SetResult(rRequest);
        }
    }
}