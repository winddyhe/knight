using Knight.Framework.Serializer;
using System.Collections;
using System.Collections.Generic;

namespace Knight.Framework.Assetbundle
{
    public enum ABBuildType
    {
        File,
        Dir,
        Dir_File,
        Dir_Dir,
        Dir_Dir_File,
        Dir_Dir_Dir
    }

    [System.Serializable]
    public partial class ABBuildEntry
    {
        public ABBuildType ABBuildType;
        public string ABName;
        public string ABAliasSuffix;
        public string ABVariant;
        public string ResPath;
        public string FilterType;
        public string FilterPattern;
        public string ABBuildPreprocessor;
        public bool IsAssetBundle = true;
        public bool IsNeedAssetList = false;

        public string ABFullName => string.IsNullOrEmpty(this.ABVariant) ? (this.ABName + this.ABAliasSuffix) : (this.ABName + this.ABAliasSuffix + "." + this.ABVariant);

        public void Clone(ABBuildEntry rABBuildEntry)
        {
            this.ABBuildType = rABBuildEntry.ABBuildType;
            this.ABName = rABBuildEntry.ABName;
            this.ABAliasSuffix = rABBuildEntry.ABAliasSuffix;
            this.ABVariant = rABBuildEntry.ABVariant;
            this.ResPath = rABBuildEntry.ResPath;
            this.FilterType = rABBuildEntry.FilterType;
            this.FilterPattern = rABBuildEntry.FilterPattern;
            this.ABBuildPreprocessor = rABBuildEntry.ABBuildPreprocessor;
            this.IsAssetBundle = rABBuildEntry.IsAssetBundle;
            this.IsNeedAssetList = rABBuildEntry.IsNeedAssetList;
        }
    }
}

