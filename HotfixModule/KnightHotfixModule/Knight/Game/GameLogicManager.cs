using System.Collections;
using UnityEngine;
using WindHotfix.GUI;
using Framework.Hotfix;
using WindHotfix.Game;

namespace Game.Knight
{
    public class GameLogicManager
    {
        public IEnumerator Initialize()
        {
            // [0] 其他模块的初始化
            // 事件模块管理器
            HotfixEventManager.Instance.Initialize();

            // UI模块管理器
            ViewManager.Instance.Initialize();
            
            // 开始游戏Init流程
            yield return Init.Start_Async(); 

            Debug.Log("End hotfix initialize...");
        }

        public void Update()
        {
            // UI的模块更新逻辑
            ViewManager.Instance.Update();
        }
    }
}
