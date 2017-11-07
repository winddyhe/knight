//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.AssetBundles;
using System.Threading.Tasks;

namespace Framework.WindUI
{
    /// <summary>
    /// UI加载器
    /// </summary>
    public class UIAssetLoader : TSingleton<UIAssetLoader>
    {
        public class LoaderRequest
        {
            public GameObject   ViewPrefabGo;
            public string       ViewName;

            public LoaderRequest(string rViewName)
            {
                this.ViewName = rViewName;
            }
        }
        
        private UIAssetLoader() { }
        
        /// <summary>
        /// 异步加载UI
        /// </summary>
        public async Task<LoaderRequest> LoadUI(string rViewName)
        {
            LoaderRequest rRequest = new LoaderRequest(rViewName);
            string rUIABPath = "game/ui/" + rRequest.ViewName.ToLower() + ".ab";

            var rAssetRequest = await ABLoader.Instance.LoadAsset(rUIABPath, rRequest.ViewName, ABPlatform.Instance.IsSumilateMode_GUI());
            if (rAssetRequest.Asset != null)
            {
                rRequest.ViewPrefabGo = rAssetRequest.Asset as GameObject;
            }
            return rRequest;
        }

        /// <summary>
        /// 卸载UI资源
        /// </summary>
        public void UnloadUI(string rViewName)
        {
            string rUIABPath = "game/ui/" + rViewName.ToLower() + ".ab";
            ABLoader.Instance.UnloadAsset(rUIABPath);
        }
    }
}
