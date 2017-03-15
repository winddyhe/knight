//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using Core;

namespace Game.Knight
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    public class Init : MonoBehaviour
    {
        void Start()
        {
            //限帧
            Application.targetFrameRate = 60;

            //开始异步初始化
            this.StartCoroutine(Init_Async());
        }

        private IEnumerator Init_Async()
        {
            GameLoading.Instance.StartLoading(1.0f, "游戏初始化阶段，开始加载资源...");

            //初始化协程管理器
            CoroutineManager.Instance.Initialize();

            //平台初始化
            AssetPlatformManager.Instance.Initialize();

            //加载Assetbundle的Manifest
            yield return AssetLoadManager.Instance.LoadManifest();

            //加载游戏配置初始化
            yield return GameConfig.Instance.Load("game/gameconfig.ab", "GameConfig");
            GameConfig.Instance.Unload("game/gameconfig.ab");

            // 加载技能配置
            yield return GPCSkillConfig.Instance.Load("game/skillconfig.ab", "SkillConfig");
            GPCSkillConfig.Instance.Unload("game/skillconfig.ab");

            // 加载热更新代码资源
            yield return HotfixManager.Instance.Load("KnightHotfixModule");
            
            //切换到Login场景
            var rLevelRequest = Globals.Instance.LoadLevel("Login");
            yield return rLevelRequest.Coroutine;
        }
    }
}
