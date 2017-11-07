//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using Core;
using Framework.Hotfix;
using UnityEngine.AssetBundles;
using Framework.WindUI;
using System.Threading.Tasks;

namespace Game.Knight
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    internal class Init : MonoBehaviour
    {
        public string HotfixABPath = "";
        public string HotfixModule = "";
        
        async void Start()
        {
            //限帧
            Application.targetFrameRate = 30;

            // 初始化热更新模块
            HotfixManager.Instance.Initialize();

            // 初始化UI管理器
            UIRoot.Instance.Initialize();

            // 初始化加载进度条
            GameLoading.Instance.LoadingView = LoadingView_Knight.Instance;
            GameLoading.Instance.StartLoading(0.5f, "游戏初始化阶段，开始加载资源...");

            //初始化协程管理器
            CoroutineManager.Instance.Initialize();

            //异步初始化代码
            await Start_Async();
        }

        private async Task Start_Async()
        {
            // 平台初始化
            await ABPlatform.Instance.Initialize();
            
            // 资源下载模块初始化
            await ABUpdater.Instance.Initialize();
            
            GameLoading.Instance.Hide();
            GameLoading.Instance.StartLoading(1.0f, "游戏初始化阶段，开始加载资源...");

            // 加载热更新代码资源
            await HotfixManager.Instance.Load(this.HotfixABPath, this.HotfixModule);

            // 开始热更新端的游戏主逻辑
            await HotfixGameMainLogic.Instance.Initialize();

            Debug.Log("End init..");
        }
    }
}
