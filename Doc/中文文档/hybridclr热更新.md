# hybridclr热更新
* 是一个特性完整、零成本、高性能、低内存的Unity全平台原生c#热更新解决方案。
* hybridclr库的地址：https://github.com/focus-creative-games/hybridclr

## 热更新模块
* 热更新模块的代码在Knight.Framework.Hotfix的Package中。
* 分别通过hybridclr和反射调用实现了热更新功能，并且这两种模式可以通过一个开关一键切换。

## 热更逻辑
* 游戏热更新逻辑写在Game.Hotfix.dll中。
* 使用hybridclr进行c#的热更新，让游戏逻辑就正常写c#代码即可，不再有其他语法的限制，例如是用Monobehaviour或者跨域继承等等均可直接使用。因此框架层也无需做特殊封装来实现一些逻辑使用。
* 热更新的入口代码：
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
