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
    public class ABVersionEntry : SerializerBinary
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

        public override void Serialize(BinaryWriter rWriter)
        {
            base.Serialize(rWriter);
            rWriter.Serialize(this.N);
            rWriter.Serialize(this.V);
            rWriter.Serialize(this.M);
            rWriter.Serialize(this.S);
            rWriter.Serialize(this.D);
        }

        public override void Deserialize(BinaryReader rReader)
        {
            base.Deserialize(rReader);
            this.N = rReader.Deserialize(this.N);
            this.V = rReader.Deserialize(this.V);
            this.M = rReader.Deserialize(this.M);
            this.S = rReader.Deserialize(this.S);
            this.D = rReader.Deserialize(this.D);
        }
    }

    public partial class ABVersion : ScriptableObject
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
