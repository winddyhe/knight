using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABBuildPreprocessorDefault : IABBuildPreprocessor
    {
        public void OnPreprocessBuild(ABBuildEntry rABBuildEntry)
        {
        }

        public List<AssetBundleBuild> ReconstructorABBuilds(List<AssetBundleBuild> rABBuilds)
        {
            return rABBuilds;
        }
    }
}