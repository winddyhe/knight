using UnityEditor;

namespace Knight.Framework.Assetbundle.Editor
{
    public static class ABBuilderEditor
    {
        [MenuItem("Tools/Assetbundle/Build AssetBundles FullPackage")]
        public static void BuildAssetbundles()
        {
            ABPlatform.Instance.Initialize();
            var rOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            var rOutputPath = "./" + ABPlatform.Instance.GetCurPlatformABDir();
            ABBuilder.Instance.Build(rOutputPath, ABPackageType.HotfixFullPackage, rOptions, EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("Tools/Assetbundle/Sync AssetBundles To StreamingAssets")]
        public static void SyncAssetbundleToStreamingAssets()
        {
            ABPlatform.Instance.Initialize();
            var rOutputPath = "./" + ABPlatform.Instance.GetCurPlatformABDir();
            ABBuilder.Instance.SyncAssetbundleToStreamingAssets(rOutputPath);
        }

        [MenuItem("Tools/Assetbundle/Build AssetBundles FullPackage And Sync StreamingAssets")]
        public static void BuildAssetBundlesAndSyncStreamingAssets()
        {
            BuildAssetbundles();
            var rOutputPath = "./" + ABPlatform.Instance.GetCurPlatformABDir();
            ABBuilder.Instance.SyncAssetbundleToStreamingAssets(rOutputPath);
        }
    }
}