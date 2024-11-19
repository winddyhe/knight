using System.Collections.Generic;
using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Knight.Framework.Assetbundle;

//ScriptMD5:FC0BBEBF34436DA64608E3F74D835F0A

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.Assetbundle
{
	public partial class ABEntry : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ABPath);
			rWriter.Serialize(this.ABVaraint);
			CommonSerializer.Serialize(rWriter, this.AssetList);
			CommonSerializer.Serialize(rWriter, this.Dependencies);
			rWriter.Serialize(this.IsAssetBundle);
			rWriter.Serialize(this.IsDeleteAB);
			rWriter.Serialize(this.MD5);
			rWriter.Serialize(this.Size);
			rWriter.Serialize(this.Version);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ABPath = rReader.Deserialize(this.ABPath);
			this.ABVaraint = rReader.Deserialize(this.ABVaraint);
			this.AssetList = CommonSerializer.Deserialize(rReader, this.AssetList);
			this.Dependencies = CommonSerializer.Deserialize(rReader, this.Dependencies);
			this.IsAssetBundle = rReader.Deserialize(this.IsAssetBundle);
			this.IsDeleteAB = rReader.Deserialize(this.IsDeleteAB);
			this.MD5 = rReader.Deserialize(this.MD5);
			this.Size = rReader.Deserialize(this.Size);
			this.Version = rReader.Deserialize(this.Version);
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
		public static Knight.Framework.Assetbundle.ABEntry Deserialize(BinaryReader rReader, Knight.Framework.Assetbundle.ABEntry value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Knight.Framework.Assetbundle.ABEntry();
			rResult.Deserialize(rReader);
			return rResult;
		}

		public static void Serialize(BinaryWriter rWriter, List<string> value)
		{
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}

		public static List<string> Deserialize(BinaryReader rReader, List<string> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
			var rResult = new List<string>(nCount);
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				rResult.Add(rReader.Deserialize(string.Empty));
			return rResult;
		}

	}
}
