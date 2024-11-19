using System.Collections.Generic;
using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:78E59627B9AE9C1862036BB59E912B9A

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class StageConfigTable : ISerializerBinary, ISBReadWriteFile
	{
		public void Serialize(BinaryWriter rWriter)
		{
			CommonSerializer.Serialize(rWriter, this.Table);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.Table = CommonSerializer.Deserialize(rReader, this.Table);
		}
		public void Save(string rFilePath)
		{
			using (var fs = new FileStream(rFilePath, FileMode.Create, FileAccess.ReadWrite))
			{
				fs.SetLength(0);
				using (var bw = new BinaryWriter(fs))
				{
					CommonSerializer.Serialize(bw, this.Table);
				}
			}
		}
		public void Read(byte[] rBytes)
		{
			using (var ms = new MemoryStream(rBytes))
			{
				using (var br = new BinaryReader(ms))
				{
					this.Table = CommonSerializer.Deserialize(br, this.Table);
				}
			}
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
		public static Game.StageConfigTable Deserialize(BinaryReader rReader, Game.StageConfigTable value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.StageConfigTable();
			rResult.Deserialize(rReader);
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, Dictionary<int, Game.StageConfig> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}

		public static Dictionary<int, Game.StageConfig> Deserialize(BinaryReader rReader, Dictionary<int, Game.StageConfig> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dictionary<int, Game.StageConfig>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = CommonSerializer.Deserialize(rReader, default(Game.StageConfig));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}

	}
}
