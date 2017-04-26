using System.IO;
using System.Collections.Generic;
using Core;
using WindHotfix.Core;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace KnightHotfixModule.Knight
{
	public static class CommonSerializer
	{
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
		public static void Serialize(this BinaryWriter rWriter, Dict<int, KnightHotfixModule.Knight.Avatar> value)
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
		public static Dict<int, KnightHotfixModule.Knight.Avatar> Deserialize(this BinaryReader rReader, Dict<int, KnightHotfixModule.Knight.Avatar> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, KnightHotfixModule.Knight.Avatar>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(KnightHotfixModule.Knight.Avatar));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, KnightHotfixModule.Knight.Hero> value)
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
		public static Dict<int, KnightHotfixModule.Knight.Hero> Deserialize(this BinaryReader rReader, Dict<int, KnightHotfixModule.Knight.Hero> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, KnightHotfixModule.Knight.Hero>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(KnightHotfixModule.Knight.Hero));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, KnightHotfixModule.Knight.ActorProfessional> value)
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
		public static Dict<int, KnightHotfixModule.Knight.ActorProfessional> Deserialize(this BinaryReader rReader, Dict<int, KnightHotfixModule.Knight.ActorProfessional> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, KnightHotfixModule.Knight.ActorProfessional>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(KnightHotfixModule.Knight.ActorProfessional));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<int, KnightHotfixModule.Knight.StageConfig> value)
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
		public static Dict<int, KnightHotfixModule.Knight.StageConfig> Deserialize(this BinaryReader rReader, Dict<int, KnightHotfixModule.Knight.StageConfig> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid)
				return null;
			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dict<int, KnightHotfixModule.Knight.StageConfig>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(default(int));
				var rValue = rReader.Deserialize(default(KnightHotfixModule.Knight.StageConfig));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}
	}
}