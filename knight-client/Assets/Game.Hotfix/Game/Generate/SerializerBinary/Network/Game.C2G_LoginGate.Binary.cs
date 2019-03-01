using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class C2G_LoginGate
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.RpcId);
			rWriter.Serialize(this.Key);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.RpcId = rReader.Deserialize(this.RpcId);
			this.Key = rReader.Deserialize(this.Key);
		}
	}
}

