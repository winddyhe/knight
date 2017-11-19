using System.IO;
using System.Collections.Generic;
using Core;
using WindHotfix.Core;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Game.Knight
{
	public static class CommonSerializer
	{
		public static void Serialize(this BinaryWriter rWriter, List<Game.Knight.GPCSymbolItem> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static List<Game.Knight.GPCSymbolItem> Deserialize(this BinaryReader rReader, List<Game.Knight.GPCSymbolItem> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<Game.Knight.GPCSymbolItem>(nCount);
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult.Add(rReader.Deserialize(default(Game.Knight.GPCSymbolItem)));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, List<Game.Knight.GPCSymbolElement> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static List<Game.Knight.GPCSymbolElement> Deserialize(this BinaryReader rReader, List<Game.Knight.GPCSymbolElement> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<Game.Knight.GPCSymbolElement>(nCount);
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult.Add(rReader.Deserialize(default(Game.Knight.GPCSymbolElement)));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, List<Game.Knight.GPCSymbolObject>> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}
		public static Dict<int, List<Game.Knight.GPCSymbolObject>> Deserialize(this BinaryReader rReader, Dict<int, List<Game.Knight.GPCSymbolObject>> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, List<Game.Knight.GPCSymbolObject>>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(List<Game.Knight.GPCSymbolObject>));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, List<Game.Knight.GPCSymbolObject> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static List<Game.Knight.GPCSymbolObject> Deserialize(this BinaryReader rReader, List<Game.Knight.GPCSymbolObject> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<Game.Knight.GPCSymbolObject>(nCount);
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult.Add(rReader.Deserialize(default(Game.Knight.GPCSymbolObject)));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, List<Game.Knight.UnitInfo> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static List<Game.Knight.UnitInfo> Deserialize(this BinaryReader rReader, List<Game.Knight.UnitInfo> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<Game.Knight.UnitInfo>(nCount);
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult.Add(rReader.Deserialize(default(Game.Knight.UnitInfo)));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, List<WindHotfix.Net.AFrameMessage> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static List<WindHotfix.Net.AFrameMessage> Deserialize(this BinaryReader rReader, List<WindHotfix.Net.AFrameMessage> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<WindHotfix.Net.AFrameMessage>(nCount);
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult.Add(rReader.Deserialize(default(WindHotfix.Net.AFrameMessage)));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, float[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
		public static float[] Deserialize(this BinaryReader rReader, float[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new float[nCount];
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
				rResult[nIndex] = rReader.Deserialize(default(float));
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, Game.Knight.ActorAvatar> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}
		public static Dict<int, Game.Knight.ActorAvatar> Deserialize(this BinaryReader rReader, Dict<int, Game.Knight.ActorAvatar> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, Game.Knight.ActorAvatar>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(Game.Knight.ActorAvatar));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, Game.Knight.ActorHero> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}
		public static Dict<int, Game.Knight.ActorHero> Deserialize(this BinaryReader rReader, Dict<int, Game.Knight.ActorHero> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, Game.Knight.ActorHero>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(Game.Knight.ActorHero));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, Game.Knight.ActorProfessional> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}
		public static Dict<int, Game.Knight.ActorProfessional> Deserialize(this BinaryReader rReader, Dict<int, Game.Knight.ActorProfessional> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, Game.Knight.ActorProfessional>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(Game.Knight.ActorProfessional));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, Game.Knight.StageConfig> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid)
				return;
			rWriter.Serialize(value.Count);
			foreach(var rPair in value)
			{
				rWriter.Serialize(rPair.Key);
				rWriter.Serialize(rPair.Value);
			}
		}
		public static Dict<int, Game.Knight.StageConfig> Deserialize(this BinaryReader rReader, Dict<int, Game.Knight.StageConfig> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, Game.Knight.StageConfig>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(Game.Knight.StageConfig));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
	}
}