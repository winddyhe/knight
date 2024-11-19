using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Knight.Core;

namespace Knight.Framework.Serializer.Editor
{
    using TypeMap = Dictionary<Type, HashSet<Type>>;
    public class CodeGenerator_CommonSerializer : SerializerCodeGenerator
    {
        private TypeMap mGeneratedArray = new TypeMap();
        private TypeMap mGeneratedDynamicArray = new TypeMap();
        private TypeMap mGeneratedList = new TypeMap();
        private TypeMap mGeneratedDynamicList = new TypeMap();
        private TypeMap mGeneratedDictionary = new TypeMap();
        private TypeMap mGeneratedDynamicDictionary = new TypeMap();
        public string NameSpace = string.Empty;
        public string ClassName;

        public CodeGenerator_CommonSerializer(string rFilePath, string rNameSpace, string rClassName)
            : base(rFilePath)
        {
            this.NameSpace = rNameSpace;
            this.ClassName = rClassName;
        }

        public void WriteAndSave()
        {
            this.WriteHead();
            this.WriteBody();
            this.WriteEnd();
            this.Save();
        }

        public override void WriteHead()
        {
            this.StringBuilder?
                .A("using System.Collections.Generic;").N()
                .A("using System.IO;").N()
                .A("using Knight.Core;").N()
                .A("using Knight.Framework.Serializer;").N();

            this.WriteReference();

            this.StringBuilder?
                .L(1)
                .A("/// <summary>").N()
                .A("/// Auto generate code, not need modify.").N()
                .A("/// </summary>").N()
                .A($"namespace {this.NameSpace}").N()
                .A("{").N()
                .T(1).A("public static partial class ").Append(this.ClassName).N()
                .T(1).A("{").N();
        }

        public override void WriteEnd()
        {
            this.StringBuilder?
                .T(1).A("}").N()
                .A("}");
        }

        public void AnalyzeGenerateCommon(Type rType, bool bDynamic, Type rClassType)
        {
            if (SerializerAssists.IsBaseType(rType)) return;

            if (rType.IsArray)
            {
                this.ReceiveGeneratedArrayType(rType, bDynamic, rClassType);
                this.AnalyzeGenerateCommon(rType.GetElementType(), bDynamic, rClassType);
            }
            else if (rType.GetInterface("System.Collections.IList") != null)
            {
                this.ReceiveGeneratedListType(rType, bDynamic, rClassType);
                this.AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic, rClassType);
            }
            else if (rType.GetInterface("System.Collections.IDictionary") != null || rType.GetInterface("Knight.Core.IDict") != null)
            {
                this.ReceiveGeneratedDictionaryType(rType, bDynamic, rClassType);
                this.AnalyzeGenerateCommon(rType.GetGenericArguments()[0], bDynamic, rClassType);
                this.AnalyzeGenerateCommon(rType.GetGenericArguments()[1], bDynamic, rClassType);
            }
        }

        public void WriteBody()
        {
            this.WriteBody(this.mGeneratedArray, false, this.ClassName, WriteArray);
            this.WriteBody(this.mGeneratedDynamicArray, true, this.ClassName, WriteArray);
            this.WriteBody(this.mGeneratedList, false, this.ClassName, WriteList);
            this.WriteBody(this.mGeneratedDynamicList, true, this.ClassName, WriteList);
            this.WriteBody(this.mGeneratedDictionary, false, this.ClassName, WriteDictionary);
            this.WriteBody(this.mGeneratedDynamicDictionary, true, this.ClassName, WriteDictionary);
        }

        void WriteBody(TypeMap rMap, bool bDynamic, string rCommonSerializer, Action<StringBuilder, Type, bool, string> rWriteAction)
        {
            foreach (KeyValuePair<Type, HashSet<Type>> rPair in rMap)
            {
                if (rPair.Value.Count > 1)
                    rWriteAction(this.StringBuilder, rPair.Key, bDynamic, rCommonSerializer);
            }
        }

        void WriteReference()
        {
            List<string> rPrivateList = new List<string>();

            rPrivateList.Sort();

            this.StringBuilder.A("/*").
                A("class type references, do not modify").N().
                A(this.GetReferenceStr()).
                A("*/").N();
        }

        string GetReferenceStr()
        {
            StringBuilder rSB = new StringBuilder();
            rSB.
                A("common ref:").N().
                A(this.GetCommonReferenceStr()).
                A("private ref:").N().
                A(this.GetPrivateReferenceStr());
            return rSB.ToString();
        }

        string GetCommonReferenceStr()
        {
            List<string> rList = new List<string>();
            this.GetCommonReference(this.mGeneratedArray, rList);
            this.GetCommonReference(this.mGeneratedDynamicArray, rList);
            this.GetCommonReference(this.mGeneratedList, rList);
            this.GetCommonReference(this.mGeneratedDynamicList, rList);
            this.GetCommonReference(this.mGeneratedDictionary, rList);
            this.GetCommonReference(this.mGeneratedDynamicDictionary, rList);
            rList.Sort();
            StringBuilder rSB = new StringBuilder();
            for (int i = 0; i < rList.Count; ++i)
                rSB.A(rList[i]).N();
            return rSB.ToString();
        }

        void GetCommonReference(TypeMap rMap, List<string> rList)
        {
            StringBuilder rSB;
            foreach (KeyValuePair<Type, HashSet<Type>> rPair in rMap)
            {
                if (rPair.Value.Count >= 2)
                {
                    rSB = new StringBuilder();
                    rSB.A(rPair.Key.ToString()).A(":");
                    foreach (Type rClass in rPair.Value)
                        rSB.A(rClass.ToString()).A(",");
                    rList.Add(rSB.ToString());
                }
            }
        }

        string GetPrivateReferenceStr()
        {
            List<string> rList = new List<string>();
            this.GetPrivateReference(this.mGeneratedArray, rList);
            this.GetPrivateReference(this.mGeneratedDynamicArray, rList);
            this.GetPrivateReference(this.mGeneratedList, rList);
            this.GetPrivateReference(this.mGeneratedDynamicList, rList);
            this.GetPrivateReference(this.mGeneratedDictionary, rList);
            this.GetPrivateReference(this.mGeneratedDynamicDictionary, rList);
            rList.Sort();
            StringBuilder rSB = new StringBuilder();
            for (int i = 0; i < rList.Count; ++i)
                rSB.A(rList[i]).N();
            return rSB.ToString();
        }

        void GetPrivateReference(TypeMap rMap, List<string> rList)
        {
            StringBuilder rSB;
            foreach (KeyValuePair<Type, HashSet<Type>> rPair in rMap)
            {
                if (rPair.Value.Count <= 1)
                {
                    rSB = new StringBuilder();
                    rSB.A(rPair.Key.ToString()).A(":");
                    foreach (Type rClass in rPair.Value)
                        rSB.A(rClass.ToString());
                    rList.Add(rSB.ToString());
                }
            }
        }

        public static void WriteArray(StringBuilder rSB, Type rType, bool bDynamic, string rCommonSerializer)
        {
            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetElementType();

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !SerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            rSB?
                .T(2).F("public static void Serialize{0}(BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Length);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Length; nIndex++)").N()
                        .T(4).A(GetTypeSerializeCode(rCommonSerializer, rTDEText, rElementType)).N()
                .T(2).A("}").N()
                .L(1);

            rSB?
                .T(2).F("public static {0} Deserialize(BinaryReader rReader, {1} value)", rTypeName, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0};", rTypeName.Insert(rTypeName.IndexOf('[') + 1, "nCount")).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; nIndex++)").N()
                        .T(4).A(GetArrayElementDeserializeCode(rCommonSerializer, rTDEText, rElementType)).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }

        public static void WriteList(StringBuilder rSB, Type rType, bool bDynamic, string rCommonSerializer)
        {
            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rElementType = rType.GetGenericArguments()[0];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDEText = bDynamic && !SerializerAssists.IsBaseType(rElementType, false) ? "Dynamic" : string.Empty;

            rSB?
                .T(2).F("public static void Serialize{0}(BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("for (int nIndex = 0; nIndex < value.Count; ++ nIndex)").N()
                        .T(4).A(GetTypeSerializeCode(rCommonSerializer, rTDEText, rElementType)).N()
                .T(2).A("}").N()
                .L(1);

            rSB?
                .T(2).F("public static {0} Deserialize{1}(BinaryReader rReader, {2} value)", rTypeName, rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).A("var nCount  = rReader.Deserialize(default(int));").N()
                    .T(3).F("var rResult = new {0}(nCount);", rTypeName).N()
                    .T(3).A("for (int nIndex = 0; nIndex < nCount; nIndex++)").N()
                        .T(4).A(GetListElementDeserializeCode(rCommonSerializer, rTDEText, rElementType)).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }

        public static void WriteDictionary(StringBuilder rSB, Type rType, bool bDynamic, string rCommonSerializer)
        {
            var rTypeName = SerializerAssists.GetTypeName(rType);
            var rKeyType = rType.GetGenericArguments()[0];
            var rValueType = rType.GetGenericArguments()[1];

            var rTDText = bDynamic ? "Dynamic" : string.Empty;
            var rTDKText = bDynamic && !SerializerAssists.IsBaseType(rKeyType, false) ? "Dynamic" : string.Empty;
            var rTDVText = bDynamic && !SerializerAssists.IsBaseType(rValueType, false) ? "Dynamic" : string.Empty;

            rSB?
                .T(2).F("public static void Serialize{0}(BinaryWriter rWriter, {1} value)", rTDText, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = (null != value);").N()
                    .T(3).A("rWriter.Serialize(bValid);").N()
                    .T(3).A("if (!bValid) return;").N()
                    .L(1)
                    .T(3).A("rWriter.Serialize(value.Count);").N()
                    .T(3).A("foreach(var rPair in value)").N()
                    .T(3).A("{").N()
                        .T(4).A(GetDictionayValueSerializeCode(rCommonSerializer, rKeyType, rTDKText, "rPair.Key")).N()
                        .T(4).A(GetDictionayValueSerializeCode(rCommonSerializer, rValueType, rTDVText, "rPair.Value")).N()
                    .T(3).A("}").N()
                .T(2).A("}").N()
                .L(1);

            rSB?
                .T(2).F("public static {0} Deserialize{1}(BinaryReader rReader, {2} value)", rTypeName, rTDText, rTypeName).N()
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
                         .T(4).A(GetDictionayValueDeserializeCode(rCommonSerializer, rValueType, rTDVText)).N()
                        .T(4).A("rResult.Add(rKey, rValue);").N()
                    .T(3).A("}").N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }

        private void ReceiveGeneratedArrayType(Type rType, bool bDynamic, Type rClassType)
        {
            if (bDynamic)
                this.ReceiveType(this.mGeneratedDynamicArray, rType, rClassType);
            else
                this.ReceiveType(this.mGeneratedArray, rType, rClassType);
        }

        private void ReceiveGeneratedListType(Type rType, bool bDynamic, Type rClassType)
        {
            if (bDynamic)
                this.ReceiveType(this.mGeneratedDynamicList, rType, rClassType);
            else
                this.ReceiveType(this.mGeneratedList, rType, rClassType);
        }

        private void ReceiveGeneratedDictionaryType(Type rType, bool bDynamic, Type rClassType)
        {
            if (bDynamic)
                this.ReceiveType(this.mGeneratedDynamicDictionary, rType, rClassType);
            else
                this.ReceiveType(this.mGeneratedDictionary, rType, rClassType);
        }

        private void ReceiveType(TypeMap rMap, Type rType, Type classType)
        {
            if (!rMap.ContainsKey(rType))
                rMap.Add(rType, new HashSet<Type>());
            if (!rMap[rType].Contains(classType))
                rMap[rType].Add(classType);
        }

        public HashSet<Type> GetRefArrayType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedArray);
        }
        public HashSet<Type> GetRefDynamicArrayType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedDynamicArray);
        }
        public HashSet<Type> GetRefListType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedList);
        }
        public HashSet<Type> GetRefDynamicListType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedDynamicList);
        }
        public HashSet<Type> GetRefDictionaryType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedDictionary);
        }
        public HashSet<Type> GetRefDynamicDictionaryType(Type rClassType)
        {
            return this.GetRefType(rClassType, this.mGeneratedDynamicDictionary);
        }

        public HashSet<Type> GetRefType(Type rClassType, TypeMap rMap)
        {
            HashSet<Type> rRetSet = new HashSet<Type>();
            foreach (KeyValuePair<Type, HashSet<Type>> rPair in rMap)
            {
                if (rPair.Value.Count == 1 && rPair.Value.Contains(rClassType))
                    rRetSet.Add(rPair.Key);
            }
            return rRetSet;
        }

        // 检测是否有修改变更
        public bool IsModified()
        {
            string rLastRefStr = this.GetLastReferenceStr();
            string rRefStr = this.GetReferenceStr();
            return rRefStr != rLastRefStr;
        }

        private string GetLastReferenceStr()
        {
            if (!File.Exists(this.FilePath))
                return "";
            string rFileStr = File.ReadAllText(this.FilePath);
            int nIndex1 = rFileStr.IndexOf("common ref:");
            if (nIndex1 < 0)
                return "";
            int nIndex2 = rFileStr.IndexOf("*/", nIndex1);
            if (nIndex2 < 0)
                return "";
            return rFileStr.Substring(nIndex1, nIndex2 - nIndex1);
        }

        // 获取类修改列表
        public HashSet<string> GetModifyList()
        {
            HashSet<string> rModifySet = new HashSet<string>();
            string rRefStr = this.GetReferenceStr();
            Dictionary<string, HashSet<string>> rCommonRefList = new Dictionary<string, HashSet<string>>();
            Dictionary<string, string> rPrivateRefList = new Dictionary<string, string>();
            this.AnalyseReference(rRefStr, rCommonRefList, rPrivateRefList);

            string rLastRefStr = this.GetLastReferenceStr();
            Dictionary<string, HashSet<string>> rLastCommonRefList = new Dictionary<string, HashSet<string>>();
            Dictionary<string, string> rLastPrivateRefList = new Dictionary<string, string>();
            this.AnalyseReference(rLastRefStr, rLastCommonRefList, rLastPrivateRefList);

            // 检测相同公共引用
            List<string> rSameCommonRef = new List<string>();
            foreach (KeyValuePair<string, HashSet<string>> rPair in rCommonRefList)
            {
                if (rLastCommonRefList.ContainsKey(rPair.Key))
                {
                    if (this.SetCompare(rPair.Value, rLastCommonRefList[rPair.Key]))
                        rSameCommonRef.Add(rPair.Key);
                }
            }

            // 移除相同公共引用
            for (int i = 0; i < rSameCommonRef.Count; ++i)
            {
                rCommonRefList.Remove(rSameCommonRef[i]);
                rLastCommonRefList.Remove(rSameCommonRef[i]);
            }

            // 统计公共引用差异
            foreach (KeyValuePair<string, HashSet<string>> rPair in rCommonRefList)
                foreach (string rStr in rPair.Value)
                    this.AddToSet(rModifySet, rStr);
            foreach (KeyValuePair<string, HashSet<string>> rPair in rLastCommonRefList)
                foreach (string rStr in rPair.Value)
                    this.AddToSet(rModifySet, rStr);


            // 检测相同私有引用
            List<string> rSamePrivateRef = new List<string>();
            foreach (KeyValuePair<string, string> rPair in rPrivateRefList)
            {
                if (rLastPrivateRefList.ContainsKey(rPair.Key))
                {
                    if (rPair.Value == rLastPrivateRefList[rPair.Key])
                        rSamePrivateRef.Add(rPair.Key);
                }
            }

            // 移除相同私有引用
            for (int i = 0; i < rSamePrivateRef.Count; ++i)
            {
                rPrivateRefList.Remove(rSamePrivateRef[i]);
                rLastPrivateRefList.Remove(rSamePrivateRef[i]);
            }

            // 统计私有引用差异
            foreach (KeyValuePair<string, string> rPair in rPrivateRefList)
                this.AddToSet(rModifySet, rPair.Value);
            foreach (KeyValuePair<string, string> rPair in rLastPrivateRefList)
                this.AddToSet(rModifySet, rPair.Value);

            return rModifySet;
        }

        private void AnalyseReference(string rRefStr, Dictionary<string, HashSet<string>> commonRefList, Dictionary<string, string> privateRefList)
        {
            StringReader rSR = new StringReader(rRefStr);
            bool bReadingCommon = true;
            string rLine = rSR.ReadLine();
            while(!string.IsNullOrEmpty(rLine))
            {
                if (rLine == "common ref:")
                    bReadingCommon = true;
                else if (rLine == "private ref:")
                    bReadingCommon = false;
                else
                {
                    if (bReadingCommon)
                    {
                        string[] rStr1 = rLine.Split(':');
                        if (rStr1.Length != 2)
                        {
                            UnityEngine.Debug.LogError("引用解析错误" + this.FilePath);
                            return;
                        }
                        string[] rStr2 = rStr1[1].Split(',');
                        HashSet<string> rSet = new HashSet<string>();
                        for (int i = 0; i < rStr2.Length; ++i)
                        {
                            if (!string.IsNullOrEmpty(rStr2[i]))
                                rSet.Add(rStr2[i]);
                        }
                        commonRefList.Add(rStr1[0], rSet);
                    }
                    else
                    {
                        string[] rStr1 = rLine.Split(':');
                        if (rStr1.Length != 2)
                        {
                            UnityEngine.Debug.LogError("引用解析错误" + this.FilePath);
                            return;
                        }
                        privateRefList.Add(rStr1[0], rStr1[1]);
                    }
                }
                rLine = rSR.ReadLine();
            }
        }

        private bool SetCompare(HashSet<string> rSet1, HashSet<string> rSet2)
        {
            if (rSet1.Count != rSet2.Count)
                return false;

            foreach (string rStr in rSet1)
            {
                if (!rSet2.Contains(rStr))
                    return false;
            }
            return true;
        }

        private void AddToSet(HashSet<string> rSet, string rValue)
        {
            if (!rSet.Contains(rValue))
                rSet.Add(rValue);
        }

        private static string GetTypeSerializeCode(string rCommonSerializer, string rTDEText, Type rElementType)
        {
            string rTypeChange = (rElementType.IsEnum ? "(int)value[nIndex]" : "value[nIndex]");
            if (IsSimpleType(rElementType))
                return string.Format("rWriter.Serialize{0}({1});", rTDEText, rTypeChange);
            else
                return string.Format("{0}.Serialize{1}(rWriter, {2});", rCommonSerializer, rTDEText, rTypeChange);
        }

        private static string GetArrayElementDeserializeCode(string rCommonSerializer, string rTDEText, Type rElementType)
        {
            string rTypeChange = (rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty);
            object rDeserializeUnwrap = SerializerAssists.GetDeserializeUnwrap(rElementType);
            if (IsSimpleType(rElementType, false))
                return string.Format("rResult[nIndex] = {0}rReader.Deserialize{1}({2});", rTypeChange, rTDEText, rDeserializeUnwrap);
            else
                return string.Format("rResult[nIndex] = {0}{1}.Deserialize{2}(rReader, {3});", rTypeChange, rCommonSerializer, rTDEText, rDeserializeUnwrap);
        }
        private static string GetListElementDeserializeCode(string rCommonSerializer, string rTDEText, Type rElementType)
        {
            string rTypeChange = (rElementType.IsEnum ? string.Format("({0})", rElementType.FullName) : string.Empty);
            object rDeserializeUnwrap = SerializerAssists.GetDeserializeUnwrap(rElementType);
            if (IsSimpleType(rElementType, false))
                return string.Format("rResult.Add({0}rReader.Deserialize{1}({2}));", rTypeChange, rTDEText, rDeserializeUnwrap);
            else
                return string.Format("rResult.Add({0}{1}.Deserialize{2}(rReader, {3}));", rTypeChange, rCommonSerializer, rTDEText, rDeserializeUnwrap);
        }

        private static string GetDictionayValueSerializeCode(string rCommonSerializer, Type rValueType, string rTDVText, string rValueName)
        {
            string rValueIfEnumCast = rValueType.IsEnum ? "(int)" + rValueName : rValueName;
            if (IsSimpleType(rValueType, false) || rValueType.IsDefined(typeof(SerializerBinaryAttribute), false))
                return string.Format("rWriter.Serialize{0}({1});", rTDVText, rValueIfEnumCast);
            else
                return string.Format("CommonSerializer.Serialize(rWriter, {0});", rValueName);
        }

        private static string GetDictionayValueDeserializeCode(string rCommonSerializer, Type rValueType, string rTDVText)
        {
            string rEnum = (rValueType.IsEnum ? string.Format("({0})", rValueType.FullName) : string.Empty);
            object rDeserializeUnwrap = SerializerAssists.GetDeserializeUnwrap(rValueType);
            if (IsSimpleType(rValueType, false))
                return string.Format("var rValue = {0}rReader.Deserialize{1}({2});", rEnum, rTDVText, rDeserializeUnwrap);
            else
                return string.Format("var rValue = {0}{1}.Deserialize{2}(rReader, {3});", rEnum, rCommonSerializer, rTDVText, rDeserializeUnwrap);
        }
    }
}
