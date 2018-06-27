using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace ETHotfix
{
	public partial class Actor_TestRequest
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.RpcId);
			rWriter.Serialize(this.ActorId);
			rWriter.Serialize(this.request);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.RpcId = rReader.Deserialize(this.RpcId);
			this.ActorId = rReader.Deserialize(this.ActorId);
			this.request = rReader.Deserialize(this.request);
		}
	}
}

