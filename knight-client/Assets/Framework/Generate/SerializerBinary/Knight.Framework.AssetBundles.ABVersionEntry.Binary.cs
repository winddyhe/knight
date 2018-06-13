
using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.AssetBundles
{
	public partial class ABVersionEntry
	{
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
}


