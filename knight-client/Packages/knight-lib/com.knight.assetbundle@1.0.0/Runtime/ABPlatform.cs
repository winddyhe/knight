using System.IO;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.Assetbundle
{
    /// <summary>
    /// AssetBundle 平台类型
    /// </summary>
    public enum ABPlatformType
    {
        OSXEditor = 0,
        OSXPlayer,
        WindowsPlayer,
        WindowsEditor,
        IphonePlayer,
        Android,
        Unkown,
    }

    public class ABPlatform : TSingleton<ABPlatform>
    {
        /// <summary>
        /// 平台名称
        /// </summary>
        public static string[] PlatformNames = new string[]
        {
            "OSX",
            "OSX",
            "Windows",
            "Windows",
            "IOS",
            "Android",
            "Unkown",
        };

        /// <summary>
        /// 平台是否为编辑器
        /// </summary>
        public static bool[] PlatformIsEditor = new bool[]
        {
            true,
            false,
            false,
            true,
            false,
            false,
            false,
        };

        /// <summary>
        /// 平台前缀
        /// </summary>
        public static string[] PlatformPrefix = new string[]
        {
            "file:///",         //OSXEditor
            "file:///",         //OSXPlayer
            "file:///",         //WindowsPlayer
            "file:///",         //WindowsEditor
            "file://",          //IphonePlayer
            "",                 //Android
            "file:///",         //Unkown
        };

        // 当前平台
        public ABPlatformType CurRuntimePlatform { get; private set; }
        public UrlInfo ServerURLInfo { get; private set; }
        public string CdnVersionCode { get; private set; }

        private ABPlatform()
        {
        }

        public void Initialize()
        {
            this.CurRuntimePlatform = RuntimePlafromToABPlatform(Application.platform);
            this.ServerURLInfo = null;
            this.CdnVersionCode = string.Empty;
        }

        public void Initialize(UrlInfo rServerUrlInfo, string rCdnVersionCode)
        {
            this.CurRuntimePlatform = RuntimePlafromToABPlatform(Application.platform);
            this.ServerURLInfo = rServerUrlInfo;
            this.CdnVersionCode = rCdnVersionCode;
        }

        public string GetCurPlatformABDir()
        {
            return this.GetCurPlatformABDir(this.CurRuntimePlatform);
        }

        public string GetCurPlatformABDir(ABPlatformType rPlatformType)
        {
            int nPlatformIndex = (int)rPlatformType;
            string rPlatformName = PlatformNames[nPlatformIndex];
#if UNITY_ANDROID && UNITY_EDITOR
            // windows editor Android平台需要读取Android版本的AB包
            rPlatformName = "Android";
#endif
            return "Assetbundles/" + rPlatformName + "_Assetbundles/";      
        }

        public string GetStreamingFile(ABPlatformType rPlatformType, string rFileName)
        {
            int nPlatformIndex = (int)rPlatformType;
            bool bIsEditor = PlatformIsEditor[nPlatformIndex];
            string rRootDir = bIsEditor ? PathTool.GetParentPath(Application.dataPath) : Application.streamingAssetsPath;
            if (!AssetLoader.Instance.IsHotfixDebugMode)
            {
                return rRootDir + "/" + this.GetCurPlatformABDir(rPlatformType) + rFileName;
            }
            else
            {
                return rRootDir + "/" + this.GetCurPlatformABDir(rPlatformType) + "/history/base/" + rFileName;
            }
        }

        public string GetRealStreamingFile(ABPlatformType rPlatformType, string rFileName)
        {
            string rRootDir = Application.streamingAssetsPath;
            return rRootDir + "/" + this.GetCurPlatformABDir(rPlatformType) + rFileName;
        }

        public string GetStreamingFile_CurPlatform(string rFileName)
        {
            return this.GetStreamingFile(this.CurRuntimePlatform, rFileName);
        }

        public string GetRealStreamingFile_CurPlatform(string rFileName)
        {
            return this.GetRealStreamingFile(this.CurRuntimePlatform, rFileName);
        }

        public string GetStreamingUrl(ABPlatformType rPlatformType, string rFileName)
        {
            int nPlatformIndex = (int)rPlatformType;
            return PlatformPrefix[nPlatformIndex] + this.GetStreamingFile(rPlatformType, rFileName);
        }

        public string GetRealStreamingUrl(ABPlatformType rPlatformType, string rFileName)
        {
            int nPlatformIndex = (int)rPlatformType;
            return PlatformPrefix[nPlatformIndex] + this.GetRealStreamingFile(rPlatformType, rFileName);
        }

        public string GetStreamingUrl_CurPlatform(string rFileName)
        {
            return this.GetStreamingUrl(this.CurRuntimePlatform, rFileName);
        }

        public string GetRealStreamingUrl_CurPlatform(string rFileName)
        {
            return this.GetRealStreamingUrl(this.CurRuntimePlatform, rFileName);
        }

        public string GetStreamingUrl_Manifest_CurPlatform()
        {
            string rRootPath = this.GetStreamingUrl_CurPlatform("");
            var rDirInfo = new DirectoryInfo(rRootPath);
            return rRootPath + rDirInfo.Name;
        }

        public string GetPersistentFile(ABPlatformType rPlatformType, string rResPrefix, string rFileName)
        {
            if (!string.IsNullOrEmpty(rResPrefix))
                return PathTool.Combine(Application.persistentDataPath, rResPrefix, this.GetCurPlatformABDir(rPlatformType), rFileName);
            else
                return PathTool.Combine(Application.persistentDataPath, this.GetCurPlatformABDir(rPlatformType), rFileName);
        }

        public string GetPersistentFile_CurPlatform(string rResPrefix, string rFileName)
        {
            return this.GetPersistentFile(this.CurRuntimePlatform, rResPrefix, rFileName);
        }

        public string GetPersistentUrl(ABPlatformType rPlatformType, string rResPrefix, string rFileName)
        {
            int nPlatformIndex = (int)rPlatformType;
            return PlatformPrefix[nPlatformIndex] + this.GetPersistentFile(rPlatformType, rResPrefix, rFileName);
        }

        public string GetPersistentUrl_CurPlatform(string rResPrefix, string rFileName)
        {
            return this.GetPersistentUrl(this.CurRuntimePlatform, rResPrefix, rFileName);
        }

        public UrlInfo GetServerUrl(string rFileName)
        {
            if (this.ServerURLInfo == null)
            {
                return null;
            }
            else
            {
                var rNewUrlInfo = new UrlInfo()
                {
                    MainURL = this.ServerURLInfo.MainURL + "/" + rFileName,
                    SubURL = this.ServerURLInfo.SubURL + "/" + rFileName,
                };
                return rNewUrlInfo;
            }
        }

        public static ABPlatformType RuntimePlafromToABPlatform(RuntimePlatform rRuntimePlatform)
        {
            switch (rRuntimePlatform)
            {
                case RuntimePlatform.OSXEditor:
                    return ABPlatformType.OSXEditor;
                case RuntimePlatform.OSXPlayer:
                    return ABPlatformType.OSXPlayer;
                case RuntimePlatform.WindowsPlayer:
                    return ABPlatformType.WindowsPlayer;
                case RuntimePlatform.WindowsEditor:
                    return ABPlatformType.WindowsEditor;
                case RuntimePlatform.IPhonePlayer:
                    return ABPlatformType.IphonePlayer;
                case RuntimePlatform.Android:
                    return ABPlatformType.Android;
                default:
                    return ABPlatformType.Unkown;
            }
        }

        public static RuntimePlatform ABPlatformToRuntimePlafrom(ABPlatformType rABPlatformType)
        {
            switch (rABPlatformType)
            {
                case ABPlatformType.OSXEditor:
                    return RuntimePlatform.OSXEditor;
                case ABPlatformType.OSXPlayer:
                    return RuntimePlatform.OSXPlayer;
                case ABPlatformType.WindowsPlayer:
                    return RuntimePlatform.WindowsPlayer;
                case ABPlatformType.WindowsEditor:
                    return RuntimePlatform.WindowsEditor;
                case ABPlatformType.IphonePlayer:
                    return RuntimePlatform.IPhonePlayer;
                case ABPlatformType.Android:
                    return RuntimePlatform.Android;
                default:
                    return RuntimePlatform.WindowsEditor;
            }
        }
    }
}