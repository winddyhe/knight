using System.IO;
using System.Collections.Generic;
using Knight.Core;
using Knight.Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.Serializer
{
	public static class CommonSerializer
	{
		public static void Serialize(this BinaryWriter rWriter, List<ETHotfix.PlayerInfo> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}

		public static List<ETHotfix.PlayerInfo> Deserialize(this BinaryReader rReader, List<ETHotfix.PlayerInfo> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<ETHotfix.PlayerInfo>(nCount);
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult.Add(rReader.Deserialize(default(ETHotfix.PlayerInfo)));
			return rResult;
		}

		public static void Serialize(this BinaryWriter rWriter, List<ETHotfix.UnitInfo> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}

		public static List<ETHotfix.UnitInfo> Deserialize(this BinaryReader rReader, List<ETHotfix.UnitInfo> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<ETHotfix.UnitInfo>(nCount);
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult.Add(rReader.Deserialize(default(ETHotfix.UnitInfo)));
			return rResult;
		}

	}
}