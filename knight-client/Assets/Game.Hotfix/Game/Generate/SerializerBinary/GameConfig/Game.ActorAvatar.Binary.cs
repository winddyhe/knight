using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class ActorAvatar
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.AvatarName);
			rWriter.Serialize(this.ABPath);
			rWriter.Serialize(this.AssetName);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.ID = rReader.Deserialize(this.ID);
			this.AvatarName = rReader.Deserialize(this.AvatarName);
			this.ABPath = rReader.Deserialize(this.ABPath);
			this.AssetName = rReader.Deserialize(this.AssetName);
		}
	}
}

