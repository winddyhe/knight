//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Knight.Core;
using UnityFx.Async;

namespace Knight.Framework.TinyMode
{
    public class AvatarAssetLoader : TSingleton<AvatarAssetLoader>
    {
        private Dict<string, GameObject> mActorPrefabs;

        private AvatarAssetLoader()
        {
        }

        public void Load(string rAssetPath)
        {
            this.mActorPrefabs = new Dict<string, GameObject>();

            var rAllActorRequest = AssetLoader.Instance.LoadAllAssets(rAssetPath, typeof(GameObject));
            if (rAllActorRequest.AllAssets == null)
            {
                Debug.LogError("Cannot find actor assets.");
                return;
            }

            var rAllAssets = rAllActorRequest.AllAssets;
            for (int i = 0; i < rAllAssets.Length; i++)
            {
                this.mActorPrefabs.Add(rAllAssets[i].name, rAllAssets[i] as GameObject);
            }
        }

        public GameObject GetActor(string rAvatarName)
        {
            if (this.mActorPrefabs == null) return null;

            GameObject rActorPrefabGo = null;
            if (!this.mActorPrefabs.TryGetValue(rAvatarName, out rActorPrefabGo))
            {
                return null;
            }
            return rActorPrefabGo;
        }
    }
}