using System.Collections;
using UnityEngine;
using WindHotfix.GUI;
using Framework.Hotfix;
using WindHotfix.Game;

namespace Game.Knight
{
    public class LogicSystemManagerSGX : LogicSystemManager
    {
        public override IEnumerator Initialize()
        {
            yield return base.Initialize();

            // [0] 其他模块的初始化
            // 事件模块管理器
            HotfixEventManager.Instance.Initialize();

            // UI模块管理器
            ViewManager.Instance.Initialize();

            // 注册子系统
            this.RegisterSystems();
            
            // 开始游戏Init流程
            yield return Init.Start_Async(); 

            Debug.Log("End hotfix initialize...");
        }

        public void RegisterSystems()
        {

        }

        public override void Update()
        {
            // UI的模块更新逻辑
            ViewManager.Instance.Update();

            base.Update();
        }
    }
}
