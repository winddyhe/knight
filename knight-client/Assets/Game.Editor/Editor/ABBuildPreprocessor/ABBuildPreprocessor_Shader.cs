using Knight.Framework.Assetbundle;
using Knight.Framework.Assetbundle.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Game.Editor
{
    public class ABBuildPreprocessor_Shader : IABBuildPreprocessor
    {
        public void OnPreprocessBuild(ABBuildEntry rABBuildEntry)
        {
        }

        public List<AssetBundleBuild> ReconstructorABBuilds(List<AssetBundleBuild> rABBuilds)
        {
            // 添加URP Shader的AB包
            var rABBuild = new AssetBundleBuild()
            {
                assetBundleName = "game/urp/shaders",
                assetBundleVariant = "ab"
            };
            var rShaderAssetPaths = new List<string>();
            var rSVCGUIDs = AssetDatabase.FindAssets("t:ShaderVariantCollection", new string[] { "Assets/SVC/ShaderVariants" });
            for (int i = 0; i < rSVCGUIDs.Length; i++)
            {
                var rSVCAssetPath = AssetDatabase.GUIDToAssetPath(rSVCGUIDs[i]);
                if (rSVCAssetPath.Contains("unity_builtin_extra")) continue;
                rSVCAssetPath = rSVCAssetPath.Replace("Assets/SVC/ShaderVariants/", "");
                rSVCAssetPath = rSVCAssetPath.Replace(".shadervariants", ".shader");
                if (!File.Exists(rSVCAssetPath))
                {
                    // 看是不是shadergraph
                    rSVCAssetPath = rSVCAssetPath.Replace(".shader", ".shadergraph");
                }
                rShaderAssetPaths.Add(rSVCAssetPath);
            }
            rShaderAssetPaths.Add("Assets/SVC/ShaderVariants");

            rABBuild.assetNames = rShaderAssetPaths.ToArray();

            rABBuilds.Clear();
            rABBuilds.Add(rABBuild);
            return rABBuilds;
        }
    }
}
