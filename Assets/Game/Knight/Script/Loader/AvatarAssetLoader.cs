//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using Framework;
using UnityEngine.AssetBundles;

namespace Game.Knight
{
    public class AvatarLoaderRequest : CoroutineRequest<AvatarLoaderRequest>
    {
        public string       ABPath;
        public string       AssetName;
        public GameObject   AvatarGo;

        public AvatarLoaderRequest(string rABPath, string rAssetName)
        {
            this.ABPath = rABPath;
            this.AssetName = rAssetName;
        }
    }

    public class AvatarAssetLoader : TSingleton<AvatarAssetLoader>
    {
        private AvatarAssetLoader() { }

        public AvatarLoaderRequest Load(string rABPath, string rAssetName)
        {
            var rRequest = new AvatarLoaderRequest(rABPath, rAssetName);
            rRequest.Start(Load_Async(rRequest));
            return rRequest;
        }

        public IEnumerator Load_Async(AvatarLoaderRequest rRequest)
        {
            string rAvatarABPath = rRequest.ABPath;
            var rAssetRequest = ABLoader.Instance.LoadAsset(rAvatarABPath, rRequest.AssetName);
            yield return rAssetRequest;
            if (rAssetRequest.asset != null)
            {
                GameObject rAvatarGo = GameObject.Instantiate(rAssetRequest.asset) as GameObject;
                rAvatarGo.name = rAssetRequest.asset.name;
                rAvatarGo.transform.position = Vector3.zero;
                rRequest.AvatarGo = rAvatarGo;
            }
            ABLoader.Instance.UnloadAsset(rAvatarABPath);
        }
    }
}