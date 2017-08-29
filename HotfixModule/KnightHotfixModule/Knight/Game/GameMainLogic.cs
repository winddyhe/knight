using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Knight
{
    public class GameMainLogic
    {
        public IEnumerator Initialize()
        {
            // 开始游戏Init流程
            yield return Init.Start_Async();

            Debug.Log("End hotfix initialize...");
        }

        public void Update()
        {

        }
    }
}
