using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:AC8D6DB9E633DF1F06CF73C001EA93AA

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class SkillConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.SkillCastTargetType);
			rWriter.Serialize(this.SkillName);
			rWriter.Serialize(this.TargetSelectAngleOrWidth);
			rWriter.Serialize(this.TargetSelectCampType);
			rWriter.Serialize(this.TargetSelectRadiusOrHeight);
			rWriter.Serialize(this.TargetSelectSearchType);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ID = rReader.Deserialize(this.ID);
			this.SkillCastTargetType = rReader.Deserialize(this.SkillCastTargetType);
			this.SkillName = rReader.Deserialize(this.SkillName);
			this.TargetSelectAngleOrWidth = rReader.Deserialize(this.TargetSelectAngleOrWidth);
			this.TargetSelectCampType = rReader.Deserialize(this.TargetSelectCampType);
			this.TargetSelectRadiusOrHeight = rReader.Deserialize(this.TargetSelectRadiusOrHeight);
			this.TargetSelectSearchType = rReader.Deserialize(this.TargetSelectSearchType);
		}
	}
}

/// <summary>
/// CommonSerializer private ref
/// </summary>
namespace Game
{
	public static partial class CommonSerializer
	{
		public static Game.SkillConfig Deserialize(BinaryReader rReader, Game.SkillConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.SkillConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
