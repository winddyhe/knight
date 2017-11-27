//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;

namespace WindHotfix.Core.Editor
{
    public class HotfixCodeGenerator_CommonSerializer : HotfixCodeGenerator
    {
        private List<Type>      mGeneratedArray             = new List<Type>();
        private List<Type>      mGeneratedDynamicArray      = new List<Type>();
        private List<Type>      mGeneratedList              = new List<Type>();
        private List<Type>      mGeneratedDynamicList       = new List<Type>();
        private List<Type>      mGeneratedDictionary        = new List<Type>();
        private List<Type>      mGeneratedDynamicDictionary = new List<Type>();

        public HotfixCodeGenerator_CommonSerializer(string rFilePath)
            : base(rFilePath)
        {
        }

        public override void WriteHead()
        {
            this.Write(0, @"
using System.IO;
using System.Collections.Generic;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
    public static class CommonSerializer
    {");
        }

        public override void WriteEnd()
        {
            this.Write(1,
  @"}
}");
        }

        public void AnalyzeGenerateCommon(Type rType, bool bDynamic)
        {
            if (HotfixSerializerAssists.IsBaseType(rType)) return;

            if (rType.IsArray)
            {
                WriteArray(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetElementType(), bDynamic);
            }
            else if (rType.GetInterface("System.Collections.IList") != null)
            {
                WriteList(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic);
            }
            else if (rType.GetInterface("System.Collections.IDictionary") != null || rType.GetInterface("Core.IDict") != null)
            {
                WriteDictionary(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[1], bDynamic);
            }
        }

        public void WriteArray(Type rType, bool bDynamic)
        {
            if (this.ReceiveGeneratedArrayType(rType, bDynamic))
                return;

            var rTypeName = HotfixSerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetElementType();

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !HotfixSerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            this.WriteBraceCode(2, 
        $"public static void Serialize{rTDText}(this BinaryWriter rWriter, {rTypeName} value)",
        "{",
         $@"var bValid = (null != value);
	        rWriter.Serialize(bValid);
	        if (!bValid) return;

	        rWriter.Serialize(value.Length); 
	        for (int nIndex = 0; nIndex < value.Length; nIndex++)
                rWriter.Serialize{rTDEText}({(rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]")});", 
        "}");

            this.WriteBraceCode(2, 
        $"public static {rTypeName} Deserialize(this BinaryReader rReader, {rTypeName} value)",
        "{",
         $@"var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

            var nCount  = rReader.Deserialize(default(int));
            var rResult = new {rTypeName.Insert(rTypeName.IndexOf('[') + 1, "nCount")};
			for (int nIndex = 0; nIndex < nCount; nIndex++)
                rResult[nIndex] = {(rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty)}rReader.Deserialize{rTDEText}({HotfixSerializerAssists.GetDeserializeUnwrap(rElementType)});
            return rResult;",
         "}");
        }

        public void WriteList(Type rType, bool bDynamic)
        {
            if (this.ReceiveGeneratedListType(rType, bDynamic)) return;

            var rTypeName = HotfixSerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetGenericArguments()[0];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !HotfixSerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            this.WriteBraceCode(2, 
        $"public static void Serialize{rTDText}(this BinaryWriter rWriter, {rTypeName} value)",
        @"{",
         $@"var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
			for (int nIndex = 0; nIndex < value.Count; ++ nIndex)
				rWriter.Serialize{rTDEText}({(rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]")});",
        @"}");

            this.WriteBraceCode(2, 
        $"public static {rTypeName} Deserialize{rTDText}(this BinaryReader rReader, {rTypeName} value)",
        @"{",
         $@"var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
            var rResult = new {rTypeName}(nCount);
			for (int nIndex = 0; nIndex < nCount; nIndex++)
                rResult.Add({(rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty)}rReader.Deserialize{rTDEText}({HotfixSerializerAssists.GetDeserializeUnwrap(rType.GetGenericArguments()[0])}));
			return rResult;",
        @"}");
        }

        public void WriteDictionary(Type rType, bool bDynamic)
        {
            if (this.ReceiveGeneratedDictionaryType(rType, bDynamic)) return;

            var rTypeName = HotfixSerializerAssists.GetTypeName(rType);
            var rKeyType = rType.GetGenericArguments()[0];
            var rValueType = rType.GetGenericArguments()[1];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDKText = bDynamic && !HotfixSerializerAssists.IsBaseType(rKeyType, false) ? "Dynamic" : string.Empty;
            var rTDVText = bDynamic && !HotfixSerializerAssists.IsBaseType(rValueType, false) ? "Dynamic" : string.Empty;

            this.WriteBraceCode(2,
        $"public static void Serialize{rTDText}(this BinaryWriter rWriter, {rTypeName} value)",
        @"{",
         $@"var bValid = (null != value);
			rWriter.Serialize(bValid);
			if (!bValid) return;

			rWriter.Serialize(value.Count);
            {this.GenerateBraceCode(0, 
            "foreach(var rPair in value)",
"            {",
$@"            rWriter.Serialize{rTDKText}({(rKeyType.IsEnum ? "(int)rPair.Key" : "rPair.Key")});
				rWriter.Serialize{rTDVText}({(rValueType.IsEnum ? "(int)rPair.Value" : "rPair.Value")});",
"            }")}",
        "}");

            this.WriteBraceCode(2,
        $"public static {rTypeName} Deserialize{rTDText}(this BinaryReader rReader, {rTypeName} value)",
        @"{",
         $@"var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var nCount  = rReader.Deserialize(default(int));
            var rResult = new {rTypeName}();
            {this.GenerateBraceCode(0,
            "for (int nIndex = 0; nIndex < nCount; ++ nIndex)",
"            {",
$@"            var rKey   = {(rKeyType.IsEnum ? string.Format("({0})", rKeyType.FullName) : string.Empty)}rReader.Deserialize{rTDKText}({HotfixSerializerAssists.GetDeserializeUnwrap(rKeyType)});
                var rValue = {(rValueType.IsEnum ? string.Format("({0})", rValueType.FullName) : string.Empty)}rReader.Deserialize{rTDVText}({HotfixSerializerAssists.GetDeserializeUnwrap(rValueType)});
                rResult.Add(rKey, rValue);",
"            }")}
            return rResult;",
         "}");
		}

        private bool ReceiveGeneratedArrayType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedArray, mGeneratedDynamicArray, rType, bDynamic);
        }

        private bool ReceiveGeneratedListType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedList, mGeneratedDynamicList, rType, bDynamic);
        }
        
        private bool ReceiveGeneratedDictionaryType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedDictionary, mGeneratedDynamicDictionary, rType, bDynamic);
        }

        private bool ReceiveType(List<Type> rGenerated, List<Type> rGeneratedDynamic, Type rType, bool bDynamic)
        {
            if (bDynamic)
            {
                if (rGeneratedDynamic.Contains(rType))
                    return true;
                rGeneratedDynamic.Add(rType);
            }
            else
            {
                if (rGenerated.Contains(rType))
                    return true;
                rGenerated.Add(rType);
            }
            return false;
        }
    }
}
