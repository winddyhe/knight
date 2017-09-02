using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.GUI;

namespace Game.Knight
{
    public class GameMainLogic
    {
        public UIViewManager UIViewMgr;

        public IEnumerator Initialize()
        {
            // UI Manager 初始化
            UIViewManager.Instance.Initialize();

            // 开始游戏Init流程
            yield return Init.Start_Async();

            Debug.Log("End hotfix initialize...");
        }

        public void Update()
        {
            // UI Manager的Update
            UIViewManager.Instance.Update();
        }
    }
}
