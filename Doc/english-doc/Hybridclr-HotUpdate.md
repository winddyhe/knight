# hybridclr Hot Update
* A fully featured, zero-cost, high-performance, low-memory native C# hot update solution for Unity across all platforms.
* The address for the hybridclr library: https://github.com/focus-creative-games/hybridclr

## Hot Update Module
* The code for the hot update module is located in the Knight.Framework.Hotfix package.
* Hot update functionality is implemented through both hybridclr and reflection calls, and these two modes can be switched with a single toggle.

## Hot Update Logic
* The game hot update logic is written in Game.Hotfix.dll.
* Using hybridclr for C# hot updates allows game logic to be written in standard C# code without other syntactical restrictions, such as using MonoBehaviour or cross-domain inheritance, etc. Therefore, the framework layer does not need to provide special encapsulations to implement certain logic.
* The entry code for the hot update:
  ```C#
    // 主工程Game.dll的调用入口
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

    // 热更新工程Game.Hotfix.dll的调用入口
    public class HotfixMainLogic : IHotfixMainLogic
    {
        public async UniTask Initialize()
        {
        }
        public void Destroy()
        {
        }
        public void LateUpdate(float fDeltaTime)
        {
        }
        public void Update(float fDeltaTime)
        {
        }
    }
  ```