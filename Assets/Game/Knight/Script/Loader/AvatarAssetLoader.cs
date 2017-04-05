//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using Framework;

namespace Game.Knight
{
    public class AvatarLoaderRequest : CoroutineRequest<AvatarLoaderRequest>
    {
        public Avatar       avatar;
        public GameObject   avatarGo;

        public AvatarLoaderRequest(Avatar rAvatar)
        {
            this.avatar = rAvatar;
        }
    }

    public class AvatarAssetLoader : TSingleton<AvatarAssetLoader>
    {
        private AvatarAssetLoader() { }

        public AvatarLoaderRequest Load(Avatar rAvatar)
        {
            var rRequest = new AvatarLoaderRequest(rAvatar);
            rRequest.Start(Load_Async(rRequest));
            return rRequest;
        }

        public IEnumerator Load_Async(AvatarLoaderRequest rRequest)
        {
            string rAvatarABPath = rRequest.avatar.ABPath;
            var rAssetRequest = AssetLoadManager.Instance.LoadAsset(rAvatarABPath, rRequest.avatar.AssetName);
            yield return rAssetRequest;
            if (rAssetRequest.asset != null)
            {
                GameObject rAvatarGo = GameObject.Instantiate(rAssetRequest.asset) as GameObject;
                rAvatarGo.name = rAssetRequest.asset.name;
                rAvatarGo.transform.position = Vector3.zero;
                rRequest.avatarGo = rAvatarGo;
            }
            AssetLoadManager.Instance.UnloadAsset(rAvatarABPath);
        }
    }
}