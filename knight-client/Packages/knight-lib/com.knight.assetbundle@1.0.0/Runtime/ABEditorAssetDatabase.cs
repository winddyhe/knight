using Knight.Core;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Knight.Framework.Assetbundle
{
    public class ABEditorAssetDatabase
    {
        private ABBuildEntryConfig mABBuildEntryConfig;
        private Dictionary<string, int> mABPathIndices;
        private Dictionary<string, List<string>> mEditorDatabase;

        public ABEditorAssetDatabase()
        {
        }

        public void Initialize(string rABBuilderConfigPath)
        {
#if UNITY_EDITOR
            this.mABBuildEntryConfig = AssetDatabase.LoadAssetAtPath<ABBuildEntryConfig>(rABBuilderConfigPath);
            if (this.mABBuildEntryConfig == null)
            {
                LogManager.LogError($"Cannot find ABBuilderConfig: {rABBuilderConfigPath}");
                return;
            }
#endif
            this.mEditorDatabase = new Dictionary<string, List<string>>();
            this.BuildAssetbundleIndices();
        }

        public string EditorGetAssetPaths(string rABName, string rAssetName)
        {
#if UNITY_EDITOR
            if (this.mABBuildEntryConfig == null || this.mEditorDatabase == null)
            {
                return null;
            }
            var rAllAssets = this.EditorGetAssetPaths(rABName);
            if (rAllAssets != null)
            {
                for (int i = 0; i < rAllAssets.Count; i++)
                {
                    if (Path.GetFileNameWithoutExtension(rAllAssets[i]).Equals(rAssetName))
                    {
                        return rAllAssets[i];
                    }
                }
            }
            return null;
#else
            return null;
#endif
        }

        public List<string> EditorGetAssetPaths(string rABName)
        {
#if UNITY_EDITOR
            if (this.mABBuildEntryConfig == null || this.mEditorDatabase == null)
            {
                return null;
            }
            if (!this.mABPathIndices.TryGetValue(rABName, out var nEntryIndex))
            {
                return null;
            }
            var rABBuildEntry = this.mABBuildEntryConfig.ABBuildEntries[nEntryIndex];
            if (rABBuildEntry == null)
            {
                return null;
            }
            if (this.mEditorDatabase.TryGetValue(rABName, out var rAllAssetPaths))
            {
                return rAllAssetPaths;
            }
            rAllAssetPaths = new List<string>();
            switch (rABBuildEntry.ABBuildType)
            {
                case ABBuildType.File:
                    {
                        var rParentDirPath = Path.GetDirectoryName(rABBuildEntry.ResPath);
                        var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rParentDirPath });
                        for (int i = 0; i < rGUIDs.Length; i++)
                        {
                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                            if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                            {
                                // 过滤掉不符合正则表达式的资源
                                var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                if (!rMatch.Success)
                                {
                                    continue;
                                }
                            }
                            var rAssetName = Path.GetFileNameWithoutExtension(rAssetPath);
                            var rABFileName = Path.GetFileNameWithoutExtension(rABName);
                            if (rAssetName.Equals(rABFileName, System.StringComparison.OrdinalIgnoreCase))
                            {
                                rAllAssetPaths.Add(rAssetPath);
                            }
                        }
                    }
                    break;
                case ABBuildType.Dir:
                    {
                        var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
                        for (int i = 0; i < rGUIDs.Length; i++)
                        {
                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                            if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                            {
                                // 过滤掉不符合正则表达式的资源
                                var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                if (!rMatch.Success)
                                {
                                    continue;
                                }
                            }
                            rAllAssetPaths.Add(rAssetPath);
                        }
                    }
                    break;
                case ABBuildType.Dir_File:
                    {
                        var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
                        bool bIsBreak = false;
                        for (int j = 0; j < rGUIDs.Length; j++)
                        {
                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[j]);
                            if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                            {
                                // 过滤掉不符合正则表达式的资源
                                var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                if (!rMatch.Success)
                                {
                                    continue;
                                }
                            }
                            var rAssetName = Path.GetFileNameWithoutExtension(rAssetPath);
                            var rABFileName = Path.GetFileNameWithoutExtension(rABName);
                            if (rAssetName.Equals(rABFileName, System.StringComparison.OrdinalIgnoreCase))
                            {
                                rAllAssetPaths.Add(rAssetPath);
                                bIsBreak = true;
                                break;
                            }
                        }
                        if (bIsBreak)
                        {
                            break;
                        }
                    }
                    break;
                case ABBuildType.Dir_Dir:
                    {
                        var rDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                        var rAllSubDirs = rDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                        for (int i = 0; i < rAllSubDirs.Length; i++)
                        {
                            if (Path.GetFileNameWithoutExtension(rABName).Equals(rAllSubDirs[i].Name, System.StringComparison.OrdinalIgnoreCase))
                            {
                                var rSubDirPath = PathTool.Combine(rABBuildEntry.ResPath, rAllSubDirs[i].Name);
                                var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rSubDirPath });
                                for (int j = 0; j < rGUIDs.Length; j++)
                                {
                                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[j]);
                                    if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                                    {
                                        // 过滤掉不符合正则表达式的资源
                                        var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                        if (!rMatch.Success)
                                        {
                                            continue;
                                        }
                                    }
                                    rAllAssetPaths.Add(rAssetPath);
                                }
                                break;
                            }
                        }
                    }
                    break;
                case ABBuildType.Dir_Dir_File:
                    {
                        var rDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                        var rAllSubDirs1 = rDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                        var rABDirStrs = rABName.Replace(rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix, "").Split('/');
                        var bIsBreak = false;
                        for (int i = 0; i < rAllSubDirs1.Length; i++)
                        {
                            if (rAllSubDirs1[i].Name.Equals(rABDirStrs[0], System.StringComparison.OrdinalIgnoreCase))
                            {
                                var rSecondSubDir = new DirectoryInfo(PathTool.Combine(rABBuildEntry.ResPath, rAllSubDirs1[i].Name));
                                var rAllSubDirs2 = rSecondSubDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                                for (int j = 0; j < rAllSubDirs2.Length; j++)
                                {
                                    if (rAllSubDirs2[j].Name.Equals(rABDirStrs[1], System.StringComparison.OrdinalIgnoreCase))
                                    {
                                        var rSubDirPath = PathTool.Combine(rABBuildEntry.ResPath, rAllSubDirs1[i].Name, rAllSubDirs2[j].Name);
                                        var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rSubDirPath });
                                        for (int k = 0; k < rGUIDs.Length; k++)
                                        {
                                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[k]);
                                            if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                                            {
                                                // 过滤掉不符合正则表达式的资源
                                                var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                                if (!rMatch.Success)
                                                {
                                                    continue;
                                                }
                                            }
                                            var rAssetName = Path.GetFileNameWithoutExtension(rAssetPath);
                                            var rABFileName = Path.GetFileNameWithoutExtension(rABName);
                                            if (rAssetName.Equals(rABFileName, System.StringComparison.OrdinalIgnoreCase))
                                            {
                                                rAllAssetPaths.Add(rAssetPath);
                                                bIsBreak = true;
                                                break;
                                            }
                                        }
                                        if (bIsBreak)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            if (bIsBreak)
                            {
                                break;
                            }
                        }
                    }
                    break;
                case ABBuildType.Dir_Dir_Dir:
                    {
                        var rDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                        var rAllSubDirs1 = rDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                        var rABDirStrs = rABName.Replace(rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix, "").Split('/');
                        var bIsBreak = false;
                        for (int i = 0; i < rAllSubDirs1.Length; i++)
                        {
                            if (rAllSubDirs1[i].Name.Equals(rABDirStrs[0], System.StringComparison.OrdinalIgnoreCase))
                            {
                                var rSecondSubDir = new DirectoryInfo(PathTool.Combine(rABBuildEntry.ResPath, rAllSubDirs1[i].Name));
                                var rAllSubDirs2 = rSecondSubDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                                for (int j = 0; j < rAllSubDirs2.Length; j++)
                                {
                                    if (rAllSubDirs2[j].Name.Equals(rABDirStrs[1], System.StringComparison.OrdinalIgnoreCase))
                                    {
                                        var rSubDirPath = PathTool.Combine(rABBuildEntry.ResPath, rAllSubDirs1[i].Name, rAllSubDirs2[j].Name);
                                        var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rSubDirPath });
                                        for (int k = 0; k < rGUIDs.Length; k++)
                                        {
                                            var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[k]);
                                            if (!string.IsNullOrEmpty(rABBuildEntry.FilterPattern))
                                            {
                                                // 过滤掉不符合正则表达式的资源
                                                var rMatch = System.Text.RegularExpressions.Regex.Match(rAssetPath, rABBuildEntry.FilterPattern);
                                                if (!rMatch.Success)
                                                {
                                                    continue;
                                                }
                                            }
                                            rAllAssetPaths.Add(rAssetPath);
                                        }
                                        bIsBreak = true;
                                    }
                                }
                            }
                            if (bIsBreak)
                            {
                                break;
                            }
                        }
                    }
                    break;
            }
            this.mEditorDatabase.Add(rABName, rAllAssetPaths);
            return rAllAssetPaths;
#else
            return null;
#endif
        }

        private void BuildAssetbundleIndices()
        {
            this.mABPathIndices = new Dictionary<string, int>();
#if UNITY_EDITOR
            for (int i = 0; i < this.mABBuildEntryConfig.ABBuildEntries.Count; i++)
            {
                var rABBuildEntry = this.mABBuildEntryConfig.ABBuildEntries[i];
                switch (rABBuildEntry.ABBuildType)
                {
                    case ABBuildType.File:
                        {
                            if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                            {
                                var rABPath = rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix + ".ab";
                                this.mABPathIndices.Add(rABPath, i);
                            }
                            else
                            {
                                var rABPath = rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix + Path.GetExtension(rABBuildEntry.ResPath);
                                this.mABPathIndices.Add(rABPath, i);
                            }
                        }
                        break;
                    case ABBuildType.Dir:
                        {
                            var rABPath = rABBuildEntry.ABName + rABBuildEntry.ABAliasSuffix + ".ab";
                            this.mABPathIndices.Add(rABPath, i);
                        }
                        break;
                    case ABBuildType.Dir_File:
                        {
                            var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rABBuildEntry.ResPath });
                            for (int j = 0; j < rGUIDs.Length; j++)
                            {
                                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[j]);
                                if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                                {
                                    var rABPath = PathTool.Combine(rABBuildEntry.ABName, Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + rABBuildEntry.ABAliasSuffix + ".ab");
                                    this.mABPathIndices.Add(rABPath, i);
                                }
                                else
                                {
                                    var rABPath = PathTool.Combine(rABBuildEntry.ABName, Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + rABBuildEntry.ABAliasSuffix + Path.GetExtension(rAssetPath));
                                    this.mABPathIndices.Add(rABPath, i);
                                }
                            }
                        }
                        break;
                    case ABBuildType.Dir_Dir:
                        {
                            var rRootDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                            var rAllDirInfos = rRootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                            for (int j = 0; j < rAllDirInfos.Length; j++)
                            {
                                var rABPath = PathTool.Combine(rABBuildEntry.ABName, rAllDirInfos[j].Name.ToLower() + rABBuildEntry.ABAliasSuffix + ".ab");
                                this.mABPathIndices.Add(rABPath, i);
                            }
                        }
                        break;
                    case ABBuildType.Dir_Dir_File:
                        {
                            var rRootDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                            var rAllDirInfos = rRootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                            for (int j = 0; j < rAllDirInfos.Length; j++)
                            {
                                var rParentPath1 = PathTool.Combine(rABBuildEntry.ResPath, rAllDirInfos[j].Name);
                                var rGUIDs = AssetDatabase.FindAssets(rABBuildEntry.FilterType, new string[] { rParentPath1 });
                                for (int k = 0; k < rGUIDs.Length; k++)
                                {
                                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[k]);
                                    if (!string.IsNullOrEmpty(rABBuildEntry.ABVariant))
                                    {
                                        var rABPath = PathTool.Combine(rABBuildEntry.ABName, rAllDirInfos[j].Name.ToLower(),
                                                                       Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + rABBuildEntry.ABAliasSuffix + ".ab");
                                        this.mABPathIndices.Add(rABPath, i);
                                    }
                                    else
                                    {
                                        var rABPath = PathTool.Combine(rABBuildEntry.ABName, rAllDirInfos[j].Name.ToLower(),
                                                                       Path.GetFileNameWithoutExtension(rAssetPath).ToLower() + rABBuildEntry.ABAliasSuffix + Path.GetExtension(rAssetPath));
                                        this.mABPathIndices.Add(rABPath, i);
                                    }
                                }
                            }
                        }
                        break;
                    case ABBuildType.Dir_Dir_Dir:
                        {
                            var rRootDirInfo = new DirectoryInfo(rABBuildEntry.ResPath);
                            var rAllDirInfos = rRootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                            for (int j = 0; j < rAllDirInfos.Length; j++)
                            {
                                var rParentPath1 = new DirectoryInfo(PathTool.Combine(rABBuildEntry.ResPath, rAllDirInfos[j].Name));
                                var rAllDirInfos1 = rParentPath1.GetDirectories("*", SearchOption.TopDirectoryOnly);
                                for (int k = 0; k < rAllDirInfos1.Length; k++)
                                {
                                    var rABPath = PathTool.Combine(rABBuildEntry.ABName, rAllDirInfos[j].Name.ToLower(), 
                                                                   rAllDirInfos1[k].Name.ToLower() + rABBuildEntry.ABAliasSuffix + ".ab");
                                    this.mABPathIndices.Add(rABPath, i);
                                }
                            }
                        }
                        break;
                }
            }
#endif
        }
    }
}
