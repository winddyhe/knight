//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.AssetBundles;
using System.Collections.Generic;

namespace Framework
{
    public class AvatarLoaderRequest : CoroutineRequest<AvatarLoaderRequest>
    {
        public string       ABPath;
        public string       AssetName;
        public GameObject   AvatarGo;

        public AvatarLoaderRequest(string rABPath, string rAssetName)
        {
            this.ABPath     = rABPath;
            this.AssetName  = rAssetName;
        }
    }

    public class AvatarAssetLoader : TSingleton<AvatarAssetLoader>
    {
        private List<string> mUnusedAvatars;

        private AvatarAssetLoader()
        {
            mUnusedAvatars = new List<string>();
        }

        public AvatarLoaderRequest Load(string rABPath, string rAssetName)
        {
            var rRequest = new AvatarLoaderRequest(rABPath, rAssetName);
            rRequest.Start(Load_Async(rRequest));
            return rRequest;
        }

        public void UnloadAsset(string rABPath, bool bIsDelayUnload = true)
        {
            if (bIsDelayUnload)
                mUnusedAvatars.Add(rABPath);
            else
                ABLoader.Instance.UnloadAsset(rABPath);
        }

        public IEnumerator Load_Async(AvatarLoaderRequest rRequest)
        {
            string rAvatarABPath = rRequest.ABPath;
            var rAssetRequest = ABLoader.Instance.LoadAsset(rAvatarABPath, rRequest.AssetName, ABPlatform.Instance.IsSumilateMode_Avatar());
            yield return rAssetRequest;
            if (rAssetRequest.Asset != null)
            {
                GameObject rAvatarGo = GameObject.Instantiate(rAssetRequest.Asset) as GameObject;
                rAvatarGo.name = rAssetRequest.Asset.name;
                rAvatarGo.transform.position = Vector3.zero;
                rRequest.AvatarGo = rAvatarGo;
            }
            this.UnloadUnusedAvatarAssets();
        }

        private void UnloadUnusedAvatarAssets()
        {
            if (mUnusedAvatars == null) return;
            for (int i = 0; i < mUnusedAvatars.Count; i++)
            {
                ABLoader.Instance.UnloadAsset(mUnusedAvatars[i]);
            }
            mUnusedAvatars.Clear();
        }
    }
}