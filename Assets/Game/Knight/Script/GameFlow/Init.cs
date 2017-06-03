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

namespace Game.Knight
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    internal class Init : MonoBehaviour
    {
        public string HotfixModule = "";
        public string HotfixScript = string.Empty;

        void Start()
        {
            //限帧
            Application.targetFrameRate = 30;

            GameLoading.Instance.StartLoading(1.0f, "游戏初始化阶段，开始加载资源...");

            //初始化协程管理器
            CoroutineManager.Instance.Initialize();

            //异步初始化代码
            CoroutineManager.Instance.Start(Start_Async());
        }

        private IEnumerator Start_Async()
        {
            // 平台初始化
            yield return ABPlatform.Instance.Initialize();

            // 资源下载模块初始化
            yield return ABUpdater.Instance.Initialize();
            
            // 加载热更新代码资源
            yield return HotfixApp.Instance.Load(HotfixModule);

            // 加载Hotfix端的代码
            yield return HotfixApp.Instance.InvokeStatic(HotfixScript, "Start_Async") as IEnumerator;

            Debug.Log("End init..");
        }
    }
}
