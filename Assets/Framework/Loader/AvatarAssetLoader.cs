//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.AssetBundles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
    public class AvatarLoaderRequest
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

        public async Task<AvatarLoaderRequest> Load(string rABPath, string rAssetName)
        {
            var rRequest = new AvatarLoaderRequest(rABPath, rAssetName);
            string rAvatarABPath = rRequest.ABPath;

            var rAssetRequest = await ABLoader.Instance.LoadAsset(rAvatarABPath, rRequest.AssetName, ABPlatform.Instance.IsSumilateMode_Avatar());
            if (rAssetRequest.Asset != null)
            {
                GameObject rAvatarGo = GameObject.Instantiate(rAssetRequest.Asset) as GameObject;
                rAvatarGo.name = rAssetRequest.Asset.name;
                rAvatarGo.transform.position = Vector3.zero;
                rRequest.AvatarGo = rAvatarGo;
            }
            this.UnloadUnusedAvatarAssets();
            return rRequest;
        }

        public void UnloadAsset(string rABPath, bool bIsDelayUnload = true)
        {
            if (bIsDelayUnload)
                mUnusedAvatars.Add(rABPath);
            else
                ABLoader.Instance.UnloadAsset(rABPath);
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