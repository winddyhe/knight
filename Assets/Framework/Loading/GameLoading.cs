//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public class GameLoading : MonoBehaviour
    {
        private static GameLoading __instance;
        public static GameLoading Instance { get { return __instance; } }

        /// <summary>
        /// 加载界面
        /// </summary>
        public ILoadingView LoadingView;

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        public void StartLoading(float rIntervalTime, string rTextTips = "")
        {
            this.LoadingView.ShowLoading(rIntervalTime, rTextTips);
        }

        public void StartLoading(string rTextTips = "")
        {
            this.LoadingView.ShowLoading(rTextTips);
        }

        public void SetTips(string rTextTips)
        {
            this.LoadingView.SetTips(rTextTips);
        }

        public void SetLoadingProgress(float fProgressValue)
        {
            this.LoadingView.SetLoadingProgress(fProgressValue);
        }
        
        public void Hide()
        {
            this.LoadingView.HideLoading();
        }
    }
}


