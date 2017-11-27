
using System.IO;
using System.Collections.Generic;
using Core;
using Core.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Serializer
{
    public static class CommonSerializer
    {
		public static void Serialize(this BinaryWriter rWriter, string[] value)
		{
			var bValid = (null != value);
	        rWriter.Serialize(bValid);
	        if (!bValid) return;

	        rWriter.Serialize(value.Length); 
	        for (int nIndex = 0; nIndex < value.Length; nIndex++)
                rWriter.Serialize(value[nIndex]);
		}
		public static string[] Deserialize(this BinaryReader rReader, string[] value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

            var nCount  = rReader.Deserialize(default(int));
            var rResult = new string[nCount];
			for (int nIndex = 0; nIndex < nCount; nIndex++)
                rResult[nIndex] = rReader.Deserialize(string.Empty);
            return rResult;
		}
		public static void Serialize(this BinaryWriter rWriter, Dict<string, UnityEngine.AssetBundles.ABVersionEntry> value)
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
		public static Dict<string, UnityEngine.AssetBundles.ABVersionEntry> Deserialize(this BinaryReader rReader, Dict<string, UnityEngine.AssetBundles.ABVersionEntry> value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
            var rResult = new Dict<string, UnityEngine.AssetBundles.ABVersionEntry>();
            for (int nIndex = 0; nIndex < nCount; ++ nIndex)
            {
	            var rKey   = rReader.Deserialize(string.Empty);
                var rValue = rReader.Deserialize(default(UnityEngine.AssetBundles.ABVersionEntry));
                rResult.Add(rKey, rValue);
            }

            return rResult;
		}
	}
}
