using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Game
{
    public class GameVersion : TSingleton<GameVersion>
    {
        private UrlInfo mServerUrlInfo;
        private UrlInfo mHotfixUrlInfo;
        private CdnFileInfo mCdnFileInfo;

        private int mMainVersionFirstCode;
        private int mMainVersionSecondCode;
        private int mMainVersionThirdCode;
        private long mPackageVersionCode;
        private long mHotfixVersionCode;

        public UrlInfo HotfixUrlInfo => this.mHotfixUrlInfo;

        private GameVersion()
        {
        }

        public async UniTask Initialize()
        {
            this.InitializeVersionCode();
            this.InitializeServerInfo();

#if USE_CDN_FILE
            await this.InitializeCdnFile();
#endif
            await this.InitializeHotfixInfo();
            
            LogManager.LogRelease($"FullVersion: {this.GetFullVersion()}");
        }

        private void InitializeVersionCode()
        {
            var rVersionStrs = Application.version.Split('.');
            if (rVersionStrs.Length < 2) return;

            if (rVersionStrs.Length >= 2)
            {
                this.mMainVersionFirstCode = Convert.ToInt32(rVersionStrs[0]);
                this.mMainVersionSecondCode = Convert.ToInt32(rVersionStrs[1]);
            }
            if (rVersionStrs.Length == 3)
            {
                this.mMainVersionThirdCode = Convert.ToInt32(rVersionStrs[2]);
            }
            var rPackageVersionText = Resources.Load<TextAsset>("GameVersion");
            this.mPackageVersionCode = Convert.ToInt64(rPackageVersionText.text);
        }

        private int GetPlatformCode()
        {
#if VERSION_PLATFORM_PC
            return 1500;
#elif VERSION_PLATFORM_ANDROID
            return 2000;
#elif VERSION_PLATFORM_IOS
            return 2500;
#else
            return 1000;
#endif
        }

        public string GetCdnVersion()
        {
            return $"{this.mMainVersionFirstCode}.{this.mMainVersionSecondCode}.{this.GetPlatformCode()}";
        }

        public string GetNetCode()
        {
#if VERSION_NET_LOCAL
            return "LC";
#elif VERSION_NET_LAN
            return "LAN";
#else
            return "LC";
#endif
        }

        public string GetFullVersion()
        {
            return $"{this.mMainVersionFirstCode}.{this.mMainVersionSecondCode}.{this.mMainVersionThirdCode}.{this.GetNetCode()}.{this.GetPlatformCode()}.{this.mPackageVersionCode}.{this.mHotfixVersionCode}";
        }

        /// <summary>
        /// 是否需要热更新
        /// </summary>
        public bool IsNeedHotfix()
        {
            return this.mHotfixVersionCode > this.mPackageVersionCode;
        }

        private void InitializeServerInfo() 
        {
            this.mServerUrlInfo = new UrlInfo();
#if VERSION_NET_LOCAL
            this.mServerUrlInfo.MainURL = "http://127.0.0.1:8080/download";
            this.mServerUrlInfo.SubURL = "http://127.0.0.1:8080/download";
#elif VERSION_NET_LAN
            this.mServerUrlInfo.MainURL = "http://10.10.1.227:8080/download";
            this.mServerUrlInfo.SubURL = "http://10.10.1.227:8080/download";
#else
            this.mServerUrlInfo.MainURL = "http://127.0.0.1:8080/download";
            this.mServerUrlInfo.SubURL = "http://127.0.0.1:8080/download";
#endif
        }

        private async UniTask InitializeCdnFile()
        {
            var rCdnVersionJson = $"{this.GetCdnVersion()}.json";
            var rCdnFileUrlInfo = new UrlInfo()
            {
                MainURL = this.mServerUrlInfo.MainURL + "/CdnFile/" + rCdnVersionJson,
                SubURL = this.mServerUrlInfo.SubURL + "/CdnFile/" + rCdnVersionJson
            };
            var rCdnFileContent = await WebRequestTool.DownloadContent(rCdnFileUrlInfo);
            if (string.IsNullOrEmpty(rCdnFileContent))
            {
                LogManager.LogError($"CdnFile is null: {rCdnFileUrlInfo.MainURL}, {rCdnFileUrlInfo.SubURL}");
                return;
            }
            this.mCdnFileInfo = JsonConvert.DeserializeObject<CdnFileInfo>(rCdnFileContent);
        }

        private async UniTask InitializeHotfixInfo()
        {
            if (this.mCdnFileInfo == null) return;

            var rHotfixVersionPath = "/" + this.mCdnFileInfo.ResPath + "version.txt";
            var rHotfixVersionUrlInfo = new UrlInfo()
            {
                MainURL = this.mServerUrlInfo.MainURL + rHotfixVersionPath,
                SubURL = this.mServerUrlInfo.SubURL + rHotfixVersionPath
            };
            LogManager.LogRelease(rHotfixVersionPath);
            var rHotfixVersion = await WebRequestTool.DownloadContent(rHotfixVersionUrlInfo);
            if (string.IsNullOrEmpty(rHotfixVersion))
            {
                LogManager.LogError($"HotfixVersion is null: {rHotfixVersionUrlInfo.MainURL}, {rHotfixVersionUrlInfo.SubURL} .");
                return;
            }
            var rHotfixVersionStrs = rHotfixVersion.Split('.');
            if (rHotfixVersionStrs.Length < 3)
            {
                LogManager.LogError($"HotfixVersion is error: {rHotfixVersion} .");
                return;
            }
            var rHotfixVersionFirstCode = Convert.ToInt32(rHotfixVersionStrs[0]);
            var rHotfixVersionSecondCode = Convert.ToInt32(rHotfixVersionStrs[1]);
            var rHotfixVersionThirdCode = Convert.ToInt64(rHotfixVersionStrs[2]);
            if (rHotfixVersionFirstCode != this.mMainVersionFirstCode || rHotfixVersionSecondCode != this.mMainVersionSecondCode)
            {
                LogManager.LogError($"HotfixVersion is error: {rHotfixVersion} .");
                return;
            }
            this.mHotfixVersionCode = rHotfixVersionThirdCode;
            LogManager.LogRelease($"HotfixVersionCode: {this.mHotfixVersionCode}");
            var rHotfixResPath = "/" + this.mCdnFileInfo.ResPath + "Res/" + this.mHotfixVersionCode;
            this.mHotfixUrlInfo = new UrlInfo()
            {
                MainURL = this.mServerUrlInfo.MainURL + rHotfixResPath,
                SubURL = this.mServerUrlInfo.SubURL + rHotfixResPath
            };
            LogManager.LogRelease($"HotfixUrlPath: {rHotfixVersionPath}");
        }
    }
}
