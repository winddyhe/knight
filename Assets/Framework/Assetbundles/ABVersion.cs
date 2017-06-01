//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Core.Serializer;
using System.IO;

namespace UnityEngine.AssetBundles
{
    [System.Serializable]
    public partial class ABVersionEntry : SerializerBinary
    {
        public string   N;
        public int      V;
        public string   M;
        public long     S;
        public string[] D;
        
        [SBIgnore]
        public string   Name         { get { return N; } set { N = value; } }
        [SBIgnore]
        public int      Version      { get { return V; } set { V = value; } }
        [SBIgnore]
        public string   MD5          { get { return M; } set { M = value; } }
        [SBIgnore]
        public long     Size         { get { return S; } set { S = value; } }
        [SBIgnore]
        public string[] Dependencies { get { return D; } set { D = value; } }
    }

    public partial class ABVersion : SerializerBinary
    {
        public Dict<string, ABVersionEntry> Entries;

        public ABVersionEntry GetEntry(string rABName)
        {
            if (this.Entries == null) return null;
            ABVersionEntry rEntry = null;
            this.Entries.TryGetValue(rABName, out rEntry);
            return rEntry;
        }
    }
}
