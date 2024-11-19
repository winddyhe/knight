using Knight.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.Assetbundle
{
    public enum ABLoadSpace
    {
        Streaming,
        Presistent,
        Server,
    }

    /// <summary>
    /// AssetBundle Entry
    /// 缓存 AssetBundle 对象, 采用引计数管理, 当引用计数为0时，使用AssetBundle.Unload(true)进行卸载。
    /// </summary>
    public class ABLoadEntry
    {
        public string ABPath;
        public string ABName;
        public string MD5;
        public ABLoadSpace LoadSpace;
        public List<string> Dependencies;
        public bool IsAssetBundle;
        public List<string> AssetList;

        public AssetBundle CacheAsset;

        public bool IsLoadCompleted;
        public bool IsLoading;
        public bool IsLock;

        public int RefCount;
    }

    public class ABLoadVersion : TSingleton<ABLoadVersion>
    {
        private Dictionary<string, string> mABMD5Dict;
        private Dictionary<string, ABLoadEntry> mEntryDict;

        private ABLoadVersion()
        {
        }

        public void Initialize()
        {
            if (this.mABMD5Dict == null)
                this.mABMD5Dict = new Dictionary<string, string>();
            this.mABMD5Dict.Clear();
            if (this.mEntryDict == null)
                this.mEntryDict = new Dictionary<string, ABLoadEntry>();
            this.mEntryDict.Clear();
        }

        public bool TryGetValue(string rABPath, out ABLoadEntry rABLoadEntry)
        {
            if (this.mEntryDict == null || this.mABMD5Dict == null)
            {
                rABLoadEntry = null;
                return false;
            }
            if (this.mABMD5Dict.TryGetValue(rABPath, out var rABPathMD5))
            {
                return this.mEntryDict.TryGetValue(rABPathMD5, out rABLoadEntry);
            }
            else
            {
                rABPathMD5 = MD5Tool.GetStringMD5(rABPath).ToHEXLowerString();
                this.mABMD5Dict.Add(rABPath, rABPathMD5);
                return this.mEntryDict.TryGetValue(rABPathMD5, out rABLoadEntry);
            }
        }

        public bool TryGetValueMD5Path(string rABPathMD5, out ABLoadEntry rABLoadEntry)
        {
            return this.mEntryDict.TryGetValue(rABPathMD5, out rABLoadEntry);
        }

        public string GetABPathWithSpace(ABLoadSpace rLoadSpace, string rABPath)
        {
            if (this.mEntryDict == null || this.mABMD5Dict == null)
            {
                return string.Empty;
            }
            if (!this.mABMD5Dict.TryGetValue(rABPath, out var rABPathMD5))
            {
                rABPathMD5 = MD5Tool.GetStringMD5(rABPath).ToHEXLowerString();
                this.mABMD5Dict.Add(rABPath, rABPathMD5);
            }
            if (this.mEntryDict.TryGetValue(rABPathMD5, out var rABLoadEntry))
            {
                if (rLoadSpace == ABLoadSpace.Streaming)
                {
                    rABPath = ABPlatform.Instance.GetStreamingFile_CurPlatform(rABPathMD5);
                }
                else if (rLoadSpace == ABLoadSpace.Presistent)
                {
                    rABPath = ABPlatform.Instance.GetPersistentFile_CurPlatform(ABPlatform.Instance.CdnVersionCode, rABPathMD5);
                }
                return rABPath;
            }
            return string.Empty;
        }

        public void AddEntry(ABEntry rABEntry, ABLoadSpace rLoadSpace)
        {
            var rABPath = string.Empty;
            if (rLoadSpace == ABLoadSpace.Streaming)
            {
                rABPath = ABPlatform.Instance.GetStreamingFile_CurPlatform(rABEntry.ABPath);
            }
            else if (rLoadSpace == ABLoadSpace.Presistent)
            {
                rABPath = ABPlatform.Instance.GetPersistentFile_CurPlatform(ABPlatform.Instance.CdnVersionCode, rABEntry.ABPath);
            }
            var rABLoadEntry = new ABLoadEntry()
            {
                ABName = rABEntry.ABPath,
                ABPath = rABPath,
                MD5 = rABEntry.MD5,
                LoadSpace = rLoadSpace,
                Dependencies = new List<string>(rABEntry.Dependencies),
                IsAssetBundle = rABEntry.IsAssetBundle,
                CacheAsset = null,
                IsLoadCompleted = false,
                IsLoading = false,
                RefCount = 0,
                IsLock = false,
            };
            if (rABEntry.AssetList != null)
            {
                rABLoadEntry.AssetList = new List<string>(rABEntry.AssetList);
            }
            this.mEntryDict.Add(rABEntry.ABPath, rABLoadEntry);
        }
    }
}