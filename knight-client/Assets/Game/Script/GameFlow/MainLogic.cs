using Cysharp.Threading.Tasks;
using Knight.Core;
using Knight.Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class MainLogic : TSingleton<MainLogic>
    {
        private bool mIsInitialized = false;

        private MainLogic()
        {
        }

        public async Task Initialize()
        {
            this.mIsInitialized = false;
            HotfixMainLogicManager.Instance = HotfixManager.Instance.Instantiate<IHotfixMainLogic>("Game.Hotfix", "Game.HotfixMainLogic");
            await HotfixMainLogicManager.Instance.Initialize();
            this.mIsInitialized = true;
        }

        public void Update(float fDeltaTime)
        {
            if (!this.mIsInitialized) return;
            HotfixMainLogicManager.Instance?.Update(fDeltaTime);
        }

        public void LateUpdate(float fDeltaTime)
        {
            if (!this.mIsInitialized) return;
            HotfixMainLogicManager.Instance?.LateUpdate(fDeltaTime);
        }

        public void Destroy()
        {
            HotfixMainLogicManager.Instance?.Destroy();
            this.mIsInitialized = false;
        }
    }
}
