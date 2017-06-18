//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.UI;
using Framework;
using System;

namespace Game.Knight
{
    public class LoadingView_Knight : MonoBehaviour, ILoadingView
    {
        private static LoadingView_Knight   __instance;
        public  static LoadingView_Knight   Instance    { get { return __instance; } }

        /// <summary>
        /// 背景图片，可能有需要切换的情况
        /// </summary>
        public Image    Background;
        /// <summary>
        /// 加载的进度条
        /// </summary>
        public Slider   LoadingBar;
        /// <summary>
        /// 加载时候的一些文字提示
        /// </summary>
        public Text     TextTips;

        void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public void SetTips(string rTips)
        {
            this.TextTips.text = rTips;
        }

        public void SetLoadingProgress(float rValue)
        {
            this.LoadingBar.value = rValue;
        }
        
        /// <summary>
        /// 开始出现加载界面
        /// </summary>
        public void ShowLoading(float rIntervalTime, string rTextTips = "")
        {
            this.gameObject.SetActive(true);
            
            this.SetTips(rTextTips);
            this.SetLoadingProgress(0);

            this.StartCoroutine(LoadingProgress(rIntervalTime));
        }

        /// <summary>
        /// 开始出现加载界面
        /// </summary>
        public void ShowLoading(string rTextTips)
        {
            this.gameObject.SetActive(true);

            this.SetTips(rTextTips);
            this.SetLoadingProgress(0);
        }
        
        /// <summary>
        /// 加载界面
        /// </summary>
        public void HideLoading()
        {
            this.SetLoadingProgress(1);
            this.gameObject.SetActive(false);
            this.SetTips("");
        }
        
        private IEnumerator LoadingProgress(float rIntervalTime)
        {
            float rLoadingTime = rIntervalTime * 0.9f;

            float rCurTime = 0;
            while (rCurTime <= rLoadingTime)
            {
                this.SetLoadingProgress(rCurTime / rIntervalTime);
                yield return 0;
                rCurTime += Time.deltaTime;
            }
        }
    }
}
