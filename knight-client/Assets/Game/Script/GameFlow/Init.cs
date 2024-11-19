using Knight.Core;
using Knight.Framework.Assetbundle;
using Knight.Framework.Hotfix;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Framework.UI;

namespace Game
{
    public class Init : MonoBehaviour
    {
        private async void Start()
        {
            // 分辨率设置
            RenderResolutionSetting.Initialize();
            RenderResolutionSetting.SetResolution(720);

            Application.runInBackground = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 1000;
            Time.fixedDeltaTime = 1.0f / 30;

            // 初始化各种数据和管理器
            // 版本数据初始化
            await GameVersion.Instance.Initialize();

            // 平台路径初始化
            ABPlatform.Instance.Initialize(GameVersion.Instance.HotfixUrlInfo, GameVersion.Instance.GetCdnVersion());

            // 异步实例化管理器初始化
            InstantiateManager.Instance.Initialize();

            // 初始化红点管理器
            RedPointManager.Instance.Initialize();

            // 资源加载器初始化
            AssetLoader.Instance = new ABLoaderManager();
            AssetLoader.Instance.Initialize(
                "Assets/Game.Editor/Editor/Assetbundle/ABSimulateConfig.asset", "Assets/Game.Editor/Editor/Assetbundle/ABBuilderConfig.asset");

            // 下载器初始化
            await ABUpdater.Instance.Initialize();
            // 开始检测并下载资源
            await ABUpdater.Instance.InitializeUpdate(this.UpdateCheck, this.UpdateProgress);

            // 初始化热更新
            var rDLLNames = new string[] { "Game.Config", "Game.Hotfix" };
            HotfixManager.Instance.Initialize(rDLLNames);
            await HotfixManager.Instance.Load("game/hotfix/libs.ab");
            await HotfixManager.Instance.LoadAOTAsms("game/hotfix/aotasms.ab");

            // 类型管理器初始化
            TypeResolveManager.Instance.Initialize();
            TypeResolveManager.Instance.AddAssembly("Game");
            TypeResolveManager.Instance.AddAssembly("Game.Config", true);
            TypeResolveManager.Instance.AddAssembly("Game.Hotfix", true);

            // 开始主逻辑初始化
            await MainLogic.Instance.Initialize(); 
        }

        public void Update()
        {
            InstantiateManager.Instance?.Update();
            RedPointManager.Instance?.Update();
            AssetLoader.Instance?.Update();
            MainLogic.Instance?.Update(Time.deltaTime);
        }

        public void LateUpdate()
        {
            MainLogic.Instance?.LateUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            MainLogic.Instance?.Destroy();
        }

#pragma warning disable 1998
        private async Task<bool> UpdateCheck(int nFileCount, long nTotalSize)
        {
            LogManager.LogRelease($"Total Update File Count: {nFileCount}, Total Update File Size: {nTotalSize}");
            return true;
        }
#pragma warning restore 1998

        private void UpdateProgress(long nTatalSize, long nCurDownloadSize, long nABSize, float fUpdateProgress)
        {
            var fTotalProgress = (double)(nCurDownloadSize + (nABSize * fUpdateProgress)) / (double)nTatalSize;
            LogManager.LogRelease("TotalProgress: " + fTotalProgress);
        }
    }
}
