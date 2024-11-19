using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;
using UnityEditor;

namespace Knight.Framework.Assetbundle.Editor
{
    public interface IABBuildPreprocessor
    {
        void OnPreprocessBuild(ABBuildEntry rABBuildEntry);
        List<AssetBundleBuild> ReconstructorABBuilds(List<AssetBundleBuild> rABBuilds);
    }

    public class ABBuildPreprocessorTypes : TypeSearchDefault<IABBuildPreprocessor>
    {
    }
}
