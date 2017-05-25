using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.AssetBundles;
using Core;
using System.IO;

namespace UnityEditor.AssetBundles
{
    public static class AssetVersionEditor
    {
        public static void Save(this AssetVersion rVersion, string rOutPath)
        {
            if (rVersion == null) return;
            if (File.Exists(rOutPath)) File.Delete(rOutPath);

            AssetDatabase.CreateAsset(rVersion, rOutPath);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static AssetVersion CreateVersion(string rOutPath, AssetVersion rOldVersion, AssetBundleManifest rNewABManifest)
        {
            AssetVersion rVersion = ScriptableObject.CreateInstance<AssetVersion>();
            rVersion.Entries = new Dict<string, AssetVersionEntry>();

            string[] rAllAssetbundles = rNewABManifest.GetAllAssetBundles();
            for (int i = 0; i < rAllAssetbundles.Length; i++)
            {
                AssetVersionEntry rAVEntry = new AssetVersionEntry();
                rAVEntry.Name = rAllAssetbundles[i];

                var rOldEntry = rOldVersion.GetEntry(rAllAssetbundles[i]);
                string rOldMD5 = rOldEntry != null ? rOldEntry.MD5 : string.Empty;
                string rNewMD5 = GetMD5InManifest(rNewABManifest, rAllAssetbundles[i]);

                rAVEntry.MD5 = rNewMD5;
                if (!string.IsNullOrEmpty(rOldMD5) && rOldMD5.Equals(rNewMD5))
                {
                    rAVEntry.Version = rAVEntry.Version + 1;
                }
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

        public static long GetABSizeInManifest(string rOutPath, string rABName)
        {
            string rABFilePath = rOutPath + "/" + rABName;
            var rABFileInfo = new FileInfo(rABFilePath);
            if (rABFilePath != null)
            {
                return rABFilePath.Length;
            }
            return 0;
        }
    }
}
