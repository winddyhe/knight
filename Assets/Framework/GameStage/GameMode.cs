//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;

namespace Framework
{
    /// <summary>
    /// 游戏模式的基类
    /// </summary>
    public class GameMode
    {
        /// <summary>
        /// 得到当前的游戏模式
        /// </summary>
        public static Func<GameMode> GetCurrentMode;
    
        /// <summary>
        /// 游戏关卡管理器对象
        /// </summary>
        public GameStageManager      gsm;
    
        /// <summary>
        /// 初始化游戏数据
        /// </summary>
        public void InitData()
        {
            //设置GameStageManager
            this.gsm = GameStageManager.Instance;
    
            // 构建GameStages
            OnBuildStages();
        }
    
        protected virtual void OnBuildStages() { }
    }
}