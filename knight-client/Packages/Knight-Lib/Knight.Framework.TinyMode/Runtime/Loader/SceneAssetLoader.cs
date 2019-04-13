//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.TinyMode
{
    public class SceneAssetLoader : TSingleton<SceneAssetLoader>
    {
        private Dict<string, GameObject> mScenePrefabs;

        private SceneAssetLoader()
        {
        }

        public void Load(string rABPath)
        {
            this.mScenePrefabs = new Dict<string, GameObject>();

            var rAllScenesRequest = AssetLoader.Instance.LoadAllAssets(rABPath, typeof(GameObject));
            if (rAllScenesRequest.AllAssets == null)
            {
                Debug.LogError("Cannot find effect assets.");
                return;
            }

            var rAllAssets = rAllScenesRequest.AllAssets;
            for (int i = 0; i < rAllAssets.Length; i++)
            {
                this.mScenePrefabs.Add(rAllAssets[i].name, rAllAssets[i] as GameObject);
            }
        }

        public GameObject GetScene(string rSceneName)
        {
            if (this.mScenePrefabs == null) return null;

            GameObject rEffectPrefabGo = null;
            if (!this.mScenePrefabs.TryGetValue(rSceneName, out rEffectPrefabGo))
            {
                return null;
            }
            return rEffectPrefabGo;
        }
    }
}
