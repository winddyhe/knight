//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using Core;
using Framework.Hotfix;

namespace Game.Knight
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    internal class Init : MonoBehaviour
    {
        public string HotfixModule = "";
        public string HotfixScript = string.Empty;

        IEnumerator Start()
        {
            //限帧
            Application.targetFrameRate = 60;

            GameLoading.Instance.StartLoading(1.0f, "游戏初始化阶段，开始加载资源...");

            //初始化协程管理器
            CoroutineManager.Instance.Initialize();

            //平台初始化
            AssetPlatformManager.Instance.Initialize();

            //加载Assetbundle的Manifest
            yield return AssetLoadManager.Instance.LoadManifest();

            // 加载热更新代码资源
            yield return HotfixManager.Instance.Load(HotfixModule);

            // 加载Hotfix端的代码
            IEnumerator rEnum = HotfixManager.Instance.App.InvokeStatic(HotfixScript, "Start_Async") as IEnumerator;
            yield return rEnum;

            //切换到Login场景
            var rLevelRequest = Globals.Instance.LoadLevel("Login");
            yield return rLevelRequest;
            
            Debug.LogError("End U3D init...");
        }
    }
}
