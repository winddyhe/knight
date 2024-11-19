using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Knight.Core;

namespace Knight.Framework.UI
{
    public class ViewManager : TSingleton<ViewManager>
    {
        private ViewAssetConfig mViewPreloader;

        private Dictionary<Guid, View> mCurViews;
        private Dictionary<string, ViewModule> mViewModules;

        private List<string> mOpenedModules;
        private List<ViewBackCache> mBackCaches;

        private ViewManager()
        {
        }

        public void Initialize(ViewAssetConfig rViewPreloader)
        {
            this.mViewPreloader = rViewPreloader;
            this.mViewPreloader.Initialize();

            this.mViewModules = new Dictionary<string, ViewModule>();
            foreach (var rViewModuleConfigPair in this.mViewPreloader.ViewModuleConfigs)
            {
                var rViewModule = new ViewModule();
                rViewModule.Initialize(rViewModuleConfigPair.Key, rViewModuleConfigPair.Value);
                this.mViewModules.Add(rViewModule.ModuleConfig.Name, rViewModule);
            }

            this.mCurViews = new Dictionary<Guid, View>();
            this.mOpenedModules = new List<string>();
            this.mBackCaches = new List<ViewBackCache>();
        }

        public async UniTask<View> Open(string rViewModuleName, string rViewName, ViewLayer rViewLayer, int nCanvasOrder)
        {
            // 尝试切换模块
            this.TryToSwitchViewModule(rViewModuleName);

            // 加载View
            var rView = await this.LoadView(rViewModuleName, rViewName);
            if (rView == null)
            {
                return null;
            }

            // 记录当前模块
            if (this.mOpenedModules.Count == 0)
            {
                this.mOpenedModules.Add(rViewModuleName);
            }
            else
            {
                var rLastOpenedModuleName = this.mOpenedModules[this.mOpenedModules.Count - 1];
                if (rLastOpenedModuleName != rViewModuleName)
                {
                    this.mOpenedModules.Add(rViewModuleName);
                }
            }

            // 生成GUID
            rView.GUID = Guid.NewGuid();
            this.mCurViews.Add(rView.GUID, rView);

            if (rView.ViewConfig.Layer != rViewLayer)
            {
                rView.SetViewLayer(rViewLayer);
            }

            // 打开View
            rView.SetActive(true);
            rView.SetCanvasOrder(rViewLayer, nCanvasOrder);
            // 绑定ViewModel的数据
            rView.Bind();
            await rView.Open();

            // 缓存View的状态，用来做回退操作
            this.mBackCaches.Add(new ViewBackCache() 
            {
                GUID = rView.GUID,
                ViewModuleName = rView.ViewModuleName,
                ViewName = rView.ViewConfig.Name,
                ViewLayer = rViewLayer,
                CanvasOrder = nCanvasOrder
            });

            return rView;
        }

        public async UniTask<View> Back()
        {
            if (this.mBackCaches.Count == 0)
            {
                LogManager.LogWarning("Back cache is empty, can not back.");
                return null;
            }

            // 获取最后一个缓存的View，他是当前View的状态
            var rCurViewCache = this.mBackCaches[this.mBackCaches.Count - 1];
            // 关闭当前View
            this.Close(rCurViewCache.GUID);
            this.mBackCaches.RemoveAt(this.mBackCaches.Count - 1);
            // 当前只有一个Cache View，则不能回退
            if (this.mBackCaches.Count == 0)
            {
                LogManager.LogWarning("Current view is the first view, can not back.");
                return null;
            }

            // 获取上一个缓存的View，他是要回退的View的状态
            var rBackViewCache = this.mBackCaches[this.mBackCaches.Count - 1];
            // 打开上一个View
            var rBackView = await this.Open(rBackViewCache.ViewModuleName, rBackViewCache.ViewName, rBackViewCache.ViewLayer, rBackViewCache.CanvasOrder);
            return rBackView;
        }

        public void Close(Guid rViewGuid)
        {
            if (this.mCurViews.TryGetValue(rViewGuid, out var rView))
            {
                rView.SetActive(false);
                rView.UnBind();
                rView.Close();

                // 如果不是预加载的UI，那么要把资源卸载掉
                var rViewModule = this.mViewModules[rView.ViewModuleName];
                if (!rViewModule.IsPreloaded(rView.ViewConfig.Name))
                {
                    rViewModule.UnloadOne(rView.ViewConfig.Name);
                }
            }
        }

        public async UniTask PrelaodViewModules(string rViewModuleName)
        {
            if (!this.mViewModules.TryGetValue(rViewModuleName, out var rViewModule))
            {
                LogManager.LogErrorFormat("ViewModule {0} not found.", rViewModuleName);
                return;
            }
            await rViewModule.Preload();
        }

        public void UnloadViewModules(string rViewModuleName)
        {
            if (!this.mViewModules.TryGetValue(rViewModuleName, out var rViewModule))
            {
                LogManager.LogErrorFormat("ViewModule {0} not found.", rViewModuleName);
                return;
            }
            rViewModule.Unload();
        }

        public async UniTask<View> LoadView(string rViewModuleName, string rViewName)
        {
            if (!this.mViewModules.TryGetValue(rViewModuleName, out var rViewModule))
            {
                LogManager.LogErrorFormat("ViewModule {0} not found.", rViewModuleName);
                return null;
            }
            var rView = await rViewModule.LoadOne(rViewName);
            return rView;
        }

        public void UnloadView(View rView)
        {
            if (rView == null) return;

            if (!this.mViewModules.TryGetValue(rView.ViewModuleName, out var rViewModule))
            {
                LogManager.LogErrorFormat("ViewModule {0} not found.", rView.ViewModuleName);
                return;
            }
            rViewModule.UnloadOne(rView.ViewConfig.Name);
        }

        private void TryToSwitchViewModule(string rViewModuleName)
        {
            if (this.mOpenedModules.Count == 0) return;

            var rLastViewModuleName = this.mOpenedModules[this.mOpenedModules.Count - 1];
            // 如果当前模块与上一个模块相同，则不切换
            if (rLastViewModuleName == rViewModuleName)
            {
                return;
            }

            var rLastViewModule = this.mViewModules[rLastViewModuleName];
            var rCurViewModule = this.mViewModules[rViewModuleName];

            // 如果当前模块的状态为PageSwitch，则关闭上一个模块的所有View
            if (rCurViewModule.ModuleConfig.ViewState == ViewState.PageSwitch)
            {
                foreach (var rPair in rLastViewModule.Views)
                {
                    var rView = rPair.Value;
                    this.Close(rView.GUID);
                }
                this.mOpenedModules.Remove(rLastViewModuleName);
            }
        }
    }
}
