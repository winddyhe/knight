using Knight.Core;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Knight.Framework.Assetbundle.Editor
{
    public class AssetbundleBuildCache
    {
        public AssetBundleBuild ABBuild;
        public bool IsAssetBundle;
        public bool IsNeedAssetList;
    }

    public class ABBuildEntryTool
    {
        public static List<AssetBundleBuild> GetAssetbundleBuilds(ABBuildEntry rABBuildEntry)
        {
            switch (rABBuildEntry.ABBuildType)
            {
                case ABBuildType.File:
                    return GetAssetbundleBuilds_File(rABBuildEntry);
                case ABBuildType.Dir:
                    return GetAssetbundleBuilds_Dir(rABBuildEntry);
                case ABBuildType.Dir_File:
                    return GetAssetbundleBuilds_Dir_File(rABBuildEntry);
                case ABBuildType.Dir_Dir:
                    return GetAssetbundleBuilds_Dir_Dir(rABBuildEntry);
                case ABBuildType.Dir_Dir_File:
                    return GetAssetbundleBuilds_Dir_Dir_File(rABBuildEntry);
                case ABBuildType.Dir_Dir_Dir:
                    return GetAssetbundleBuilds_Dir_Dir_Dir(rABBuildEntry);
            }
            return new List<AssetBundleBuild>();
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_File(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rAssetBundleBuild = new AssetBundleBuild();
            if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
            {
                rAssetBundleBuild.assetBundleName = rABBuildEntry.ABName;
                rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                rAssetBundleBuild.assetNames = new string[] { rABBuildEntry.ResPath };
                rAssetBundleBuilds.Add(rAssetBundleBuild);
            }
            else
            {
                var rAssetPath = rABBuildEntry.ResPath;
                rAssetBundleBuild.assetBundleName = rAssetPath;
                rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                rAssetBundleBuild.assetNames = new string[] { rABBuildEntry.ResPath };
                rAssetBundleBuilds.Add(rAssetBundleBuild);
            }
            return rAssetBundleBuilds;
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_Dir(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rAssetBundleBuild = new AssetBundleBuild();
            rAssetBundleBuild.assetBundleName = rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix;
            rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
            var rAssetNames = new List<string>();
            var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
            foreach (var rGUID in rGUIDs)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                {
                    // 过滤掉不符合正则表达式的资源
                    var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                    if (!rMatch.Success)
                    {
                        continue;
                    }
                }
                rAssetNames.Add(rAssetPath);
            }
            rAssetBundleBuild.assetNames = rAssetNames.ToArray();
            rAssetBundleBuilds.Add(rAssetBundleBuild);
            return rAssetBundleBuilds;
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_Dir_File(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
            foreach (var rGUID in rGUIDs)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                if (AssetDatabase.IsValidFolder(rAssetPath)) continue;

                if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                {
                    // 过滤掉不符合正则表达式的资源
                    var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                    if (!rMatch.Success)
                    {
                        continue;
                    }
                }
                var rAssetBundleBuild = new AssetBundleBuild();

                var rFileExtension = Path.GetExtension(rAssetPath);
                rAssetBundleBuild.assetBundleName = string.IsNullOrEmpty(rABBuildEntry.ABVariant)
                    ? $"{rABBuildEntry.ABName}/{Path.GetFileNameWithoutExtension(rAssetPath)}{rABBuildEntry.ABAliasSuffix}{rFileExtension}"
                    : $"{rABBuildEntry.ABName}/{Path.GetFileNameWithoutExtension(rAssetPath)}{rABBuildEntry.ABAliasSuffix}";
                rAssetBundleBuild.assetBundleName = rAssetBundleBuild.assetBundleName.ToLower();
                rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                rAssetBundleBuild.assetNames = new string[] { rAssetPath };
                rAssetBundleBuilds.Add(rAssetBundleBuild);
            }
            return rAssetBundleBuilds;
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_Dir_Dir(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rDirectoryInfo = new System.IO.DirectoryInfo(rABBuildEntry.ResPath);
            var rAllDirectoryInfos = rDirectoryInfo.GetDirectories();
            foreach (var rDirInfo in rAllDirectoryInfos)
            {
                var rDirAssetPath = rABBuildEntry.ResPath + "/" + rDirInfo.Name;
                var rAssetBundleBuild = new AssetBundleBuild();
                rAssetBundleBuild.assetBundleName = rABBuildEntry.ABName + "/" + rDirInfo.Name.ToLower() + rABBuildEntry.ABAliasSuffix;
                rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                var rAssetNames = new List<string>();
                var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rDirAssetPath });
                foreach (var rGUID in rGUIDs)
                {
                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                    if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                    {
                        // 过滤掉不符合正则表达式的资源
                        var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                        if (!rMatch.Success)
                        {
                            continue;
                        }
                    }
                    rAssetNames.Add(rAssetPath);
                }
                rAssetBundleBuild.assetNames = rAssetNames.ToArray();
                rAssetBundleBuilds.Add(rAssetBundleBuild);
            }
            return rAssetBundleBuilds;
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_Dir_Dir_File(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rDirectoryInfo = new System.IO.DirectoryInfo(rABBuildEntry.ResPath);
            var rAllDirectoryInfos = rDirectoryInfo.GetDirectories();
            foreach (var rDirInfo in rAllDirectoryInfos)
            {
                var rDirAssetPath = rABBuildEntry.ResPath + "/" + rDirInfo.Name;
                var rSecondDirectoryInfo = new System.IO.DirectoryInfo(rDirAssetPath);
                var rSecondAllDirectoryInfos = rSecondDirectoryInfo.GetDirectories();
                foreach (var rSecondDirInfo in rSecondAllDirectoryInfos)
                {
                    var rSecondDirAssetPath = rDirAssetPath + "/" + rSecondDirInfo.Name;
                    var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rSecondDirAssetPath });
                    foreach (var rGUID in rGUIDs)
                    {
                        var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                        if (AssetDatabase.IsValidFolder(rAssetPath)) continue;

                        if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                        {
                            // 过滤掉不符合正则表达式的资源
                            var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                            if (!rMatch.Success)
                            {
                                continue;
                            }
                        }
                        var rAssetBundleBuild = new AssetBundleBuild();
                        var rFileExtension = Path.GetExtension(rAssetPath);
                        rAssetBundleBuild.assetBundleName = string.IsNullOrEmpty(rABBuildEntry.ABVariant)
                            ? $"{rABBuildEntry.ABName}/{rDirInfo.Name}/{rSecondDirInfo.Name}/{Path.GetFileNameWithoutExtension(rAssetPath)}{rABBuildEntry.ABAliasSuffix}{rFileExtension}"
                            : $"{rABBuildEntry.ABName}/{rDirInfo.Name}/{rSecondDirInfo.Name}/{Path.GetFileNameWithoutExtension(rAssetPath)}{rABBuildEntry.ABAliasSuffix}";
                        rAssetBundleBuild.assetBundleName = rAssetBundleBuild.assetBundleName.ToLower();
                        rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                        rAssetBundleBuild.assetNames = new string[] { rAssetPath };
                        rAssetBundleBuilds.Add(rAssetBundleBuild);
                    }
                }
            }
            return rAssetBundleBuilds;
        }

        private static List<AssetBundleBuild> GetAssetbundleBuilds_Dir_Dir_Dir(ABBuildEntry rABBuildEntry)
        {
            var rAssetBundleBuilds = new List<AssetBundleBuild>();
            var rDirectoryInfo = new System.IO.DirectoryInfo(rABBuildEntry.ResPath);
            var rAllDirectoryInfos = rDirectoryInfo.GetDirectories();
            foreach (var rDirInfo in rAllDirectoryInfos)
            {
                var rDirAssetPath = rABBuildEntry.ResPath + "/" + rDirInfo.Name;
                var rSecondDirectoryInfo = new System.IO.DirectoryInfo(rDirAssetPath);
                var rSecondAllDirectoryInfos = rSecondDirectoryInfo.GetDirectories();
                foreach (var rSecondDirInfo in rSecondAllDirectoryInfos)
                {
                    var rSecondDirAssetPath = rDirAssetPath + "/" + rSecondDirInfo.Name;
                    var rAssetBundleBuild = new AssetBundleBuild();
                    rAssetBundleBuild.assetBundleName = rABBuildEntry.ABName + "/" + rDirInfo.Name.ToLower() + "/" + rSecondDirInfo.Name.ToLower() + rABBuildEntry.ABAliasSuffix;
                    rAssetBundleBuild.assetBundleVariant = rABBuildEntry.ABVariant;
                    var rAssetNames = new List<string>();
                    var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rSecondDirAssetPath });
                    foreach (var rGUID in rGUIDs)
                    {
                        var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                        if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                        {
                            // 过滤掉不符合正则表达式的资源
                            var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                            if (!rMatch.Success)
                            {
                                continue;
                            }
                        }
                        rAssetNames.Add(rAssetPath);
                    }
                    rAssetBundleBuild.assetNames = rAssetNames.ToArray();
                    rAssetBundleBuilds.Add(rAssetBundleBuild);
                }
            }
            return rAssetBundleBuilds;
        }

        public static void PreprocessBuild(ABBuildEntry rABBuildEntry)
        {
            if (string.IsNullOrEmpty(rABBuildEntry.ABBuildPreprocessor)) return;
            var rType = TypeResolveManager.Instance.GetType(rABBuildEntry.ABBuildPreprocessor);
            if (rType == null)
            {
                Debug.LogError("PreprocessBuild Error: " + rABBuildEntry.ABBuildPreprocessor);
                return;
            }
            var rPreprocessor = ReflectTool.Construct(rType) as IABBuildPreprocessor;
            if (rPreprocessor == null)
            {
                Debug.LogError("PreprocessBuild Error: " + rABBuildEntry.ABBuildPreprocessor);
                return;
            }
            rPreprocessor.OnPreprocessBuild(rABBuildEntry);
        }

        public static List<AssetBundleBuild> ReconstructorABBuilds(ABBuildEntry rABBuildEntry, List<AssetBundleBuild> rABBuilds)
        {
            if (string.IsNullOrEmpty(rABBuildEntry.ABBuildPreprocessor)) return rABBuilds;
            var rType = TypeResolveManager.Instance.GetType(rABBuildEntry.ABBuildPreprocessor);
            if (rType == null)
            {
                Debug.LogError("PreprocessBuild Error: " + rABBuildEntry.ABBuildPreprocessor);
                return rABBuilds;
            }
            var rPreprocessor = ReflectTool.Construct(rType) as IABBuildPreprocessor;
            if (rPreprocessor == null)
            {
                Debug.LogError("PreprocessBuild Error: " + rABBuildEntry.ABBuildPreprocessor);
                return rABBuilds;
            }
            return rPreprocessor.ReconstructorABBuilds(rABBuilds);
        }

        public static void PostprocessBuildNoneAssetbundle(ABBuildEntry rABBuildEntry)
        {
            if (rABBuildEntry.IsAssetBundle) return;

            if (rABBuildEntry.ABBuildType == ABBuildType.Dir ||
                rABBuildEntry.ABBuildType == ABBuildType.Dir_Dir ||
                rABBuildEntry.ABBuildType == ABBuildType.Dir_Dir_Dir)
            {
                Debug.LogError("None assetbundle and ABBuildType is directory, error!!!");
                return;
            }

            if (rABBuildEntry.ABBuildType == ABBuildType.File)
            {
                if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                {
                    var rResPath = rABBuildEntry.ResPath + "." + rABBuildEntry.ABVariant;
                    var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName + "." + rABBuildEntry.ABVariant;
                    PathTool.CopyFile(rResPath, rAssetBundlePath);
                }
                else
                {
                    var rResPath = rABBuildEntry.ResPath;
                    var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName;
                    PathTool.CopyFile(rResPath, rAssetBundlePath);
                }
            }
            else if (rABBuildEntry.ABBuildType == ABBuildType.Dir_File)
            {
                var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
                foreach (var rGUID in rGUIDs)
                {
                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                    if (AssetDatabase.IsValidFolder(rAssetPath)) continue;

                    if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                    {
                        var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName + "/" +
                                               Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + "." + rABBuildEntry.ABVariant;
                        PathTool.CopyFile(rAssetPath, rAssetBundlePath);
                    }
                    else
                    {
                        var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName + "/" +
                                               Path.GetFileName(rAssetPath).ToLower();
                        PathTool.CopyFile(rAssetPath, rAssetBundlePath);
                    }
                }
            }
            else if (rABBuildEntry.ABBuildType == ABBuildType.Dir_Dir_File)
            {
                var rDirectoryInfo = new System.IO.DirectoryInfo(rABBuildEntry.ResPath);
                var rAllDirectoryInfos = rDirectoryInfo.GetDirectories();
                foreach (var rDirInfo in rAllDirectoryInfos)
                {
                    var rDirAssetPath = rABBuildEntry.ResPath + "/" + rDirInfo.Name;
                    var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rDirAssetPath });
                    foreach (var rGUID in rGUIDs)
                    {
                        if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                        {
                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                            var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName + "/" + rDirInfo.Name.ToLower() + "/" +
                                                   Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + "." + rABBuildEntry.ABVariant;
                            PathTool.CopyFile(rAssetPath, rAssetBundlePath);
                        }
                        else
                        {
                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUID);
                            var rAssetBundlePath = ABPlatform.Instance.GetCurPlatformABDir() + rABBuildEntry.ABName + "/" + rDirInfo.Name.ToLower() + "/" +
                                                   Path.GetFileName(rAssetPath).ToLower();
                            PathTool.CopyFile(rAssetPath, rAssetBundlePath);
                        }
                    }
                }
            }
        }
    }
}
