//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Knight.Framework;

namespace Game
{
    public class Init
    {
        public static async Task Start_Async()
        {
            // 加载GameConfig
            await GameConfig.Instance.Load("game/gameconfig.ab", "GameConfig");

            //切换到Login场景
            await Login.Instance.Initialize();
            Debug.Log("End hotfix init...");
        }
    }
}
