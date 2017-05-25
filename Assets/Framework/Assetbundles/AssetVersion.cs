//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;

namespace UnityEngine.AssetBundles
{
    [System.Serializable]
    public class AssetVersionEntry
    {
        public string   N;
        public int      V;
        public string   M;
        public long     S;
        public string[] D;
        
        public string   Name         { get { return N; } set { N = value; } }
        public int      Version      { get { return V; } set { V = value; } }
        public string   MD5          { get { return M; } set { M = value; } }
        public long     Size         { get { return S; } set { S = value; } }
        public string[] Dependencies { get { return D; } set { D = value; } }
    }

    public partial class AssetVersion : ScriptableObject
    {
        public Dict<string, AssetVersionEntry> Entries;

        public AssetVersionEntry GetEntry(string rABName)
        {
            if (this.Entries == null) return null;
            AssetVersionEntry rEntry = null;
            this.Entries.TryGetValue(rABName, out rEntry);
            return rEntry;
        }
    }
}
