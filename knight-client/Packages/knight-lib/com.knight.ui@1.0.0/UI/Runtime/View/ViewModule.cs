using Cysharp.Threading.Tasks;
using Knight.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class ViewModuleConfig
    {
        public string Name;
        public ViewState ViewState;
        public Dictionary<string, ViewConfig> ViewConfigs;
    }

    /// <summary>
    /// View模块 一个ViewModule包含多个View
    /// </summary>
    public class ViewModule
    {
        private List<string> mPreloadViews;

        public ViewModuleConfig ModuleConfig;
        public Dictionary<string, View> Views;

        public void Initialize(string rName, ViewModuleConfig rViewModuleConfig)
        {
            this.mPreloadViews = new List<string>();

            this.ModuleConfig = rViewModuleConfig;   
            this.Views = new Dictionary<string, View>();
        }

        public async UniTask Preload()
        {
            var rViewLoadRequests = await ViewAssetLoader.Instance.LoadViewGroup(this.ModuleConfig.ViewConfigs.Keys);
            foreach (var rViewLoadRequest in rViewLoadRequests)
            {
                var rViewName = rViewLoadRequest.AssetName;
                var rViewConfig = this.ModuleConfig.ViewConfigs[rViewName];
                var rView = await this.CreateView(rViewConfig, rViewLoadRequest);
                if (rView == null)
                {
                    LogManager.LogError($"ViewModule: Failed to create view {rViewName}");
                    continue;
                }
                this.Views.Add(rViewName, rView);
                this.mPreloadViews.Add(rViewName);
            }
        }

        public void Unload()
        {
            foreach (var rViewPair in this.Views)
            {
                var rView = rViewPair.Value;
                UtilUnityTool.SafeDestroy(rView.gameObject);
            }
            ViewAssetLoader.Instance.UnloadViews(this.Views.Keys);
            this.Views.Clear();
            this.mPreloadViews.Clear();
        }

        public bool IsPreloaded(string rViewName)
        {
            return this.mPreloadViews.Contains(rViewName);
        }

        public async UniTask<View> LoadOne(string rViewName)
        {
            if (!this.ModuleConfig.ViewConfigs.TryGetValue(rViewName, out var rViewConfig))
            {
                LogManager.LogError($"ViewModule: Failed to find view config {rViewName}");
                return null;
            }
            // 已经加载过View 直接返回
            if (this.Views.TryGetValue(rViewName, out var rView))
            {
                return rView;
            }
            // 加载View资源
            var rViewLoadRequest = await ViewAssetLoader.Instance.LoadView(rViewName);
            if (rViewLoadRequest == null)
            {
                LogManager.LogError($"ViewModule: Failed to load view {rViewName}");
                return null;
            }
            rView = await this.CreateView(rViewConfig, rViewLoadRequest);
            if (rView == null)
            {
                LogManager.LogError($"ViewModule: Failed to create view {rViewName}");
                return null;
            }
            this.Views.Add(rViewName, rView);
            return rView;
        }

        public void UnloadOne(string rViewName)
        {
            if (!this.Views.TryGetValue(rViewName, out var rView))
            {
                LogManager.LogError($"ViewModule: Failed to find view {rViewName}");
                return;
            }
            UtilUnityTool.SafeDestroy(rView.gameObject);
            ViewAssetLoader.Instance.UnloadView(rViewName);
            this.Views.Remove(rViewName);
        }

        private async UniTask<View> CreateView(ViewConfig rViewConfig, AssetLoaderRequest<GameObject> rViewLoadRequest)
        {
            var rViewName = rViewConfig.Name;
            var rViewPrefab = rViewLoadRequest.Asset;
            if (rViewPrefab == null)
            {
                LogManager.LogError($"ViewPreloader: Failed to load view {rViewName}");
                return null;
            }

            // 异步实例化
            var rInstantiateRequest = InstantiateManager.Instance.CreateAsync<GameObject>(rViewPrefab, 1);
            await rInstantiateRequest.Task();
            var rViewGo = rInstantiateRequest.Result;
            var rView = rViewGo.GetComponent<View>();
            if (rView == null)
            {
                LogManager.LogError($"ViewPreloader: Failed to get view component from {rViewName}");
                UtilUnityTool.SafeDestroy(rViewGo);
                return null;
            }

            // 获取ViewController
            var rViewControllerDataSource = rViewGo.GetComponent<ViewControllerDataSource>();
            var rViewControllerType = TypeResolveManager.Instance.GetType(rViewControllerDataSource.ViewControllerClass);
            var rViewController = (ViewController)ReflectTool.Construct(rViewControllerType);
            if (rViewControllerType == null || rViewController == null)
            {
                LogManager.LogError($"ViewPreloader: Failed to construct view controller {rViewControllerDataSource.ViewControllerClass}");
                UtilUnityTool.SafeDestroy(rViewGo);
                return null;
            }
            rView.ViewModuleName = this.ModuleConfig.Name;
            // 设置ViewConfig
            rView.ViewConfig = rViewConfig;
            // 设置View状态
            rView.SetViewLayer(rViewConfig.Layer);

            // 初始化View
            rView.Initialize(rViewController);
            rView.SetActive(false);
            rViewController.View = rView;

            return rView;
        }
    }
}
