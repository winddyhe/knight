using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Knight.Core;
using System.IO;
using System;
using Newtonsoft.Json;
using Knight.Core.Editor;
using System.Runtime.ExceptionServices;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABBuilder : TSingleton<ABBuilder>
    {
        private uint mABBuildKey = 982310;

        private ABBuilder()
        {
        }

        public void Build(string rOutputPath, ABPackageType rABPackageType, BuildAssetBundleOptions rBuildOptions, BuildTarget rBuildTarget)
        {
            // 初始化类型解析器
            TypeResolveManagerEditor.ScriptsReloaded();
            // 初始化配置
            ABBuildEntryConfigManager.Instance.Initialize();
            // 预处理资源
            this.PreprocessBuild(ABBuildEntryConfigManager.Instance.ABBuildEntries);

            // 创建目录
            if (!Directory.Exists(rOutputPath))
            {
                Directory.CreateDirectory(rOutputPath);
            }

            // 打包资源
            var rAssetBundleBuildCaches = this.GetAllAssetbundleBuilds(ABBuildEntryConfigManager.Instance.ABBuildEntries);
            var rAssetBundleBuilds = this.ABBuildCacheToABBuild(rAssetBundleBuildCaches);
            var rBuildMap = BuildPipeline.BuildAssetBundles(rOutputPath, rAssetBundleBuilds.ToArray(), rBuildOptions, rBuildTarget);
            if (rBuildMap == null)
            {
                Debug.LogError("ABBuilder.Build Error!");
                return;
            }
            // 后处理非Assetbundle的资源
            var rNoneAssetbundleBuildCaches = this.GetAllNoneAssetbundleBuilds(ABBuildEntryConfigManager.Instance.ABBuildEntries);
            var rNoneAssetbundleBuilds = this.ABBuildCacheToABBuild(rNoneAssetbundleBuildCaches);
            this.PostprocessNoneAssetbundle(ABBuildEntryConfigManager.Instance.ABBuildEntries);

            // 计算资源的MD5
            var rABBuildMD5Dict = this.CalcAssetbundleBuildsMD5(rAssetBundleBuilds, rNoneAssetbundleBuilds, rBuildMap);

            // 根据AssetBundleManifest生成ABVersion并保存
            var rABVersionBytesPath = Path.Combine(rOutputPath, "ABVersion.bytes");
            var rABVersionMD5Path = Path.Combine(rOutputPath, "ABVersionMD5.bytes");
            var rABVersionJsonPath = Path.Combine(rOutputPath, "ABVersion.json");
            var rOldABVersion = ABVersionEditor.LoadABVersionByJson(rABVersionJsonPath);
            var rABVersion = ABVersionEditor.ABManifestToABVersion(rOutputPath, rOldABVersion, rAssetBundleBuildCaches, rNoneAssetbundleBuildCaches, rBuildMap, rABBuildMD5Dict);

            // 根据MD5对现有的Assetbundle进行加密
            var rEncrptABVersion = this.EncryptAssetbundles(rOutputPath, rABVersion);

            // 保存ABVersion
            ABVersionEditor.SaveABVersionToJson(rABVersionJsonPath, rABVersion);
            // 保存加密的ABVersion
            ABVersionEditor.SaveABVersion(rABVersionBytesPath, rEncrptABVersion);
            ABVersionEditor.SaveABVersionMD5(rABVersionMD5Path, rABVersionBytesPath);
            ABVersionEditor.SaveABVersionToJson(rABVersionJsonPath + ".encrypt", rEncrptABVersion);
            // 保存MD5映射关系表
            var rABBuildMD5DictJsonPath = Path.Combine(rOutputPath, "ABBuildMD5Dict.json");
            var rABBuildMD5DictJson = JsonConvert.SerializeObject(rABBuildMD5Dict, Formatting.Indented);
            PathTool.WriteFile(rABBuildMD5DictJsonPath, rABBuildMD5DictJson);

            Debug.Log("ABBuilder.Build Success!");

            if (rABPackageType == ABPackageType.HotfixFullPackage)
            {
                // 将整包资源拷贝到history/base中
                this.SyncAssetbundleToHistoryBase(rOutputPath);
            }
            else if (rABPackageType == ABPackageType.HotfixPackage)
            {
                // 将热更新的增量资源拷贝到History中，文件夹以时间到秒为名字
                this.SyncAssetbundleToHistoryIncrement(rOutputPath);
            }
        }

        private void SyncAssetbundleToHistoryBase(string rOutputPath)
        {
            var rBaseDirInfo = new DirectoryInfo(PathTool.Combine(rOutputPath, "history/base"));
            if (rBaseDirInfo.Exists)
            {
                rBaseDirInfo.Delete(true);
            }
            if (!rBaseDirInfo.Exists)
            {
                rBaseDirInfo.Create();
            }
            var rABVersionBytesPath = Path.Combine(rOutputPath, "ABVersion.bytes");
            var rABVersionMD5Path = Path.Combine(rOutputPath, "ABVersionMD5.bytes");
            var rABVersion = ABVersionEditor.LoadABVersion(rABVersionBytesPath);
            foreach (var rABVersionItem in rABVersion.Entries)
            {
                var rABVersionItemPath = Path.Combine(rOutputPath, rABVersionItem.Value.ABPath);
                var rABVersionItemHistoryBasePath = Path.Combine(rOutputPath, "history/base", rABVersionItem.Value.ABPath);
                var rABVersionItemHistorBaseParentDir = PathTool.GetParentPath(rABVersionItemHistoryBasePath);
                if (!Directory.Exists(rABVersionItemHistorBaseParentDir))
                    Directory.CreateDirectory(rABVersionItemHistorBaseParentDir);

                var rABVersionItemFileInfo = new FileInfo(rABVersionItemPath);
                rABVersionItemFileInfo.CopyTo(rABVersionItemHistoryBasePath, true);
            }
            // 拷贝ABVersion
            var rABVersionHistoryPath = Path.Combine(rOutputPath, "history/base", "ABVersion.bytes");
            var rABVersionFileInfo = new FileInfo(rABVersionBytesPath);
            rABVersionFileInfo.CopyTo(rABVersionHistoryPath, true);
            // 拷贝ABVersionMD5
            var rABVersionHistoryMD5Path = Path.Combine(rOutputPath, "history/base", "ABVersionMD5.bytes");
            var rABVersionMD5FileInfo = new FileInfo(rABVersionMD5Path);
            rABVersionMD5FileInfo.CopyTo(rABVersionHistoryMD5Path, true);
            // 拷贝ABVersionJson
            var rABVersionJsonPath = Path.Combine(rOutputPath, "history/base", "ABVersion.json.encrypt");
            var rABVersionJsonFileInfo = new FileInfo(Path.Combine(rOutputPath, "ABVersion.json.encrypt"));
            rABVersionJsonFileInfo.CopyTo(rABVersionJsonPath, true);

            Debug.Log("ABBuilder.SyncAssetbundleToHistoryBase Success!");
        }

        private void SyncAssetbundleToHistoryIncrement(string rOutputPath)
        {
            // 将热更新的增量资源拷贝到History中，文件夹以时间到秒为名字
            var rHistoryDirInfo = new DirectoryInfo(PathTool.Combine(rOutputPath, "history/base"));
            if (!rHistoryDirInfo.Exists)
            {
                Debug.Log("Base directory not exists, cannot build hotfix packages!");
                return;
            }

            // 比较两个版本
            var rBaseABVersionBytesPath = Path.Combine(rOutputPath, "history/base", "ABVersion.bytes");
            var rBaseABVersion = ABVersionEditor.LoadABVersion(rBaseABVersionBytesPath);
            var rABVersionBytesPath = Path.Combine(rOutputPath, "ABVersion.bytes");
            var rABVersion = ABVersionEditor.LoadABVersion(rABVersionBytesPath);

            var rABVersionIncrement = new ABVersion();
            rABVersionIncrement.Entries = new Dictionary<string, ABEntry>();
            foreach (var rABVersionItem in rABVersion.Entries)
            {
                if (!rBaseABVersion.Entries.ContainsKey(rABVersionItem.Key))
                {
                    // 是新增
                    rABVersionIncrement.Entries.Add(rABVersionItem.Key, rABVersionItem.Value);
                }
                else
                {
                    // 是修改
                    var rBaseABVersionItem = rBaseABVersion.Entries[rABVersionItem.Key];
                    if (rBaseABVersionItem.MD5 != rABVersionItem.Value.MD5 || rBaseABVersionItem.Version != rABVersionItem.Value.Version)
                    {
                        rABVersionIncrement.Entries.Add(rABVersionItem.Key, rABVersionItem.Value);
                    }
                }
            }
            foreach (var rBaseABVersionItem in rBaseABVersion.Entries)
            {
                // 是删除
                if (!rABVersion.Entries.ContainsKey(rBaseABVersionItem.Key))
                {
                    var rCloneABVersionItem = new ABEntry();
                    rCloneABVersionItem.Clone(rBaseABVersionItem.Value);
                    rCloneABVersionItem.IsDeleteAB = true;
                    rCloneABVersionItem.Size = 0;
                    rCloneABVersionItem.Version++;
                    rABVersionIncrement.Entries.Add(rBaseABVersionItem.Key, rCloneABVersionItem);
                }
            }
            if (rABVersionIncrement.Entries.Count == 0)
            {
                Debug.Log("This build has no increment assetbundle packages.");
                return;
            }

            // 获取增量目录
            var rDateTimeNow = DateTime.Now;
            var rHistoryIncrementDirName = rDateTimeNow.ToString("yyyyMMddHHmmss");
            var rHistoryIncrementDirInfo = new DirectoryInfo(PathTool.Combine(rOutputPath, "history", rHistoryIncrementDirName));
            if (rHistoryIncrementDirInfo.Exists)
            {
                rHistoryIncrementDirInfo.Delete(true);
            }
            if (!rHistoryIncrementDirInfo.Exists)
            {
                rHistoryIncrementDirInfo.Create();
            }
            // 复制增量版本中的文件
            foreach (var rIncrABVersionItem in rABVersionIncrement.Entries)
            {
                if (rIncrABVersionItem.Value.IsDeleteAB) continue;

                var rABVersionItemPath = PathTool.Combine(rOutputPath, rIncrABVersionItem.Value.ABPath);
                var rABVersionItemHistoryIncrementPath = Path.Combine(rOutputPath, "history", rHistoryIncrementDirName, rIncrABVersionItem.Value.ABPath);
                var rABVersionItemHistorIncrementParentDir = PathTool.GetParentPath(rABVersionItemHistoryIncrementPath);
                if (!Directory.Exists(rABVersionItemHistorIncrementParentDir))
                    Directory.CreateDirectory(rABVersionItemHistorIncrementParentDir);

                var rABVersionItemFileInfo = new FileInfo(rABVersionItemPath);
                rABVersionItemFileInfo.CopyTo(rABVersionItemHistoryIncrementPath, true);
            }
            // 保存增量版本文件
            var rIncrementABVersionPath = PathTool.Combine(rOutputPath, "history", rHistoryIncrementDirName, "ABVersion.bytes");
            var rIncrementABVersionJsonPath = PathTool.Combine(rOutputPath, "history", rHistoryIncrementDirName, "ABVersion.json");
            var rIncrementABVersionMD5Path = PathTool.Combine(rOutputPath, "history", rHistoryIncrementDirName, "ABVersionMD5.bytes");
            ABVersionEditor.SaveABVersion(rIncrementABVersionPath, rABVersionIncrement);
            ABVersionEditor.SaveABVersionToJson(rIncrementABVersionJsonPath, rABVersionIncrement);
            ABVersionEditor.SaveABVersionMD5(rIncrementABVersionMD5Path, rABVersionBytesPath);

            Debug.Log("ABBuilder.SyncAssetbundleToHistoryIncrement Success!");
        }

        /// <summary>
        /// 同步Assetbundle到StreamingAssets
        /// </summary>
        public void SyncAssetbundleToStreamingAssets(string rOutputPath)
        {
            // 获取StreamingAssetbundle目录
            var rStreamingAssetbundleDirInfo = new DirectoryInfo(ABPlatform.Instance.GetRealStreamingFile_CurPlatform(""));
            // 先全部删除
            if (rStreamingAssetbundleDirInfo.Exists)
            {
                rStreamingAssetbundleDirInfo.Delete(true);
            }
            // 再拷贝
            if (!rStreamingAssetbundleDirInfo.Exists)
            {
                rStreamingAssetbundleDirInfo.Create();
            }
            var rABVersionBytesPath = Path.Combine(rOutputPath, "ABVersion.bytes");
            var rABVersionMD5Path = Path.Combine(rOutputPath, "ABVersionMD5.bytes");
            var rABVersion = ABVersionEditor.LoadABVersion(rABVersionBytesPath);
            foreach (var rABVersionItem in rABVersion.Entries)
            {
                var rABVersionItemPath = Path.Combine(rOutputPath, rABVersionItem.Value.ABPath);
                var rABVersionItemStreamingPath = ABPlatform.Instance.GetRealStreamingFile_CurPlatform(rABVersionItem.Value.ABPath);
                var rABVersionItemStreamingParentDir = PathTool.GetParentPath(rABVersionItemStreamingPath);
                if (!Directory.Exists(rABVersionItemStreamingParentDir))
                    Directory.CreateDirectory(rABVersionItemStreamingParentDir);

                var rABVersionItemFileInfo = new FileInfo(rABVersionItemPath);
                rABVersionItemFileInfo.CopyTo(rABVersionItemStreamingPath, true);
            }
            // 拷贝ABVersion
            var rABVersionStreamingPath = ABPlatform.Instance.GetRealStreamingFile_CurPlatform("ABVersion.bytes");
            var rABVersionFileInfo = new FileInfo(rABVersionBytesPath);
            rABVersionFileInfo.CopyTo(rABVersionStreamingPath, true);
            // 拷贝ABVersionMD5
            var rABVersionMD5StreamingPath = ABPlatform.Instance.GetRealStreamingFile_CurPlatform("ABVersionMD5.bytes");
            var rABVersionMD5FileInfo = new FileInfo(rABVersionMD5Path);
            rABVersionMD5FileInfo.CopyTo(rABVersionMD5StreamingPath, true);
            // 刷新
            AssetDatabase.Refresh();
            Debug.Log("ABBuilder.SyncAssetbundleToStreamingAssets Success!");
        }

        private ABVersion EncryptAssetbundles(string rOutputPath, ABVersion rABVersion)
        {
            foreach (var rABVersionItem in rABVersion.Entries)
            {
                if (rABVersionItem.Value.IsAssetBundle)
                {
                    var rABPath = PathTool.Combine(rOutputPath, rABVersionItem.Value.ABPath);
                    var rABBytes = File.ReadAllBytes(rABPath);
                    var rABMD5 = rABVersionItem.Value.MD5;

                    var nOffsetLength = this.mABBuildKey % 255 ^ rABMD5[4] + 10;
                    var rNewABBytes = new byte[rABBytes.Length + nOffsetLength];
                    var rRandGenerator = new RandGenerator(this.mABBuildKey + rABMD5[5]);
                    for (int nIndex = 0; nIndex < nOffsetLength; ++nIndex)
                    {
                        rNewABBytes[nIndex] = (byte)(rABBytes[nIndex] ^ rRandGenerator.Range(0, 255));
                    }
                    Array.Copy(rABBytes, 0, rNewABBytes, nOffsetLength, rABBytes.Length);
                    var rEncrptFilePath = rABPath + ".encrypt";
                    File.WriteAllBytes(rEncrptFilePath, rNewABBytes);
                }
                else
                {
                    var rABPath = PathTool.Combine(rOutputPath, rABVersionItem.Value.ABPath);
                    var rABPathFileInfo = new FileInfo(rABPath);
                    var rEncryptABPathFileInfo = new FileInfo(PathTool.Combine(rOutputPath, rABVersionItem.Value.ABPath + ".encrypt"));
                    rABPathFileInfo.CopyTo(rEncryptABPathFileInfo.FullName, true);
                }
            }
            // 加密AB包的路径
            var rEncrptABVersion = ABVersionEditor.ABVersionEncryptABName(rABVersion, out var rEncryptPathDict);
            foreach (var rEncryptPath in rEncryptPathDict)
            {
                var rABPathFileInfo = new FileInfo(Path.Combine(rOutputPath, rEncryptPath.Key + ".encrypt"));
                var rEncryptABPathFileInfo = new FileInfo(PathTool.Combine(rOutputPath, rEncryptPath.Value));
                rABPathFileInfo.CopyTo(rEncryptABPathFileInfo.FullName, true);
            }
            return rEncrptABVersion;
        }

        private void PreprocessBuild(List<ABBuildEntry> rABBuildEntries)
        {
            foreach (var rABBuildEntry in rABBuildEntries)
            {
                ABBuildEntryTool.PreprocessBuild(rABBuildEntry);
            }
        }

        private List<AssetbundleBuildCache> GetAllAssetbundleBuilds(List<ABBuildEntry> rABBuildEntries)
        {
            var rAssetBundleBuilds = new List<AssetbundleBuildCache>();
            foreach (var rABBuildEntry in rABBuildEntries)
            {
                if (!rABBuildEntry.IsAssetBundle) continue;
                var rABBuilds = ABBuildEntryTool.GetAssetbundleBuilds(rABBuildEntry);
                // 重构AssetBundleBuild List
                rABBuilds = ABBuildEntryTool.ReconstructorABBuilds(rABBuildEntry, rABBuilds);
                for (int i = 0; i < rABBuilds.Count; i++)
                {
                    rAssetBundleBuilds.Add(new AssetbundleBuildCache() 
                    { 
                        ABBuild = rABBuilds[i], 
                        IsAssetBundle = rABBuildEntry.IsAssetBundle, 
                        IsNeedAssetList = rABBuildEntry.IsNeedAssetList
                    });
                }
            }
            return rAssetBundleBuilds;
        }

        private List<AssetBundleBuild> ABBuildCacheToABBuild(List<AssetbundleBuildCache> rAssetbundleBuildCaches)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            foreach (var rAssetbundleBuildCache in rAssetbundleBuildCaches)
            {
                rAssetBundleBuilds.Add(rAssetbundleBuildCache.ABBuild);
            }
            return rAssetBundleBuilds;
        }

        private List<AssetbundleBuildCache> GetAllNoneAssetbundleBuilds(List<ABBuildEntry> rABBuildEntries)
        {
            var rAssetBundleBuilds = new List<AssetbundleBuildCache>();
            foreach (var rABBuildEntry in rABBuildEntries)
            {
                if (rABBuildEntry.IsAssetBundle) continue;
                var rABBuilds = ABBuildEntryTool.GetAssetbundleBuilds(rABBuildEntry);
                // 重构AssetBundleBuild List
                rABBuilds = ABBuildEntryTool.ReconstructorABBuilds(rABBuildEntry, rABBuilds);
                for (int i = 0; i < rABBuilds.Count; i++)
                {
                    rAssetBundleBuilds.Add(new AssetbundleBuildCache()
                    {
                        ABBuild = rABBuilds[i],
                        IsAssetBundle = rABBuildEntry.IsAssetBundle,
                        IsNeedAssetList = rABBuildEntry.IsNeedAssetList
                    });
                }
            }
            return rAssetBundleBuilds;
        }

        private void PostprocessNoneAssetbundle(List<ABBuildEntry> rABBuildEntries)
        {
            foreach (var rABBuildEntry in rABBuildEntries)
            {
                ABBuildEntryTool.PostprocessBuildNoneAssetbundle(rABBuildEntry);
            }
        }

        /// <summary>
        /// 计算一个AB包的MD5码，遍历这个资源包里面的包含的所有资源（包括依赖资源）
        /// 然后这些资源在其他资源包未作为主资源在资源包中
        /// </summary>
        private Dictionary<string, string> CalcAssetbundleBuildsMD5(List<AssetBundleBuild> rAssetbundleBuilds, List<AssetBundleBuild> rNoneAssetbundleBuilds, AssetBundleManifest rBuildMap)
        {
            var rAssetbundleBuildsMD5Dict = new Dictionary<string, string>();
            foreach (var rAssetbundleBuild in rAssetbundleBuilds)
            {
                var rAllDependenciesABNames = rBuildMap.GetAllDependencies(rAssetbundleBuild.assetBundleName + "." + rAssetbundleBuild.assetBundleVariant);
                var rAllDependenciesABAssetList = new List<string>();
                for (int i = 0; i < rAllDependenciesABNames.Length; i++)
                {
                    var rDependenciesABName = rAllDependenciesABNames[i];
                    var rAllDependenciesAB = rAssetbundleBuilds.Find((rABBuild) => { return (rABBuild.assetBundleName + "." + rABBuild.assetBundleVariant).Equals(rDependenciesABName); });
                    if (!string.IsNullOrEmpty(rAllDependenciesAB.assetBundleName))
                    {
                        rAllDependenciesABAssetList.AddRange(rAllDependenciesAB.assetNames);
                    }
                }
                if (rAssetbundleBuild.assetBundleName.Equals("game/hotfix/libs"))
                {
                    var rGUIDs = AssetDatabase.FindAssets("t:script", new string[] { "Assets/Game.Hotfix", "Assets/Game.Config" });
                    for (int i = 0; i < rGUIDs.Length; i++)
                    {
                        var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                        rAllDependenciesABAssetList.Add(rAssetPath);
                    }
                }
                var rAssetbundleBuildMD5 = this.CalcAssetbundleBuildMD5(rAllDependenciesABAssetList, rAssetbundleBuild);
                rAssetbundleBuildsMD5Dict.Add(rAssetbundleBuild.assetBundleName + "." + rAssetbundleBuild.assetBundleVariant, rAssetbundleBuildMD5);
            }            
            foreach (var rAssetbundleBuild in rNoneAssetbundleBuilds)
            {
                var rAssetPaths = new List<string>();
                for (int i = 0; i < rAssetbundleBuild.assetNames.Length; i++)
                {
                    rAssetPaths.Add(rAssetbundleBuild.assetNames[i]);
                    rAssetPaths.Add(rAssetbundleBuild.assetNames[i] + ".meta");
                }
                var rAssetbundleName = string.IsNullOrEmpty(rAssetbundleBuild.assetBundleVariant) ? rAssetbundleBuild.assetBundleName : rAssetbundleBuild.assetBundleName + "." + rAssetbundleBuild.assetBundleVariant;
                var rAssetbundleBuildMD5 = MD5Tool.GetFilesMD5(rAssetPaths, rAssetbundleName).ToHEXString();
                rAssetbundleBuildsMD5Dict.Add(rAssetbundleName, rAssetbundleBuildMD5);
            }
            return rAssetbundleBuildsMD5Dict;
        }

        private string CalcAssetbundleBuildMD5(List<string> rAllDependenciesABAssetList, AssetBundleBuild rAssetbundleBuild)
        {
            var rAllAssetPaths = new HashSet<string>();
            foreach (var rAssetPath in rAssetbundleBuild.assetNames)
            {
                if (rAllDependenciesABAssetList.Contains(rAssetPath)) continue;

                rAllAssetPaths.Add(rAssetPath);
                rAllAssetPaths.Add(rAssetPath + ".meta");
                var rDependenciesAssetPaths = AssetDatabase.GetDependencies(rAssetPath, true);
                foreach (var rDependenciesAssetPath in rDependenciesAssetPaths)
                {
                    rAllAssetPaths.Add(rDependenciesAssetPath);
                    rAllAssetPaths.Add(rDependenciesAssetPath + ".meta");
                }
            }
            // Hashset to list
            var rAllAssetPathsList = new List<string>();
            foreach (var rAssetPath in rAllAssetPaths)
            {
                rAllAssetPathsList.Add(rAssetPath);
            }
            return MD5Tool.GetFilesMD5(rAllAssetPathsList, rAssetbundleBuild.assetBundleName + "." + rAssetbundleBuild.assetBundleVariant).ToHEXString();
        }
    }
}