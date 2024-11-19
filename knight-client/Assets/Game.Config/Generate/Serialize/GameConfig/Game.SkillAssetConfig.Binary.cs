using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:8EFD12A27B27EDB7623DE05709B24D3B

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class SkillAssetConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.SkillEffectName);
			rWriter.Serialize(this.SkillScriptName);
			rWriter.Serialize(this.SkillTimelineName);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ID = rReader.Deserialize(this.ID);
			this.SkillEffectName = rReader.Deserialize(this.SkillEffectName);
			this.SkillScriptName = rReader.Deserialize(this.SkillScriptName);
			this.SkillTimelineName = rReader.Deserialize(this.SkillTimelineName);
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
		public static Game.SkillAssetConfig Deserialize(BinaryReader rReader, Game.SkillAssetConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.SkillAssetConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
