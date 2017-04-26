//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Game.Knight;

namespace KnightHotfixModule.Knight
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
            yield return Globals.Instance.LoadLevel("Login");

            Debug.Log("End hotfix init...");
        }
    }
}
