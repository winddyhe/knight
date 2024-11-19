using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Knight.Framework.ShaderVariant.Editor
{
    public class SVCCombine
    {
        [MenuItem("Tools/Shader/ShaderVariant Increment Add")]
        static void IncAdd()
        {
            SVCCombine r = new SVCCombine();
            r.Begin("Assets/SVC/ShaderVariants");
        }

        Dictionary<string, ShaderVariantCollection> mSVCDict = new Dictionary<string, ShaderVariantCollection>();

        void Begin(string rPath)
        {
            ShaderUtils.ClearCache();
            this.LoadShaderVariants("Assets/SVC/ShaderVariants");
            this.CombineSVC("Assets/SVC/ManualSaved.shadervariants");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void CombineSVC(string rPath)
        {
            var manualSVC = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(rPath);
            var rSVCData = ShaderVariantCollectionHelper.ExtractData(manualSVC);
            foreach (var rSVPair in rSVCData)
            {
                string rShaderPath = AssetDatabase.GetAssetPath(rSVPair.Key);
                string rShaderVariantsPath = System.IO.Path.ChangeExtension(rShaderPath, ".shadervariants");
                rShaderVariantsPath = rShaderVariantsPath.Insert(0, "Assets/SVC/ShaderVariants/");
                string rGUID = AssetDatabase.AssetPathToGUID(rShaderPath);

                if (this.mSVCDict.ContainsKey(rGUID))
                {
                    if (this.mSVCDict[rGUID] != null)
                    {
                        ShaderVariantCollection rSVC = this.mSVCDict[rGUID];

                        bool bChanged = false;
                        foreach (var sv in rSVPair.Value)
                        {
                            ShaderVariantCollection.ShaderVariant rShaderVariant = new ShaderVariantCollection.ShaderVariant();
                            rShaderVariant.passType = sv.passType;
                            rShaderVariant.shader = sv.shader;
                            rShaderVariant.keywords = sv.keywords;
                            if (!rSVC.Contains(rShaderVariant))
                            {
                                rSVC.Add(rShaderVariant);
                                bChanged = true;
                            }
                        }

                        if (bChanged)
                            EditorUtility.SetDirty(rSVC);
                    }
                }
            }
        }

        void LoadShaderVariants(string rPath)
        {
            string[] rDirs = Directory.GetDirectories(rPath);
            for (int i = 0; i < rDirs.Length; ++i)
                this.LoadShaderVariants(rDirs[i]);

            string[] rFiles = Directory.GetFiles(rPath);
            for (int i = 0; i < rFiles.Length; ++i)
                this.LoadShaderVariantsFile(rFiles[i]);
        }

        void LoadShaderVariantsFile(string rPath)
        {
            if (!rPath.EndsWith(".shadervariants"))
                return;

            ShaderVariantCollection rTempSVC = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(rPath);
            var rSVCData = ShaderVariantCollectionHelper.ExtractData(rTempSVC);
            foreach (var sv in rSVCData)
            {
                string rShaderPath = AssetDatabase.GetAssetPath(sv.Key);
                if (rShaderPath.Contains("unity_builtin_extra"))
                    continue;
                string rGUID = AssetDatabase.AssetPathToGUID(rShaderPath);
                string rShaderVariantsPath = System.IO.Path.ChangeExtension(rShaderPath, ".shadervariants");
                rShaderVariantsPath = rShaderVariantsPath.Insert(0, "Assets/SVC/ShaderVariants/");

                ShaderVariantCollection rShaderSVC = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(rShaderVariantsPath);
                this.mSVCDict.Add(rGUID, rShaderSVC);
            }
        }
    }
}
