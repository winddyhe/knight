using Knight.Framework.Assetbundle;
using Knight.Framework.Assetbundle.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Game.Editor
{
    public class ABBuildPreprocessor_GameConfig : IABBuildPreprocessor
    {
        public void OnPreprocessBuild(ABBuildEntry rABBuildEntry)
        {
            GameSerializeTool.SerializeGameConfigJsonToBinary();
        }

        public List<AssetBundleBuild> ReconstructorABBuilds(List<AssetBundleBuild> rABBuilds)
        {
            return rABBuilds;
        }
    }
}
