using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class ActorHero
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.Name);
			rWriter.Serialize(this.AvatarID);
			rWriter.Serialize(this.SkillID);
			rWriter.Serialize(this.Scale);
			rWriter.Serialize(this.Height);
			rWriter.Serialize(this.Radius);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.ID = rReader.Deserialize(this.ID);
			this.Name = rReader.Deserialize(this.Name);
			this.AvatarID = rReader.Deserialize(this.AvatarID);
			this.SkillID = rReader.Deserialize(this.SkillID);
			this.Scale = rReader.Deserialize(this.Scale);
			this.Height = rReader.Deserialize(this.Height);
			this.Radius = rReader.Deserialize(this.Radius);
		}
	}
}

