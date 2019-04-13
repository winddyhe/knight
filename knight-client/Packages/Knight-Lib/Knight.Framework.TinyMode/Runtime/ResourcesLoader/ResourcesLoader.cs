//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;
using UnityFx.Async;
using System;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace Knight.Framework.TinyMode
{
    public class ResourcesLoader : IAssetLoader
    {
        public void Initialize()
        {
        }

        public void UnloadAsset(string rABPath)
        {
        }

        public IAsyncOperation<ResourcesLoaderRequest> LoadAsset(string rAssetPath, Type rAssetType)
        {
            var rRequest = new ResourcesLoaderRequest(rAssetPath, rAssetType);
            return rRequest.Start(LoadAsset_Async(rRequest, rAssetType));
        }

        public ResourcesLoaderRequest LoadAllAssets(string rAssetFolderPath, Type rAssetType)
        {
            var rRequest = new ResourcesLoaderRequest(rAssetFolderPath, rAssetType);
            rRequest.AllAssets = Resources.LoadAll(rAssetFolderPath, rAssetType);
            return rRequest;
        }

        private IEnumerator LoadAsset_Async(ResourcesLoaderRequest rRequest, Type rAssetType)
        {
            var rResourceRequest = Resources.LoadAsync(rRequest.AssetPath, rAssetType);
            yield return rResourceRequest;
            rRequest.Asset = rResourceRequest.asset;
            rRequest.SetResult(rRequest);
        }

        #region NotImplemented
        public IAsyncOperation<AssetLoaderRequest> LoadAssetAsync(string rABName, string rAssetName, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<AssetLoaderRequest> LoadAllAssetsAsync(string rABName, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<AssetLoaderRequest> LoadSceneAsync(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public AssetLoaderRequest LoadAsset(string rABName, string rAssetName, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public AssetLoaderRequest LoadAsset(string rABName, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public AssetLoaderRequest LoadScene(string rABName, string rAssetName, LoadSceneMode rLoadSceneMode, bool bIsSimulate)
        {
            throw new NotImplementedException();
        }

        public bool IsSumilateMode_Script()
        {
            throw new NotImplementedException();
        }

        public bool IsSumilateMode_Config()
        {
            throw new NotImplementedException();
        }

        public bool IsSumilateMode_GUI()
        {
            throw new NotImplementedException();
        }

        public bool IsSumilateMode_Avatar()
        {
            throw new NotImplementedException();
        }

        public bool IsSumilateMode_Scene()
        {
            throw new NotImplementedException();
        }
        #endregion // NotImplemented

    }
}