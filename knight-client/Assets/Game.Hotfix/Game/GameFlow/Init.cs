//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Knight.Framework;
using UnityEngine.UI;

namespace Game
{
    public class Init
    {
        public static async Task Start_Async()
        {
            // 加载GameConfig
            await GameConfig.Instance.Load("game/config/gameconfig/binary/gameconfig.ab", "GameConfig");
            // 加载图集配置
            await UIAtlasManager.Instance.Load("game/gui/textures/config.ab");

            //切换到Login场景
            await Login.Instance.Initialize();

            Debug.Log("End hotfix init...");
        }
    }
}
