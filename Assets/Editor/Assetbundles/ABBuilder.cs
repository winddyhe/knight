//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Core;
using Core.Editor;

namespace UnityEditor.AssetBundles
{
    /// <summary>
    /// 资源打包的辅助类
    /// </summary>
    public class ABBuilder : TSingleton<ABBuilder>
    {
        public enum BuildPlatform
        {
            Windows     = BuildTarget.StandaloneWindows,        //Windows
            Windows64   = BuildTarget.StandaloneWindows64,      //Windows64
            OSX         = BuildTarget.StandaloneOSX,            //OSX
            IOS         = BuildTarget.iOS,                      //IOS
            Android     = BuildTarget.Android,                  //Android
        };
        
        /// <summary>
        /// 输出的Assetbundle的目录
        /// </summary>
        public static string AssetbundlePath        = "Assets/Assetbundles";

        /// <summary>
        /// 资源包配置文件路径
        /// </summary>
        public static string ABEntryConfigPath      = "Assets/Editor/Assetbundles/Assetbundle_Settings.asset";

        /// <summary>
        /// 当前工程的平台
        /// </summary>
        public BuildPlatform CurBuildPlatform       = BuildPlatform.Windows;
    
        /// <summary>
        /// 资源项的配置缓存
        /// </summary>
        public List<ABEntry> ABEntries;
    
        private ABBuilder()
        {
            CurBuildPlatform = GetCurrentBuildPlatform();
        }
    
        /// <summary>
        /// 得到Assetbundle的路径前缀，根据不同的平台来选择
        /// </summary>
        public string GetPathPrefix_Assetbundle()
        {
            return Path.Combine(AssetbundlePath, GetManifestName()).Replace("\\", "/");
        }
    
        /// <summary>
        /// 得到Manifest的名字
        /// </summary>
        public string GetManifestName()
        {
            return GetCurrentBuildPlatformName() + "_Assetbundles";
        }
    
        /// <summary>
        /// 打包资源
        /// </summary>
        public void BuildAssetbundles(BuildAssetBundleOptions rOptions)
        {
            List<AssetBundleBuild> rABBList = AssetbundleEntry_Building();
            
            string rABPath = GetPathPrefix_Assetbundle();
            DirectoryInfo rDirInfo = new DirectoryInfo(rABPath);
            if (!rDirInfo.Exists) rDirInfo.Create();

            var rOldABVersion = ABVersionEditor.Load(rABPath);
            var rOldMD5 = ABVersionEditor.GetMD5ForABVersion(rABPath);

            // 开始打包
            var rNewABManifest = BuildPipeline.BuildAssetBundles(rABPath, rABBList.ToArray(), rOptions, (BuildTarget)CurBuildPlatform);
            
            // 生成新的版本文件
            var rNewABVersion = ABVersionEditor.CreateVersion(rABPath, rOldABVersion, rNewABManifest);
            rNewABVersion.SaveInEditor(rABPath);

            var rNewMD5 = ABVersionEditor.GetMD5ForABVersion(rABPath);

            // 保存历史的版本记录
            if (!string.IsNullOrEmpty(rOldMD5) && !rOldMD5.Equals(rNewMD5))
            {
                rOldABVersion.SaveHistory(rABPath);
            }
            Debug.Log("资源打包完成！");
        }

        /// <summary>
        /// 生成AB包的配置项
        /// </summary>
        public List<ABEntry> GenerateABEntries()
        {
            var rABEntryConfig = EditorAssists.ReceiveAsset<ABEntryConfig>(ABEntryConfigPath);
            if (rABEntryConfig == null) return new List<ABEntry>();
            return rABEntryConfig.ABEntries;
        }
    
        /// <summary>
        /// 构建需要打包的资源的路径、包名以及包的后缀
        /// </summary>
        public List<AssetBundleBuild> AssetbundleEntry_Building()
        {
            this.ABEntries = this.GenerateABEntries();
            if (ABEntries == null) ABEntries = new List<ABEntry>();
    
            // 资源预处理
            List<ABEntryProcessor> rABEntryProcessors = new List<ABEntryProcessor>();
            foreach (var rEntry in ABEntries)
            {
                ABEntryProcessor rProcessor = ABEntryProcessor.Create(rEntry);
                rProcessor.PreprocessAssets();
                rProcessor.ProcessAssetBundleLabel();
                rABEntryProcessors.Add(rProcessor);
            }
            // 打包
            List<AssetBundleBuild> rABBList = new List<AssetBundleBuild>();
            foreach (var rProcessor in rABEntryProcessors)
            {
                rABBList.AddRange(rProcessor.ToABBuild());
            }
            return rABBList;
        }

        public void UpdateAllAssetsABLabels(string aBEntryConfigPath)
        {
            this.ABEntries = this.GenerateABEntries();
            if (ABEntries == null) ABEntries = new List<ABEntry>();

            // 资源预处理
            List<ABEntryProcessor> rABEntryProcessors = new List<ABEntryProcessor>();
            foreach (var rEntry in ABEntries)
            {
                ABEntryProcessor rProcessor = ABEntryProcessor.Create(rEntry);
                rProcessor.PreprocessAssets();
                rProcessor.ProcessAssetBundleLabel();
                rABEntryProcessors.Add(rProcessor);
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
        
        public static BuildPlatform GetCurrentBuildPlatform()
        {
            return (BuildPlatform)EditorUserBuildSettings.activeBuildTarget;
        }

        public static string GetCurrentBuildPlatformName()
        {
            string rPlatformName = GetCurrentBuildPlatform().ToString();
            if (rPlatformName.Equals("Windows64"))
            {
                rPlatformName = "Windows";
            }
            return rPlatformName;
        }
    }
}