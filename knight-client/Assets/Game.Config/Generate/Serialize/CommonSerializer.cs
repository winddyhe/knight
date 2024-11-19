using System.Collections.Generic;
using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
/*class type references, do not modify
common ref:
System.Double[]:Game.Sample1Config,Game.Sample2Config,
System.Double[][]:Game.Sample1Config,Game.Sample2Config,
System.Int32[]:Game.Sample1Config,Game.Sample2Config,Game.StageConfig,Game.UnitConfig,
System.Int32[][]:Game.Sample1Config,Game.Sample2Config,
System.Int64[]:Game.Sample1Config,Game.Sample2Config,
System.Int64[][]:Game.Sample1Config,Game.Sample2Config,
System.Single[]:Game.Sample1Config,Game.Sample2Config,
System.Single[][]:Game.Sample1Config,Game.Sample2Config,
System.String[]:Game.Sample1Config,Game.Sample2Config,
System.String[][]:Game.Sample1Config,Game.Sample2Config,
private ref:
System.Collections.Generic.Dictionary`2[System.Int32,Game.Sample1Config]:Game.Sample1ConfigTable
System.Collections.Generic.Dictionary`2[System.Int32,Game.Sample2Config]:Game.Sample2ConfigTable
System.Collections.Generic.Dictionary`2[System.Int32,Game.SkillConfig]:Game.SkillConfigTable
System.Collections.Generic.Dictionary`2[System.Int32,Game.StageConfig]:Game.StageConfigTable
System.Collections.Generic.Dictionary`2[System.Int32,Game.UnitConfig]:Game.UnitConfigTable
System.Collections.Generic.Dictionary`2[System.String,Game.LanguageConfig]:Game.LanguageConfigTable
System.Collections.Generic.Dictionary`2[System.String,Game.SkillAssetConfig]:Game.SkillAssetConfigTable
*/

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public static partial class CommonSerializer
	{
		public static void Serialize(BinaryWriter rWriter, int[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				rWriter.Serialize(value[nIndex]);
		}

		public static int[] Deserialize(BinaryReader rReader, int[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new int[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = rReader.Deserialize(default(int));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, long[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				rWriter.Serialize(value[nIndex]);
		}

		public static long[] Deserialize(BinaryReader rReader, long[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new long[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = rReader.Deserialize(default(long));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, float[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				rWriter.Serialize(value[nIndex]);
		}

		public static float[] Deserialize(BinaryReader rReader, float[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new float[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = rReader.Deserialize(default(float));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, double[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				rWriter.Serialize(value[nIndex]);
		}

		public static double[] Deserialize(BinaryReader rReader, double[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new double[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = rReader.Deserialize(default(double));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, string[] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				rWriter.Serialize(value[nIndex]);
		}

		public static string[] Deserialize(BinaryReader rReader, string[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new string[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = rReader.Deserialize(string.Empty);
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, int[][] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				CommonSerializer.Serialize(rWriter, value[nIndex]);
		}

		public static int[][] Deserialize(BinaryReader rReader, int[][] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new int[nCount][];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = CommonSerializer.Deserialize(rReader, default(int[]));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, long[][] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				CommonSerializer.Serialize(rWriter, value[nIndex]);
		}

		public static long[][] Deserialize(BinaryReader rReader, long[][] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new long[nCount][];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = CommonSerializer.Deserialize(rReader, default(long[]));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, float[][] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				CommonSerializer.Serialize(rWriter, value[nIndex]);
		}

		public static float[][] Deserialize(BinaryReader rReader, float[][] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new float[nCount][];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = CommonSerializer.Deserialize(rReader, default(float[]));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, double[][] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				CommonSerializer.Serialize(rWriter, value[nIndex]);
		}

		public static double[][] Deserialize(BinaryReader rReader, double[][] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new double[nCount][];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = CommonSerializer.Deserialize(rReader, default(double[]));
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, string[][] value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Length);
			for (int nIndex = 0; nIndex < value.Length; nIndex++)
				CommonSerializer.Serialize(rWriter, value[nIndex]);
		}

		public static string[][] Deserialize(BinaryReader rReader, string[][] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new string[nCount][];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult[nIndex] = CommonSerializer.Deserialize(rReader, default(string[]));
			return rResult;
		}

	}
}