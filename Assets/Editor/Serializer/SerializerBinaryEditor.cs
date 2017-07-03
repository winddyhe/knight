using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using Core.Editor;

namespace Core.Serializer.Editor
{
    public class SerializerBinaryEditor : AutoCSGenerate
    {
        const string GeneratePathRoot       = "Assets/Generate/SerializerBinary/";
        const string GeneratePath           = GeneratePathRoot + "Runtime/";
        const string CommonSerializerPath   = GeneratePath + "CommonSerializer.cs";

        public override void CSGenerateProcess(CSGenerate rGenerate)
        {
            StartGenerateCommon();
            foreach (var rType in SerializerBinaryTypes.Types)
            {
                var rText = new StringBuilder();

                rText
                    .A("using System.IO;").N()
                    .A("using Core;").N()
                    .A("using Core.Serializer;").N()
                    .A("using Game.Serializer;").N()
                    .L(2)
                    .A("/// <summary>").N()
                    .A("/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成").N()
                    .A("/// </summary>").N();
                if (!string.IsNullOrEmpty(rType.Namespace))
                {
                    rText
                        .F("namespace {0}", rType.Namespace).N()
                        .A("{").N();
                }
                rText
                    .F("public partial class {0}", rType.Name).N()
                    .A("{").N();
                
                var rSerializeMemberInfo = SearchSerializeMember(rType);

                // Serialize Function
                rText
                    .T(1).A("public override void Serialize(BinaryWriter rWriter)").N()
                    .T(1).A("{").N()
                        .T(2).A("base.Serialize(rWriter);").N();
                foreach(var rMemberInfo in rSerializeMemberInfo)
                {
                    var rParamText = GenerateClassMemberDummyText(rMemberInfo);
                    if (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), true) && !IsBaseType(GetMemberType(rMemberInfo), false))
                        rText.T(2).F("rWriter.SerializeDynamic({0});", rParamText).N();
                    else
                        rText.T(2).F("rWriter.Serialize({0});", rParamText).N();
                }

                rText
                    .T(1).A("}").N();

                // Deserialize Function
                rText
                    .T(1).A("public override void Deserialize(BinaryReader rReader)").N()
                    .T(1).A("{").N()
                        .T(2).A("base.Deserialize(rReader);").N();
                foreach(var rMemberInfo in rSerializeMemberInfo)
                {
                    if (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), false) && !IsBaseType(GetMemberType(rMemberInfo), false))
                    {
                        rText.T(2).F("this.{0} = {1}rReader.DeserializeDynamic({2});", rMemberInfo.Name,
                            GenerateClassMemberTypeText(rMemberInfo),
                            GenerateClassMemberDummyText(rMemberInfo)).N();
                    }
                    else
                    {
                        rText.T(2).F("this.{0} = {1}rReader.Deserialize({2});", rMemberInfo.Name,
                            GenerateClassMemberTypeText(rMemberInfo),
                            GenerateClassMemberDummyText(rMemberInfo)).N();
                    }
                }

                rText
                    .T(1).A("}").N();

                rText.A("}").N();
                if (!string.IsNullOrEmpty(rType.Namespace))
                    rText.A("}").N();

                var rGroupName = string.Empty;
                var rAttributes = rType.GetCustomAttributes<SBGroupAttribute>(true);
                if (rAttributes.Length > 0)
                    rGroupName = rAttributes[0].GroupName;

                rGenerate.Add(rText.ToString(), UtilTool.PathCombine(GeneratePath, rGroupName, rType.FullName + ".Binary.cs"));

                foreach(var rMemberInfo in rSerializeMemberInfo)
                {
                    var bDynamic = rMemberInfo.IsDefined(typeof(SBDynamicAttribute), false);
                    if (rMemberInfo.MemberType == MemberTypes.Field)
                        AnalyzeGenerateCommon((rMemberInfo as FieldInfo).FieldType, bDynamic);
                    else if (rMemberInfo.MemberType == MemberTypes.Property)
                        AnalyzeGenerateCommon((rMemberInfo as PropertyInfo).PropertyType, bDynamic);
                }
            }
            EndGenerateCommon();

            rGenerate.AddHead(mCommonSerializer.ToString(), CommonSerializerPath);
        }
        string GenerateClassMemberDummyText(MemberInfo rMemberInfo)
        {
            if(rMemberInfo.MemberType == MemberTypes.Field)
            {
                return (rMemberInfo as FieldInfo).FieldType.IsEnum ?
                    string.Format("(int)this.{0}", rMemberInfo.Name) :
                    string.Format("this.{0}", rMemberInfo.Name);
            }
            else if(rMemberInfo.MemberType == MemberTypes.Property)
            {
                return (rMemberInfo as PropertyInfo).PropertyType.IsEnum ?
                    string.Format("(int)this.{0}", rMemberInfo.Name) :
                    string.Format("this.{0}", rMemberInfo.Name);
            }

            return string.Empty;
        }
        Type GetMemberType(MemberInfo rMemberInfo)
        {
            if(rMemberInfo.MemberType == MemberTypes.Field)
                return (rMemberInfo as FieldInfo).FieldType;
            else if(rMemberInfo.MemberType == MemberTypes.Property)
                return (rMemberInfo as PropertyInfo).PropertyType;

            return null;
        }
        string GenerateClassMemberTypeText(MemberInfo rMemberInfo)
        {
            var rMemberType = GetMemberType(rMemberInfo);
            if (null != rMemberType)
                return rMemberType.IsEnum ? string.Format("({0})", rMemberType.FullName) : string.Empty;

            return string.Empty;
        }
        List<MemberInfo> SearchSerializeMember(Type rType)
        {
            var rMemberInfos = new List<MemberInfo>();
            foreach(var rMemberInfo in rType.GetMembers())
            {
                if((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    rMemberInfo.DeclaringType != rType)
                    continue;

                if(rMemberInfo.IsDefined(typeof(SBIgnoreAttribute), false))
                    continue;

                if(rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    Debug.LogFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }
            foreach(var rMemberInfo in rType.GetMembers(BindingFlags.NonPublic|BindingFlags.Instance))
            {
                if((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    rMemberInfo.DeclaringType != rType)
                    continue;

                if(!rMemberInfo.IsDefined(typeof(SBEnableAttribute), false))
                    continue;

                if(rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    Debug.LogFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }
            return rMemberInfos;
        }
        void AnalyzeGenerateCommon(Type rType, bool bDynamic)
        {
            if (IsBaseType(rType))
                return;

            if(rType.IsArray)
            {
                WriteArray(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetElementType(), bDynamic);
            }
            else if (rType.GetInterface("System.Collections.IList") != null)
            {
                WriteList(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic);
            }
            else if(rType.GetInterface("System.Collections.IDictionary") != null || rType.GetInterface("Core.IDict") != null)
            {
                WriteDictionary(rType, bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic);
                AnalyzeGenerateCommon(rType.GetGenericArguments()[1], bDynamic);
            }
        }
        bool IsBaseType(Type rType, bool bIncludeSB = true)
        {
            return 
                (rType == typeof(char)) || (rType == typeof(byte))   || (rType == typeof(sbyte)) ||
                (rType == typeof(short) || (rType == typeof(ushort)) || (rType == typeof(int)) ||
                (rType == typeof(uint)) || (rType == typeof(long))   || (rType == typeof(ulong)) || 
                (rType == typeof(float))|| (rType == typeof(double)) || (rType == typeof(decimal)) ||
                (rType == typeof(string)) || (bIncludeSB && typeof(SerializerBinary).IsAssignableFrom(rType)));
        }
        void StartGenerateCommon()
        {
            mCommonSerializer
                .A("using System.IO;").N()
                .A("using System.Collections.Generic;").N()
                .A("using Core;").N()
                .A("using Core.Serializer;").N()
                .L(2)
                .A("/// <summary>").N()
                .A("/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成").N()
                .A("/// </summary>").N()
                .A("namespace Game.Serializer").N()
                .A("{").N()
                .T(1).A("public static class CommonSerializer").N()
                .T(1).A("{").N();
        }
        void WriteList(Type rType, bool bDynamic)
        {
            if (ReceiveGeneratedListType(rType, bDynamic))
                return;

            var rTypeName    = GetTypeName(rType);
            var rElementType = rType.GetGenericArguments()[0];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;
            // Serialize
            mCommonSerializer
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return;").N()
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Count; ++ nIndex)").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDEText, rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]").N()
                .T(2).A("}").N();

            // Deserialize
            mCommonSerializer
                .T(2).F("public static {1} Deserialize{0}(this BinaryReader rReader, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return null;").N()
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0}(nCount);", GetTypeName(rType)).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; ++ nIndex)").N()
                        .T(4).F("rResult.Add({0}rReader.Deserialize{1}({2}));", rElementType.IsEnum ?
                            string.Format("({0})", rElementType.FullName) : string.Empty, rTDEText,
                            GetDeserializeUnwrap(rType.GetGenericArguments()[0])).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N();
        }
        void WriteArray(Type rType, bool bDynamic)
        {
            if (ReceiveGeneratedArrayType(rType, bDynamic))
                return;

            var rTypeName    = GetTypeName(rType);
            var rElementType = rType.GetElementType();

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            // Serialize
            mCommonSerializer
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return;").N()
                    .T(3).A("rWriter.Serialize(value.Length);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Length; ++ nIndex)").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDEText, rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]").N()
                .T(2).A("}").N();

            // Deserialize
            mCommonSerializer
                .T(2).F("public static {1} Deserialize{0}(this BinaryReader rReader, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return null;").N()
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0};", rTypeName.Insert(rTypeName.IndexOf('[') + 1, "nCount")).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; ++ nIndex)").N()
                        .T(4).F("rResult[nIndex] = {0}rReader.Deserialize{1}({2});", rElementType.IsEnum ?
                            string.Format("({0})", rElementType.FullName) : string.Empty, rTDEText,
                            GetDeserializeUnwrap(rElementType)).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N();
        }
        void WriteDictionary(Type rType, bool bDynamic)
        {
            if (ReceiveGeneratedDictionaryType(rType, bDynamic))
                return;

            var rKeyType   = rType.GetGenericArguments()[0];
            var rValueType = rType.GetGenericArguments()[1];

            var rTDText  = bDynamic ? "Dynamic" : string.Empty;
            var rTDKText = bDynamic && !IsBaseType(rKeyType, false) ? "Dynamic" : string.Empty;
            var rTDVText = bDynamic && !IsBaseType(rValueType, false) ? "Dynamic" : string.Empty;
            // Serialize
            mCommonSerializer
                .T(2).F("public static void Serialize{0}(this BinaryWriter rWriter, {1} value)", rTDText, GetTypeName(rType)).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return;").N()
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("foreach(var rPair in value)").N()
                    .T(3).A("{").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDKText, rKeyType.IsEnum   ? "(int)rPair.Key"   : "rPair.Key").N()
                        .T(4).F("rWriter.Serialize{0}({1});", rTDVText, rValueType.IsEnum ? "(int)rPair.Value" : "rPair.Value").N()
                    .T(3).A("}").N()
                .T(2).A("}").N();

            // Deserialize
            mCommonSerializer
                .T(2).F("public static {1} Deserialize{0}(this BinaryReader rReader, {1} value)", rTDText, GetTypeName(rType)).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid)").N()
                        .T(4).A("return null;").N()
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0}();", GetTypeName(rType)).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; ++ nIndex)").N()
                    .T(3).A("{").N()
                        .T(4).F("var rKey   = {0}rReader.Deserialize{1}({2});", rKeyType.IsEnum ? string.Format("({0})", rKeyType.FullName) : string.Empty, 
                            rTDKText, GetDeserializeUnwrap(rKeyType)).N()
                        .T(4).F("var rValue = {0}rReader.Deserialize{1}({2});", rValueType.IsEnum ? string.Format("({0})", rValueType.FullName) : string.Empty, 
                            rTDVText, GetDeserializeUnwrap(rValueType)).N()
                        .T(4).A("rResult.Add(rKey, rValue);").N()
                    .T(3).A("}").N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N();
        }
        string GetTypeName(Type rType)
        {
            if(rType == typeof(char))           return "char";
            else if(rType == typeof(byte))      return "byte";
            else if(rType == typeof(sbyte))     return "sbyte";
            else if(rType == typeof(short))     return "short";
            else if(rType == typeof(ushort))    return "ushort";
            else if(rType == typeof(int))       return "int";
            else if(rType == typeof(uint))      return "uint";
            else if(rType == typeof(long))      return "long";
            else if(rType == typeof(ulong))     return "ulong";
            else if(rType == typeof(float))     return "float";
            else if(rType == typeof(double))    return "double";
            else if(rType == typeof(decimal))   return "decimal";
            else if(rType == typeof(string))    return "string";
            else if(rType.IsArray)
            {
                return string.Format("{0}[]", GetTypeName(rType.GetElementType()));
            }
            else if(rType.GetInterface("System.Collections.IList") != null)
            {
                return string.Format("List<{0}>", GetTypeName(rType.GetGenericArguments()[0]));
            }
            else if (rType.GetInterface("System.Collections.IDictionary") != null)
            {
                return string.Format("Dictionary<{0}, {1}>", 
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.GetInterface("Core.IDict") != null)
            {
                return string.Format("Dict<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else
            {
                return rType.FullName;
            }
        }
        string GetDeserializeUnwrap(Type rType)
        {
            if (rType == typeof(char))          return "default(char)";
            else if (rType == typeof(byte))     return "default(byte)";
            else if (rType == typeof(sbyte))    return "default(sbyte)";
            else if (rType == typeof(short))    return "default(short)";
            else if (rType == typeof(ushort))   return "default(ushort0";
            else if (rType == typeof(int))      return "default(int)";
            else if (rType == typeof(uint))     return "default(uint)";
            else if (rType == typeof(long))     return "default(long)";
            else if (rType == typeof(ulong))    return "default(ulong)";
            else if (rType == typeof(float))    return "default(float)";
            else if (rType == typeof(double))   return "default(double)";
            else if (rType == typeof(decimal))  return "default(decimal)";
            else if (rType == typeof(string))   return "string.Empty";
            else if (rType.IsEnum)
            {
                return string.Format("int.MaxValue");
            }
            else
            {
                return string.Format("default({0})", GetTypeName(rType));
            }
        }
        void EndGenerateCommon()
        {
            mCommonSerializer
                .T(1).A("}").N()
            .A("}");
        }

        bool ReceiveGeneratedArrayType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedArray, mGeneratedDynamicArray, rType, bDynamic);
        }
        bool ReceiveGeneratedListType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedList, mGeneratedDynamicList, rType, bDynamic);
        }
        bool ReceiveGeneratedDictionaryType(Type rType, bool bDynamic)
        {
            return ReceiveType(mGeneratedDictionary, mGeneratedDynamicDictionary, rType, bDynamic);
        }
        bool ReceiveType(List<Type> rGenerated, List<Type> rGeneratedDynamic, Type rType, bool bDynamic)
        {
            if(bDynamic)
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

        StringBuilder   mCommonSerializer           = new StringBuilder();
        List<Type>      mGeneratedArray             = new List<Type>();
        List<Type>      mGeneratedDynamicArray      = new List<Type>();
        List<Type>      mGeneratedList              = new List<Type>();
        List<Type>      mGeneratedDynamicList       = new List<Type>();
        List<Type>      mGeneratedDictionary        = new List<Type>();
        List<Type>      mGeneratedDynamicDictionary = new List<Type>();
    }
}
