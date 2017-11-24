//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Core.Serializer.Editor
{
    public class CodeGenerator_CommonSerializer : CodeGenerator
    {
        public CodeGenerator_CommonSerializer(string rFilePath)
            : base(rFilePath)
        {

        }

        public override void WriteHead()
        {
            this.Write(@"
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
    {");
        
        }

        public override void WriteEnd()
        {
            this.Write(@"    }
}
");
        }

        public void WriteArray(Type rType)
        {
            this.Write(
$"        public static void Serialize(this BinaryWriter rWriter, {rType.FullName}[] value)");
            this.Write(@"        {
            var bValid = (null != value);
	        rWriter.Serialize(bValid);
	        if (!bValid) return;

	        rWriter.Serialize(value.Length); 
	        for (int nIndex = 0; nIndex < value.Length; nIndex++)
	            rWriter.Serialize(value[nIndex]);
        }
");

            this.Write(
$"        public static {rType.FullName}[] Deserialize(this BinaryReader rReader, {rType.FullName}[] value)");
            this.Write(@"        {
            var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

            var nCount  = rReader.Deserialize(default(int));");
            this.Write(
$"            var rResult = new {rType.FullName}[nCount];");
            this.Write(@"
			for (int nIndex = 0; nIndex < nCount; nIndex++)");
            this.Write(
$"                rResult[nIndex] = rReader.Deserialize(default({rType.FullName}));");
            this.Write(@"
            return rResult;
        }
");
        }

        public void WriteList(Type rType)
        {
            this.Write(
$"        public static void Serialize(this BinaryWriter rWriter, List<{rType.FullName}> value)");
            this.Write(@"        {
			var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize(value[nIndex]);
		}
");

            this.Write(
$"        public static List<{rType.FullName}> Deserialize(this BinaryReader rReader, List<{rType.FullName}> value)");
            this.Write(@"        {
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));");
            this.Write(
$"            var rResult = new List<{rType.FullName}>(nCount);");
            this.Write(@"
			for (int nIndex = 0; nIndex < nCount; nIndex++)");
            this.Write(
$"                rResult[nIndex] = rReader.Deserialize(default({rType.FullName}));");
            this.Write(@"
			return rResult;
		}
");
        }

        public void WriteDict(Type rKeyType, Type rValueType)
        {
            this.Write(
$"        public static void Serialize(this BinaryWriter rWriter, Dict<{rKeyType.FullName}, {rValueType.FullName}> value)");
            this.Write(@"        {
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
");

            this.Write(
$"        public static Dict<{rKeyType.FullName}, {rValueType.FullName}> Deserialize(this BinaryReader rReader, Dict<{rKeyType.FullName}, {rValueType.FullName}> value)");
            this.Write(@"        {
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));");
            this.Write(
$"            var rResult = new Dict<{rKeyType.FullName}, {rValueType.FullName}>();");

            this.Write(@"
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{");
            this.Write(
$"                var rKey   = rReader.Deserialize(default({rKeyType.FullName}));");
            this.Write(
$"                var rValue = rReader.Deserialize(default({rValueType.FullName}));");
			this.Write(
@"                rResult.Add(rKey, rValue);
			}
			return rResult;
		}
");
        }

        public void WriteDictionary(Type rKeyType, Type rValueType)
        {
            this.Write(
$"        public static void Serialize(this BinaryWriter rWriter, Dictionary<{rKeyType.FullName}, {rValueType.FullName}> value)");
            this.Write(@"        {
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
");

            this.Write(
$"        public static Dictionary<{rKeyType.FullName}, {rValueType.FullName}> Deserialize(this BinaryReader rReader, Dictionary<{rKeyType.FullName}, {rValueType.FullName}> value)");
            this.Write(@"        {
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));");
            this.Write(
$"            var rResult = new Dictionary<{rKeyType.FullName}, {rValueType.FullName}>();");

            this.Write(@"
			for (int nIndex = 0; nIndex < nCount; ++ nIndex)
			{");
            this.Write(
$"                var rKey   = rReader.Deserialize(default({rKeyType.FullName}));");
            this.Write(
$"                var rValue = rReader.Deserialize(default({rValueType.FullName}));");
            this.Write(
@"                rResult.Add(rKey, rValue);
			}
			return rResult;
		}
");
        }
    }
}
