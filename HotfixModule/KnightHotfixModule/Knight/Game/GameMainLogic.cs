using System.Collections;
using UnityEngine;
using WindHotfix.GUI;
using Framework.Hotfix;
using WindHotfix.Game;

namespace Game.Knight
{
    public class GameMainLogic : GameLogicBase
    {
        public ViewManager UIViewMgr;

        public override IEnumerator Initialize()
        {
            yield return base.Initialize();

            // 事件管理器
            HotfixEventManager.Instance.Initialize();

            // UI Manager 初始化
            ViewManager.Instance.Initialize();

            // 开始游戏Init流程
            yield return Init.Start_Async(); 

            Debug.Log("End hotfix initialize...");
        }

        public override void Update()
        {
            // UI Manager的Update
            ViewManager.Instance.Update();
        }
    }
}
