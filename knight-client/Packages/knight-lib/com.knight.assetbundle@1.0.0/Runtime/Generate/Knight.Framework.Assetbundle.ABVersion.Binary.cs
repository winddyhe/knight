using System.Collections.Generic;
using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Knight.Framework.Assetbundle;

//ScriptMD5:9BA13CC78466B6298DFB3D569883830F

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.Assetbundle
{
	public partial class ABVersion : ISerializerBinary, ISBReadWriteFile
	{
		public void Serialize(BinaryWriter rWriter)
		{
			CommonSerializer.Serialize(rWriter, this.Entries);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.Entries = CommonSerializer.Deserialize(rReader, this.Entries);
		}
		public void Save(string rFilePath)
		{
			using (var fs = new FileStream(rFilePath, FileMode.Create, FileAccess.ReadWrite))
			{
				fs.SetLength(0);
				using (var bw = new BinaryWriter(fs))
				{
					CommonSerializer.Serialize(bw, this.Entries);
				}
			}
		}
		public void Read(byte[] rBytes)
		{
			using (var ms = new MemoryStream(rBytes))
			{
				using (var br = new BinaryReader(ms))
				{
					this.Entries = CommonSerializer.Deserialize(br, this.Entries);
				}
			}
		}
	}
}

/// <summary>
/// CommonSerializer private ref
/// </summary>
namespace Knight.Framework.Assetbundle
{
	public static partial class CommonSerializer
	{
		public static Knight.Framework.Assetbundle.ABVersion Deserialize(BinaryReader rReader, Knight.Framework.Assetbundle.ABVersion value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Knight.Framework.Assetbundle.ABVersion();
			rResult.Deserialize(rReader);
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, Dictionary<string, Knight.Framework.Assetbundle.ABEntry> value)
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

		public static Dictionary<string, Knight.Framework.Assetbundle.ABEntry> Deserialize(BinaryReader rReader, Dictionary<string, Knight.Framework.Assetbundle.ABEntry> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new Dictionary<string, Knight.Framework.Assetbundle.ABEntry>();
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{
				var rKey   = rReader.Deserialize(string.Empty);
				var rValue = CommonSerializer.Deserialize(rReader, default(Knight.Framework.Assetbundle.ABEntry));
				rResult.Add(rKey, rValue);
			}
			return rResult;
		}

	}
}
