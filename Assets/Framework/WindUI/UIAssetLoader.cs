//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.AssetBundles;

namespace Framework.WindUI
{
    /// <summary>
    /// UI加载器
    /// </summary>
    public class UIAssetLoader : TSingleton<UIAssetLoader>
    {
        public class LoaderRequest : CoroutineRequest<LoaderRequest>
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
        public LoaderRequest LoadUI(string rViewName)
        {
            LoaderRequest rRequest = new LoaderRequest(rViewName);
            rRequest.Start(LoadUI_Async(rRequest));

            return rRequest;
        }
    
        /// <summary>
        /// 根据名字异步加载UI
        /// </summary>
        private IEnumerator LoadUI_Async(LoaderRequest rRequest)
        {
            string rUIABPath = "game/ui/" + rRequest.ViewName.ToLower() + ".ab";
            var rAssetRequest = ABLoader.Instance.LoadAsset(rUIABPath, rRequest.ViewName, ABPlatform.Instance.IsSumilateMode_GUI());
            yield return rAssetRequest;

            if (rAssetRequest.Asset != null)
            {
                rRequest.ViewPrefabGo = rAssetRequest.Asset as GameObject;
            }
            ABLoader.Instance.UnloadAsset(rUIABPath);
        }
    }
}
