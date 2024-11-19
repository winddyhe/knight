using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:045475EE1493698C704558C36F977EF8

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class StageConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			CommonSerializer.Serialize(rWriter, this.Heros);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.SceneName);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.Heros = CommonSerializer.Deserialize(rReader, this.Heros);
			this.ID = rReader.Deserialize(this.ID);
			this.SceneName = rReader.Deserialize(this.SceneName);
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
		public static Game.StageConfig Deserialize(BinaryReader rReader, Game.StageConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.StageConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
