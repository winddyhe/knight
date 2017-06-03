//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.AssetBundles;
using Core;
using System.IO;
using Core.WindJson;
using System;

namespace UnityEditor.AssetBundles
{
    public static class ABVersionEditor
    {
        public static ABVersion Load(string rOutPath)
        {
            string rVersionPath = Path.Combine(rOutPath, ABVersion.ABVersion_File_Bin);
            if (!File.Exists(rVersionPath)) return null;
                
            ABVersion rABVersion = new ABVersion();
            using (FileStream fs = new FileStream(rVersionPath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    rABVersion.Deserialize(br);
                }
            }
            return rABVersion;
        }

        public static void SaveInEditor(this ABVersion rVersion, string rOutPath)
        {
            if (rVersion == null) return;
            
            string rVersionBinPath  = Path.Combine(rOutPath, ABVersion.ABVersion_File_Bin);
            string rVersionJsonPath = Path.Combine(rOutPath, ABVersion.ABVersion_File_Json);
            string rVersionMD5Path  = Path.Combine(rOutPath, ABVersion.ABVersion_File_MD5);

            using (FileStream fs = new FileStream(rVersionBinPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    rVersion.Serialize(bw);
                }
            }
            JsonNode rJsonNode = JsonParser.ToJsonNode(rVersion);
            File.WriteAllText(rVersionJsonPath, rJsonNode.ToString());

            string rVersionMD5 = UtilTool.GetMD5(rVersionBinPath).ToHEXString();
            File.WriteAllText(rVersionMD5Path, rVersionMD5);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void SaveHistory(this ABVersion rVersion, string rOutPath)
        {
            if (rVersion == null) return;

            string rHistoryDateStr = string.Format("History/{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            string rVersionBinPath = UtilTool.PathCombine(rOutPath, rHistoryDateStr, ABVersion.ABVersion_File_Bin);

            string rDirPath = Path.GetDirectoryName(rVersionBinPath);
            if (!Directory.Exists(rDirPath)) Directory.CreateDirectory(rDirPath);

            using (FileStream fs = new FileStream(rVersionBinPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    rVersion.Serialize(bw);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void SaveIncrement(this ABVersion rVersion, string rABPath, string rTargetPath)
        {
            if (rVersion == null) return;

            // 保存增量版本文件
            string rTargetVersionBinPath = UtilTool.PathCombine(rTargetPath, ABVersion.ABVersion_File_Bin);
            using (FileStream fs = new FileStream(rTargetVersionBinPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    rVersion.Serialize(bw);
                }
            }

            // 复制增量AB包
            foreach (var rVersionPair in rVersion.Entries)
            {
                var rAVEntry = rVersionPair.Value;
                
                string rSrcABPath = UtilTool.PathCombine(rABPath, rAVEntry.Name);
                string rDistABPath = UtilTool.PathCombine(rTargetPath, rAVEntry.Name);
                
                string rDistDir = Path.GetDirectoryName(rDistABPath);
                if (!Directory.Exists(rDistDir)) Directory.CreateDirectory(rDistDir);

                File.Copy(rSrcABPath, rDistABPath, true);
            }

            // 复制MD5文件
            string rSrcMD5Path = UtilTool.PathCombine(rABPath, ABVersion.ABVersion_File_MD5);
            string rDistMD5Path = UtilTool.PathCombine(rTargetPath, ABVersion.ABVersion_File_MD5);

            string rDistMD5Dir = Path.GetDirectoryName(rDistMD5Path);
            if (!Directory.Exists(rDistMD5Dir)) Directory.CreateDirectory(rDistMD5Dir);

            File.Copy(rSrcMD5Path, rDistMD5Path, true);
        }

        public static ABVersion CreateVersion(string rOutPath, ABVersion rOldVersion, AssetBundleManifest rNewABManifest)
        {
            ABVersion rVersion = new ABVersion();
            rVersion.Entries = new Dict<string, ABVersionEntry>();

            string[] rAllAssetbundles = rNewABManifest.GetAllAssetBundles();
            for (int i = 0; i < rAllAssetbundles.Length; i++)
            {
                ABVersionEntry rAVEntry = new ABVersionEntry();
                rAVEntry.Name = rAllAssetbundles[i];

                var rOldEntry = rOldVersion != null ? rOldVersion.GetEntry(rAllAssetbundles[i]) : null;

                string rOldMD5 = rOldEntry != null ? rOldEntry.MD5 : string.Empty;
                string rNewMD5 = GetMD5InManifest(rNewABManifest, rAllAssetbundles[i]);

                rAVEntry.MD5 = rNewMD5;
                
                if (!string.IsNullOrEmpty(rOldMD5) && !rOldMD5.Equals(rNewMD5))
                    rAVEntry.Version = GetVersionInABVersion(rOldVersion, rAVEntry.Name) + 1;
                else
                    rAVEntry.Version = GetVersionInABVersion(rOldVersion, rAVEntry.Name);

                rAVEntry.Size = GetABSizeInManifest(rOutPath, rAllAssetbundles[i]);
                rAVEntry.Dependencies = rNewABManifest.GetDirectDependencies(rAllAssetbundles[i]);
                rVersion.Entries.Add(rAllAssetbundles[i], rAVEntry);
            }
            return rVersion;
        }

        public static string GetMD5InManifest(AssetBundleManifest rManifest, string rABName)
        {
            if (rManifest == null) return string.Empty;
            return rManifest.GetAssetBundleHash(rABName).ToString();
        }

        public static string GetMD5ForABVersion(string rOutPath)
        {
            string rVersionMD5Path = Path.Combine(rOutPath, ABVersion.ABVersion_File_Bin);
            if (!File.Exists(rVersionMD5Path)) return string.Empty;

            return UtilTool.GetMD5(rVersionMD5Path).ToHEXString();
        }

        public static int GetVersionInABVersion(ABVersion rVersion, string rABName)
        {
            if (rVersion == null) return 1;

            ABVersionEntry rAVEntry = rVersion.GetEntry(rABName);
            if (rAVEntry == null) return 1;
            return rAVEntry.Version;
        }

        public static long GetABSizeInManifest(string rOutPath, string rABName)
        {
            string rABFilePath = rOutPath + "/" + rABName;
            var rABFileInfo = new FileInfo(rABFilePath);
            if (rABFilePath != null)
            {
                return rABFileInfo.Length;
            }
            return 0;
        }
    }
}
