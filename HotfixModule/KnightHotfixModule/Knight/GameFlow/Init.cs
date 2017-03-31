using System;
using System.Collections.Generic;
using Framework.Hotfix;
using UnityEngine;
using System.Collections;
using Game.Knight;

namespace KnightHotfixModule.Knight.GameFlow
{
    public class Init
    {
        public static IEnumerator Start_Async()
        {
            //加载游戏配置初始化
            yield return GameConfig.Instance.Load("game/gameconfig.ab", "GameConfig");
            GameConfig.Instance.Unload("game/gameconfig.ab");

            // 加载技能配置
            yield return GPCSkillConfig.Instance.Load("game/skillconfig.ab", "SkillConfig");
            GPCSkillConfig.Instance.Unload("game/skillconfig.ab");

            //切换到Login场景
            var rSceneRequest = SceneAssetLoader.Instance.Load_Async(
                "game/scene/login.ab", 
                "Assets/Game/Knight/GameAsset/Scene/Login.unity",
                UnityEngine.SceneManagement.LoadSceneMode.Single);
            yield return rSceneRequest.Coroutine;
        }
    }
}
