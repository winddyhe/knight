//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Core;
using System.Threading.Tasks;

namespace WindHotfix.GameStage
{
    /// <summary>
    /// 一个Stage中的一个Task
    /// </summary>
    public class StageTask
    {
        /// <summary>
        /// 当前的Stage是否完成
        /// </summary>
        public bool                     isCompleted = false;
        /// <summary>
        /// Stage的名字
        /// </summary>
        public string                   name = "";
    
        /// <summary>
        /// 初始化GameStage
        /// </summary>
        public bool Init() 
        {
            if (!OnInit())
            {
                Debug.LogErrorFormat("GameStage {0} Init Failed.", this.name);
                return false;
            }
            return true; 
        }
    
        /// <summary>
        /// 开始执行GameStage
        /// </summary>
        public async Task Run_Async()  
        {
            isCompleted = false;

            await OnRun_Async();
    
            isCompleted = true;
        }
    
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual bool OnInit() { return true; }
    
        /// <summary>
        /// 执行GameStage
        /// </summary>
        protected virtual async Task OnRun_Async() { }
    }
    
    /// <summary>
    /// 一个GameStage包含很多个StageTask
    /// </summary>
    public class GameStage
    {
        /// <summary>
        /// 一个Stage由一组Task构成的。
        /// </summary>
        public List<StageTask>  taskList;
    
        /// <summary>
        /// Stage的索引
        /// </summary>
        public int              index;
    
        /// <summary>
        /// 该Stage是否已经完成。
        /// </summary>
        public bool             isStageCompleted = false;
    
        /// <summary>
        /// GameStage的初始化
        /// </summary>
        public void Init()
        {
            this.isStageCompleted = false;
            for (int i = 0; i < taskList.Count; i++)
            {
                taskList[i].Init();
            }
        }
    
        /// <summary>
        /// 开始异步执行GameStage
        /// </summary>
        public async Task Run_Async()
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                taskList[i].Run_Async();
            }
            
            //等待这个索引的所有的Task执行完成后，才进入下一个索引
            while (!CheckStageIsCompleted())
            {
                await new WaitForEndOfFrame();
            }
            this.isStageCompleted = true;
        }
    
        /// <summary>
        /// 检查这个Stage中所有的Task是否全部完成。
        /// </summary>
        private bool CheckStageIsCompleted()
        {
            if (taskList == null) return true;
    
            bool isAllCompleted = true;
            for (int i = 0; i < taskList.Count; i++)
            {
                isAllCompleted &= taskList[i].isCompleted;
            }
            return isAllCompleted;
        }
    }
}