using System.Collections;
using System.Collections.Generic;
using Knight.Framework.Serializer;

namespace Knight.Framework.Assetbundle
{
    [SerializerBinary]
    public partial class ABEntry
    {
        public string ABPath;
        public string ABVaraint;
        public List<string> Dependencies;
        public string MD5;
        public int Version;
        public long Size;
        public bool IsAssetBundle;
        public bool IsDeleteAB;
        public List<string> AssetList;

        public void Clone(ABEntry rABEntry)
        {
            this.ABPath = rABEntry.ABPath;
            this.ABVaraint = rABEntry.ABVaraint;
            this.Dependencies = rABEntry.Dependencies;
            this.MD5 = rABEntry.MD5;
            this.Version = rABEntry.Version;
            this.Size = rABEntry.Size;
            this.IsAssetBundle = rABEntry.IsAssetBundle;
            this.IsDeleteAB = rABEntry.IsDeleteAB;
            this.AssetList = new List<string>(rABEntry.AssetList);
        }
    }
}