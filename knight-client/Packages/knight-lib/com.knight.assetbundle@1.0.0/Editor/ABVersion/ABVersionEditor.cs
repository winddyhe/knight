using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Knight.Core;
using Knight.Framework.Serializer;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABVersionEditor
    {
        public static ABVersion LoadABVersion(string rABVersionPath)
        {
            if (!File.Exists(rABVersionPath)) return null;

            var rAllBytes = File.ReadAllBytes(rABVersionPath);
            var rABVersion = new ABVersion();
            TSerializerBinaryReadWriteHelper.Read(rABVersion, rAllBytes);
            return rABVersion;
        }

        public static ABVersion LoadABVersionByJson(string rABVersionPath)
        {
            if (!File.Exists(rABVersionPath)) return null;

            var rJsonTxt = File.ReadAllText(rABVersionPath);
            var rABVersion = JsonConvert.DeserializeObject<ABVersion>(rJsonTxt);
            return rABVersion;
        }

        public static void SaveABVersion(string rABVersionPath, ABVersion rABVersion)
        {
            TSerializerBinaryReadWriteHelper.Save(rABVersion, rABVersionPath);
        }

        public static void SaveABVersionMD5(string rABVersionMD5Path, string rABVersionPath)
        {
            var rVersionMD5 = MD5Tool.GetFileMD5(rABVersionPath).ToHEXString();
            File.WriteAllText(rABVersionMD5Path, rVersionMD5);
        }

        public static void SaveABVersionToJson(string rABVersionPath, ABVersion rABVersion)
        {
            var rJsonTxt = JsonConvert.SerializeObject(rABVersion, Formatting.Indented);
            File.WriteAllText(rABVersionPath, rJsonTxt);
        }

        public static ABVersion ABManifestToABVersion(string rOutputPath, ABVersion rOldABVersion, 
                                                      List<AssetbundleBuildCache> rABBuilds, List<AssetbundleBuildCache> rNoneABBuilds,
                                                      AssetBundleManifest rABManifest, Dictionary<string, string> rABBuildMD5Dict)
        {
            ABVersion rABVersion = new ABVersion();
            rABVersion.Entries = new Dictionary<string, ABEntry>();
            string[] rAllAssetBundles = rABManifest.GetAllAssetBundles();
            for (int i = 0; i < rAllAssetBundles.Length; i++)
            {
                string rAssetBundleName = rAllAssetBundles[i];
                string[] rDependencies = rABManifest.GetAllDependencies(rAssetBundleName);
                ABEntry rABEntry = new ABEntry();
                rABEntry.ABPath = rAssetBundleName;
                rABEntry.ABVaraint = Path.GetExtension(rAssetBundleName).TrimStart('.'); 
                rABEntry.IsAssetBundle = true;
                rABEntry.Dependencies = new List<string>();
                for (int j = 0; j < rDependencies.Length; j++)
                {
                    rABEntry.Dependencies.Add(rDependencies[j]);
                }
                rABEntry.MD5 = rABBuildMD5Dict[rAssetBundleName];
                if (rOldABVersion != null && rOldABVersion.Entries.TryGetValue(rAssetBundleName, out var rOldABEntry))
                {
                    if (!rABEntry.MD5.Equals(rOldABEntry.MD5))
                        rABEntry.Version = rOldABEntry.Version + 1;
                    else
                        rABEntry.Version = rOldABEntry.Version;
                }
                else
                {
                    rABEntry.Version = 0;
                }
                rABEntry.Size = PathTool.GetFileSize(PathTool.Combine(rOutputPath, rAssetBundleName));
                for (int j = 0; j < rABBuilds.Count; j++)
                {
                    var rFindABName = rABBuilds[j].ABBuild.assetBundleName + "." + rABBuilds[j].ABBuild.assetBundleVariant;
                    if (rFindABName.Equals(rAssetBundleName) && rABBuilds[j].IsNeedAssetList)
                    {
                        rABEntry.AssetList = new List<string>(rABBuilds[j].ABBuild.assetNames);
                        break;
                    }
                }
                rABVersion.Entries.Add(rAssetBundleName, rABEntry);
            }
            for (int i = 0; i < rNoneABBuilds.Count; i++)
            {
                string rAssetBundleName = string.IsNullOrEmpty(rNoneABBuilds[i].ABBuild.assetBundleVariant) 
                    ? rNoneABBuilds[i].ABBuild.assetBundleName
                    : rNoneABBuilds[i].ABBuild.assetBundleName + "." + rNoneABBuilds[i].ABBuild.assetBundleVariant;
                ABEntry rABEntry = new ABEntry();
                rABEntry.ABPath = rAssetBundleName;
                rABEntry.ABVaraint = rNoneABBuilds[i].ABBuild.assetBundleVariant;
                rABEntry.Dependencies = new List<string>();
                rABEntry.IsAssetBundle = false;
                rABEntry.MD5 = rABBuildMD5Dict[rAssetBundleName];
                if (rOldABVersion != null && rOldABVersion.Entries.TryGetValue(rAssetBundleName, out var rOldABEntry))
                {
                    if (!rABEntry.MD5.Equals(rOldABEntry.MD5))
                        rABEntry.Version = rOldABEntry.Version + 1;
                    else
                        rABEntry.Version = rOldABEntry.Version;
                }
                else
                {
                    rABEntry.Version = 0;
                }
                rABEntry.Size = PathTool.GetFileSize(PathTool.Combine(rOutputPath, rAssetBundleName));
                for (int j = 0; j < rNoneABBuilds.Count; j++)
                {
                    if (rNoneABBuilds[j].ABBuild.assetBundleName.Equals(rAssetBundleName) && rNoneABBuilds[j].IsNeedAssetList)
                    {
                        rABEntry.AssetList = new List<string>(rNoneABBuilds[j].ABBuild.assetNames);
                        break;
                    }
                }
                rABVersion.Entries.Add(rAssetBundleName, rABEntry);
            }
            return rABVersion;
        }

        public static ABVersion ABVersionEncryptABName(ABVersion rABVersion, out Dictionary<string, string> rEncryptPathDict)
        {
            rEncryptPathDict = new Dictionary<string, string>();
            var rEncrptABVersion = new ABVersion();
            rEncrptABVersion.Entries = new Dictionary<string, ABEntry>();
            foreach (var rABEntry in rABVersion.Entries)
            {
                if(rABEntry.Value.IsAssetBundle)
                {
                    var rEncrpytABEntry = new ABEntry();
                    rEncrpytABEntry.ABPath = MD5Tool.GetStringMD5(rABEntry.Value.ABPath).ToHEXString().ToLower();
                    rEncrpytABEntry.ABVaraint = rABEntry.Value.ABVaraint;
                    rEncrpytABEntry.MD5 = rABEntry.Value.MD5;
                    rEncrpytABEntry.Version = rABEntry.Value.Version;
                    rEncrpytABEntry.Size = rABEntry.Value.Size;
                    rEncrpytABEntry.IsAssetBundle = rABEntry.Value.IsAssetBundle;
                    rEncrpytABEntry.Dependencies = new List<string>();
                    for (int i = 0; i < rABEntry.Value.Dependencies.Count; i++)
                    {
                        rEncrpytABEntry.Dependencies.Add(MD5Tool.GetStringMD5(rABEntry.Value.Dependencies[i]).ToHEXString().ToLower());
                    }
                    rEncrptABVersion.Entries.Add(rEncrpytABEntry.ABPath, rEncrpytABEntry);
                    rEncryptPathDict.Add(rABEntry.Key, rEncrpytABEntry.ABPath);
                }
                else
                {
                    var rEncrpytABEntry = new ABEntry();
                    rEncrpytABEntry.ABPath = string.IsNullOrEmpty(rABEntry.Value.ABVaraint) 
                        ? MD5Tool.GetStringMD5(rABEntry.Value.ABPath).ToHEXString().ToLower()
                        : MD5Tool.GetStringMD5(rABEntry.Value.ABPath).ToHEXString().ToLower() + "." + rABEntry.Value.ABVaraint;
                    rEncrpytABEntry.ABVaraint = rABEntry.Value.ABVaraint;
                    rEncrpytABEntry.MD5 = rABEntry.Value.MD5;
                    rEncrpytABEntry.Version = rABEntry.Value.Version;
                    rEncrpytABEntry.Size = rABEntry.Value.Size;
                    rEncrpytABEntry.IsAssetBundle = rABEntry.Value.IsAssetBundle;
                    rEncrpytABEntry.Dependencies = new List<string>();
                    rEncrptABVersion.Entries.Add(rEncrpytABEntry.ABPath, rEncrpytABEntry);
                    rEncryptPathDict.Add(rABEntry.Key, rEncrpytABEntry.ABPath);
                }
            }
            return rEncrptABVersion;
        }
    }
}