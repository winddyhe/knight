//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;

namespace Framework.WindUI
{
    /// <summary>
    /// UI加载器
    /// </summary>
    public class UILoader : MonoBehaviour
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

        private static UILoader __instance;
        public  static UILoader Instance { get{ return __instance; }}
        
        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }
    
        /// <summary>
        /// 异步加载UI
        /// </summary>
        public LoaderRequest LoadUI(string rViewName)
        {
            LoaderRequest rLoaderRequest = new LoaderRequest(rViewName);
            rLoaderRequest.Start(LoadUI_Async(rLoaderRequest));

            return rLoaderRequest;
        }
    
        /// <summary>
        /// 根据名字异步加载UI
        /// </summary>
        private IEnumerator LoadUI_Async(LoaderRequest rLoaderRequest)
        {
            var rRequest = Resources.LoadAsync(rLoaderRequest.ViewName, typeof(GameObject));
            yield return rRequest;
    
            if (rRequest == null || rRequest.asset == null)
            {
                Debug.LogErrorFormat("Not found UI {0}.", rLoaderRequest.ViewName);
                yield break;
            }
            rLoaderRequest.ViewPrefabGo = rRequest.asset as GameObject;
        }
    }
}
