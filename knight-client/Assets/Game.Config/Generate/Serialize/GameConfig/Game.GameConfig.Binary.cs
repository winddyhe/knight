using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:C46D51DAC92CFE496E9BC375D135DF55

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class GameConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.Language);
			rWriter.Serialize(this.Skill);
			rWriter.Serialize(this.SkillAsset);
			rWriter.Serialize(this.Stage);
			rWriter.Serialize(this.Unit);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.Language = CommonSerializer.Deserialize(rReader, this.Language);
			this.Skill = CommonSerializer.Deserialize(rReader, this.Skill);
			this.SkillAsset = CommonSerializer.Deserialize(rReader, this.SkillAsset);
			this.Stage = CommonSerializer.Deserialize(rReader, this.Stage);
			this.Unit = CommonSerializer.Deserialize(rReader, this.Unit);
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
		public static Game.GameConfig Deserialize(BinaryReader rReader, Game.GameConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.GameConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
