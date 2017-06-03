//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using System.Collections.Generic;

namespace UnityEngine.AssetBundles
{
    public enum LoaderSpace
    {
        Streaming,
        Persistent,
    }
        /// <summary>
    /// 资源的加载信息
    /// </summary>
    public class ABLoadEntry
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        public string                       ABName;
        /// <summary>
        /// 资源的路径名
        /// </summary>
        public string                       ABPath;
        /// <summary>
        /// 该资源的依赖项
        /// </summary>
        public string[]                     ABDependNames;
        /// <summary>
        /// 是否处于加载中
        /// </summary>
        public bool                         IsLoading = false;
        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool                         IsLoadCompleted = false;
        /// <summary>
        /// 缓存的Cache
        /// </summary>
        public AssetBundle                  CacheAsset;
        /// <summary>
        /// 该资源包的引用计数               
        /// </summary>
        public int                          RefCount = 0;
    }
    
    /// <summary>
    /// 最终生成的版本信息文件，用于加载资源用
    /// </summary>
    public class ABLoaderVersion : TSingleton<ABLoaderVersion>
    {
        /// <summary>
        /// 加载的资源信息
        /// </summary>
        private Dict<string, ABLoadEntry>    Entries;

        private ABLoaderVersion()
        {
        }

        public void Initialize()
        {
            this.Entries = new Dict<string, ABLoadEntry>();
        }

        public void AddEntry(LoaderSpace rLoaderSpace, ABVersionEntry rAVEntry)
        {
            ABLoadEntry rABLoadInfo = new ABLoadEntry();
            rABLoadInfo.ABPath          = GetABPath_With_Space(rLoaderSpace, rAVEntry.Name);
            rABLoadInfo.ABName          = rAVEntry.Name;
            List<string> rDependABs     = new List<string>(rAVEntry.Dependencies);
            rABLoadInfo.ABDependNames   = rDependABs.ToArray();
            rABLoadInfo.IsLoading       = false;
            rABLoadInfo.IsLoadCompleted = false;
            rABLoadInfo.CacheAsset      = null;
            rABLoadInfo.RefCount        = 0;

            this.Entries.Add(rAVEntry.Name, rABLoadInfo);
        }

        public bool TryGetValue(string rABName, out ABLoadEntry rABLoadEntry)
        {
            return this.Entries.TryGetValue(rABName, out rABLoadEntry);
        }

        private string GetABPath_With_Space(LoaderSpace rLoaderSpace, string rABName)
        {
            switch (rLoaderSpace)
            {
                case LoaderSpace.Streaming:
                    return ABPlatform.Instance.GetStreamingFile_CurPlatform(rABName);
                case LoaderSpace.Persistent:
                    return ABPlatform.Instance.GetPersistentFile_CurPlatform(rABName);
            }
            return ABPlatform.Instance.GetStreamingFile_CurPlatform(rABName);
        }
    }
}
