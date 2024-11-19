using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:03B801BB33B0F7148B1FB5236C2CFC8C

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class LanguageConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.Arabic);
			rWriter.Serialize(this.ChineseSimplified);
			rWriter.Serialize(this.ChineseTraditional);
			rWriter.Serialize(this.English);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.Indonesian);
			rWriter.Serialize(this.Japanese);
			rWriter.Serialize(this.Korean);
			rWriter.Serialize(this.Malay);
			rWriter.Serialize(this.Thai);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.Arabic = rReader.Deserialize(this.Arabic);
			this.ChineseSimplified = rReader.Deserialize(this.ChineseSimplified);
			this.ChineseTraditional = rReader.Deserialize(this.ChineseTraditional);
			this.English = rReader.Deserialize(this.English);
			this.ID = rReader.Deserialize(this.ID);
			this.Indonesian = rReader.Deserialize(this.Indonesian);
			this.Japanese = rReader.Deserialize(this.Japanese);
			this.Korean = rReader.Deserialize(this.Korean);
			this.Malay = rReader.Deserialize(this.Malay);
			this.Thai = rReader.Deserialize(this.Thai);
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
		public static Game.LanguageConfig Deserialize(BinaryReader rReader, Game.LanguageConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.LanguageConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
