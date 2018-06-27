//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Core.Serializer.Editor
{
    public class CodeGenerator_CommonSerializer : CodeGenerator
    {
        private List<Type>      mGeneratedArray             = new List<Type>();
        private List<Type>      mGeneratedDynamicArray      = new List<Type>();
        private List<Type>      mGeneratedList              = new List<Type>();
        private List<Type>      mGeneratedDynamicList       = new List<Type>();
        private List<Type>      mGeneratedDictionary        = new List<Type>();
        private List<Type>      mGeneratedDynamicDictionary = new List<Type>();

        public CodeGenerator_CommonSerializer(string rFilePath)
            : base(rFilePath)
        {

        }

        public override void WriteHead()
        {
            this.StringBuilder?
                .A("using System.IO;").N()
                .A("using System.Collections.Generic;").N()
                .A("using Knight.Core;").N()
                .A("using Knight.Core.Serializer;").N()
                .L(1)
                .A("/// <summary>").N()
                .A("/// Auto generate code, not need modify.").N()
                .A("/// </summary>").N()
                .A("namespace Knight.Framework.Serializer").N()
                .A("{").N()
                .T(1).A("public static class CommonSerializer").N()
                .T(1).A("{").N();
        }

        public override void WriteEnd()
        {
            this.StringBuilder?
                .T(1).A("}").N()
                .A("}");
        }

        public void AnalyzeGenerateCommon(Type rType, bool bDynamic)
        {
            if (SerializerAssists.IsBaseType(rType)) return;

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
            else if (rType.GetInterface("System.Collections.IDictionary") != null || rType.GetInterface("Knight.Core.IDict") != null)
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

            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetElementType();

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !SerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            this.StringBuilder?
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Length);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Length; nIndex++)").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDEText, (rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]")).N()
                .T(2).A("}").N()
                .L(1);

            this.StringBuilder?
                .T(2).F("public static {0} Deserialize(this BinaryReader rReader, {1} value)", rTypeName, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0};", rTypeName.Insert(rTypeName.IndexOf('[') + 1, "nCount")).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; nIndex++)").N()
                        .T(4).F("rResult[nIndex] = {0}rReader.Deserialize{1}({2});",
                            (rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty),
                            rTDEText,
                            SerializerAssists.GetDeserializeUnwrap(rElementType)).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }

        public void WriteList(Type rType, bool bDynamic)
        {
            if (this.ReceiveGeneratedListType(rType, bDynamic)) return;

            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetGenericArguments()[0];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !SerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            this.StringBuilder?
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Count; ++ nIndex)").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDEText, (rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]")).N()
                .T(2).A("}").N()
                .L(1);

            this.StringBuilder?
                .T(2).F("public static {0} Deserialize{1}(this BinaryReader rReader, {2} value)", rTypeName, rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0}(nCount);", rTypeName).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; nIndex++)").N()
                        .T(4).F("rResult.Add({0}rReader.Deserialize{1}({2}));", 
                            (rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty), 
                            rTDEText, 
                            SerializerAssists.GetDeserializeUnwrap(rType.GetGenericArguments()[0])).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }

        public void WriteDictionary(Type rType, bool bDynamic)
        {
            if (this.ReceiveGeneratedDictionaryType(rType, bDynamic)) return;

            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rKeyType = rType.GetGenericArguments()[0];
            var rValueType = rType.GetGenericArguments()[1];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDKText = bDynamic && !SerializerAssists.IsBaseType(rKeyType, false) ? "Dynamic" : string.Empty;
            var rTDVText = bDynamic && !SerializerAssists.IsBaseType(rValueType, false) ? "Dynamic" : string.Empty;

            this.StringBuilder?
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("foreach(var rPair in value)").N()
                    .T(3).A("{").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDKText, (rKeyType.IsEnum ? "(int)rPair.Key" : "rPair.Key")).N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDVText, (rValueType.IsEnum ? "(int)rPair.Value" : "rPair.Value")).N()
                    .T(3).A("}").N()
                .T(2).A("}").N()
                .L(1);

            this.StringBuilder?
                .T(2).F("public static {0} Deserialize{1}(this BinaryReader rReader, {2} value)", rTypeName, rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0}();", rTypeName).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; ++ nIndex)").N()
                    .T(3).A("{").N()
                        .T(4).F("var rKey   = {0}rReader.Deserialize{1}({2});",
                            (rKeyType.IsEnum ? string.Format("({0})", rKeyType.FullName) : string.Empty),
                            rTDKText,
                            SerializerAssists.GetDeserializeUnwrap(rKeyType)).N()
                        .T(4).F("var rValue = {0}rReader.Deserialize{1}({2});",
                            (rValueType.IsEnum ? string.Format("({0})", rValueType.FullName) : string.Empty),
                            rTDVText,
                            SerializerAssists.GetDeserializeUnwrap(rValueType)).N()
                        .T(4).A("rResult.Add(rKey, rValue);").N()
                    .T(3).A("}").N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
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
