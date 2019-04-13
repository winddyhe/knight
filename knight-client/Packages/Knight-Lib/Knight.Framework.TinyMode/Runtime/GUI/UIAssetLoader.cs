//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Knight.Core;
using System.Threading.Tasks;
using UnityFx.Async;
using Knight.Framework;

namespace Knight.Framework.TinyMode.UI
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
            string rUIABPath = "GUI/Prefabs/" + rRequest.ViewName;
            var rAssetRequest = await AssetLoader.Instance.LoadAsset(rUIABPath, typeof(GameObject));
            if (rAssetRequest.Asset != null)
            {
                rRequest.ViewPrefabGo = rAssetRequest.Asset as GameObject;
            }
            return rRequest;
        }
    }
}
