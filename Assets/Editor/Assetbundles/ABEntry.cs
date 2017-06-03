//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using Object = UnityEngine.Object;
using Core;

namespace UnityEditor.AssetBundles
{
    /// <summary>
    /// 一个资源包项
    /// </summary>
    [System.Serializable]
    public class ABEntry
    {
        /// <summary>
        /// 资源包的类型
        /// </summary>
        public enum AssetSourceType
        {
            Dir_Dir,            //目录下的每一个目录是一个包
            Dir_File,           //目录下的每一个文件是一个包
            Dir,                //一个目录一个包
            File,               //一个文件一个包
        };

        /// <summary>
        /// 资源包名
        /// </summary>
        public string           abName;
        /// <summary>
        /// 资源包的后缀名
        /// </summary>
        public string           abVariant;
        /// <summary>
        /// 资源包的原始路径
        /// </summary>
        public string           assetResPath;
        /// <summary>
        /// 需要的资源类型，比如如果是预制件那么应该为 "t:Prefab"
        /// </summary>
        public string           assetType;
        /// <summary>
        /// 需要过滤的资源
        /// </summary>
        public List<string>     filerAssets;
        /// <summary>
        /// 原始资源的类型
        /// </summary>
        public AssetSourceType  assetSrcType;
        /// <summary>
        /// 资源的类名
        /// </summary>
        public string           abClassName;
        /// <summary>
        /// 最原始的资源路径
        /// </summary>
        public string           abOriginalResPath;

        /// <summary>
        /// 资源包名，包含后缀
        /// </summary>
        public string           ABFullName { get { return abName + "." + abVariant; } }

        public AssetBundleBuild[] ToABBuild()
        {
            switch (assetSrcType)
            {
                case AssetSourceType.Dir_Dir:
                    return GetOneDir_Dirs();
                case AssetSourceType.Dir_File:
                    return GetOneDir_Files();
                case AssetSourceType.Dir:
                    return GetOneDir();
                case AssetSourceType.File:
                    return GetOneFile();
            }
            return null;
        }

        /// <summary>
        /// 得到一个文件的ABB
        /// </summary>
        private AssetBundleBuild[] GetOneFile()
        {
            Object rObj = AssetDatabase.LoadAssetAtPath(assetResPath, typeof(Object));
            if (rObj == null) return null;

            AssetBundleBuild rABB = new AssetBundleBuild();
            rABB.assetBundleName = abName;
            rABB.assetBundleVariant = abVariant;
            rABB.assetNames = new string[] { assetResPath };
            return new AssetBundleBuild[] { rABB };
        }

        /// <summary>
        /// 得到一个目录的ABB
        /// </summary>
        private AssetBundleBuild[] GetOneDir()
        {
            DirectoryInfo rDirInfo = new DirectoryInfo(assetResPath);
            if (!rDirInfo.Exists) return null;

            AssetBundleBuild rABB = new AssetBundleBuild();
            rABB.assetBundleName = abName;
            rABB.assetBundleVariant = abVariant;
            rABB.assetNames = new string[] { assetResPath };
            return new AssetBundleBuild[] { rABB };
        }

        /// <summary>
        /// 得到一个目录下的所有的文件对应的ABB
        /// </summary>
        private AssetBundleBuild[] GetOneDir_Files()
        {
            DirectoryInfo rDirInfo = new DirectoryInfo(assetResPath);
            if (!rDirInfo.Exists) return null;

            List<AssetBundleBuild> rABBList = new List<AssetBundleBuild>();
            string[] rGUIDS = AssetDatabase.FindAssets(this.assetType, new string[] { assetResPath });
            for (int i = 0; i < rGUIDS.Length; i++)
            {
                string rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDS[i]);

                AssetBundleBuild rABB = new AssetBundleBuild();
                rABB.assetBundleName = abName + "/" + Path.GetFileNameWithoutExtension(rAssetPath);
                rABB.assetBundleVariant = abVariant;
                rABB.assetNames = new string[] { rAssetPath };
                rABBList.Add(rABB);
            }
            return rABBList.ToArray();
        }

        /// <summary>
        /// 得到一个目录下的所有的目录对应的ABB
        /// </summary>
        private AssetBundleBuild[] GetOneDir_Dirs()
        {
            DirectoryInfo rDirInfo = new DirectoryInfo(assetResPath);
            if (!rDirInfo.Exists) return null;

            List<AssetBundleBuild> rABBList = new List<AssetBundleBuild>();
            DirectoryInfo[] rSubDirs = rDirInfo.GetDirectories();
            for (int i = 0; i < rSubDirs.Length; i++)
            {
                string rDirPath = rSubDirs[i].FullName;
                string rRootPath = System.Environment.CurrentDirectory + "\\";
                rDirPath = rDirPath.Replace(rRootPath, "").Replace("\\", "/");

                string rFileName = Path.GetFileNameWithoutExtension(rDirPath);
                if (filerAssets != null && filerAssets.FindIndex((item) => { return rFileName.Contains(item); }) >= 0) continue;

                AssetBundleBuild rABB = new AssetBundleBuild();
                rABB.assetBundleName = abName + "/" + rFileName;
                rABB.assetBundleVariant = abVariant;
                rABB.assetNames = new string[] { rDirPath };
                rABBList.Add(rABB);
            }
            return rABBList.ToArray();
        }
    }

    /// <summary>
    /// ABEntry的预处理器
    /// </summary>
    public class ABEntryProcessor
    {
        public ABEntry Entry;
        
        /// <summary>
        /// 预处理所有的资源
        /// </summary>
        public virtual void PreprocessAssets()
        {
        }

        public void ProcessAssetBundleLabel()
        {
            if (this.Entry == null) return;

            var rAssetbundleBuilds = this.Entry.ToABBuild();
            for (int i = 0; i < rAssetbundleBuilds.Length; i++)
            {
                var rABBuild = rAssetbundleBuilds[i];
                for (int j = 0; j < rABBuild.assetNames.Length; j++)
                {
                    string rAssetPath = rABBuild.assetNames[j];
                    AssetImporter rAssetImporter = AssetImporter.GetAtPath(rAssetPath);
                    if (rAssetImporter == null) return;
                    
                    rAssetImporter.SetAssetBundleNameAndVariant(rABBuild.assetBundleName, rABBuild.assetBundleVariant);
                    AssetDatabase.WriteImportSettingsIfDirty(rAssetPath);
                }
            }
        }
        
        /// <summary>
        /// 将Entry转成能用于打包的ABB
        /// </summary>
        public AssetBundleBuild[] ToABBuild()
        {
            if (this.Entry == null) return new AssetBundleBuild[0];
            return this.Entry.ToABBuild();
        }

        /// <summary>
        /// 根据不同的类名构建不同的资源预处理器
        /// </summary>
        public static ABEntryProcessor Create(ABEntry rABEntry)
        {
            ABEntryProcessor rEntryProcessor = null;

            Type rType = Type.GetType(rABEntry.abClassName);
            if (rType == null) 
                rEntryProcessor = new ABEntryProcessor();
            else
                rEntryProcessor = ReflectionAssist.Construct(rType) as ABEntryProcessor;

            rEntryProcessor.Entry = rABEntry;

            return rEntryProcessor;
        }
    }
}