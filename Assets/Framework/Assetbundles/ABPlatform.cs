//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using System.IO;

namespace UnityEngine.AssetBundles
{
    /// <summary>
    /// 资源管理类，主要用作不同资源平台的路径管理
    /// </summary>
    public class ABPlatform : TSingleton<ABPlatform>
    {
        /// <summary>
        /// 暂时选用如下平台，如需新平台再添加
        /// </summary>
        public enum Platform
        {
            OSXEditor = 0,
            OSXPlayer,
            WindowsPlayer,
            WindowsEditor,
            IphonePlayer,
            Android,
            Unkown,
        }
    
        /// <summary>
        /// 当前的平台
        /// </summary>
        public Platform runtimePlatform;
    
        /// <summary>
        /// 平台名
        /// </summary>
        public static string[] PlatformNames = 
        {
            "OSX",
            "OSX",
            "Windows",
            "Windows",
            "IOS",
            "Android"
        };
    
        /// <summary>
        /// 运行平台是否为Editor
        /// </summary>
        public static bool[] PlatformIsEditor = 
        {
            true,
            false,
            false,
            true,
            false,
            false
        };
    
        public static string[] PlatformPrefixs = 
        {
            "file:///",         //OSXEditor
            "file:///",         //OSXPlayer
            "file:///",         //WindowsPlayer
            "file:///",         //WindowsEditor
            "file://",          //IphonePlayer
            "",                 //Android
        };
    
        /// <summary>
        /// 得到StreamingAssets下的资源目录路径
        /// </summary>
        public string GetStreamingFile(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
    
            bool isEditor = PlatformIsEditor[rPlatformIndex];
            string rRootDir = isEditor ? Application.dataPath : Application.streamingAssetsPath;
    
            return rRootDir + "/Assetbundles/" + PlatformNames[rPlatformIndex] + "_Assetbundles/";
        }
    
        /// <summary>
        /// 得到StreamingAssets下的资源目录的URL
        /// </summary>
        public string GetStreamingUrl(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
            return PlatformPrefixs[rPlatformIndex] + GetStreamingFile(rPlatform);
        }
    
        /// <summary>
        /// 得到当前平台的资源的URL
        /// </summary>
        public string GetStreamingUrl_CurPlatform(string rABName)
        {
            string rPath = GetStreamingUrl(this.runtimePlatform) + rABName;
            Debug.LogFormat("---- {0}", rPath);
            return rPath;
        }
    
        /// <summary>
        /// 得到AssetbundleManifest的Url路径
        /// </summary>
        public string GetAssetbundleManifestUrl()
        {
            string rRootPath = GetStreamingUrl(this.runtimePlatform);
            DirectoryInfo rDirInfo = new DirectoryInfo(rRootPath);
            return rRootPath + rDirInfo.Name;
        }
    
        private ABPlatform()
        {
        }
    
        /// <summary>
        /// 管理器的初始化
        /// </summary>
        public void Initialize()
        {
            runtimePlatform = RuntimePlatform_To_Plaform(Application.platform);
        }
    
        /// <summary>
        /// 平台的对应关系转换
        /// </summary>
        private Platform RuntimePlatform_To_Plaform(RuntimePlatform rRuntimePlatform)
        {
            switch (rRuntimePlatform)
            {
                case RuntimePlatform.Android:       return Platform.Android;
                case RuntimePlatform.IPhonePlayer:  return Platform.IphonePlayer;
                case RuntimePlatform.OSXEditor:     return Platform.OSXEditor;
                case RuntimePlatform.OSXPlayer:     return Platform.OSXPlayer;
                case RuntimePlatform.WindowsEditor: return Platform.WindowsEditor;
                case RuntimePlatform.WindowsPlayer: return Platform.WindowsPlayer;
                default:                            return Platform.Unkown;
            }
        }
    }
}