
using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace ETHotfix
{
	public partial class Frame_ClickMap
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.RpcId);
			rWriter.Serialize(this.Id);
			rWriter.Serialize(this.X);
			rWriter.Serialize(this.Z);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.RpcId = rReader.Deserialize(this.RpcId);
			this.Id = rReader.Deserialize(this.Id);
			this.X = rReader.Deserialize(this.X);
			this.Z = rReader.Deserialize(this.Z);
		}
    }
}


