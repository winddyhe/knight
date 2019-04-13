//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using UnityEngine;
using UnityFx.Async;

namespace Knight.Framework.TinyMode
{
    public class EffectAssetLoader : TSingleton<EffectAssetLoader>
    {
        private Dict<string, GameObject> mEffectPrefabs;

        private EffectAssetLoader()
        {
        }

        public void Load(string rABPath)
        {
            this.mEffectPrefabs = new Dict<string, GameObject>();
            
            var rAllEffectsRequest = ResourcesLoader.Instance.LoadAllAssets(rABPath, typeof(GameObject));
            if (rAllEffectsRequest.AllAssets == null)
            {
                Debug.LogError("Cannot find effect assets.");
                return;
            }

            var rAllAssets = rAllEffectsRequest.AllAssets;
            for (int i = 0; i < rAllAssets.Length; i++)
            {
                this.mEffectPrefabs.Add(rAllAssets[i].name, rAllAssets[i] as GameObject);
            }
        }

        public GameObject GetEffect(string rEffectName)
        {
            if (this.mEffectPrefabs == null) return null;

            GameObject rEffectPrefabGo = null;
            if (!this.mEffectPrefabs.TryGetValue(rEffectName, out rEffectPrefabGo))
            {
                return null;
            }
            return rEffectPrefabGo;
        }
    }
}
