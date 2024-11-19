using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Knight.Core;
namespace Knight.Framework.Serializer.Editor
{
    public class CodeGenerator_ClassSerializer : SerializerCodeGenerator
    {
        // 仅此类使用的公共解析类型记录
        private HashSet<Type> mCommonArray;
        private HashSet<Type> mCommonDynamicArray;
        private HashSet<Type> mCommonList;
        private HashSet<Type> mCommonDynamicList;
        private HashSet<Type> mCommonDictionary;
        private HashSet<Type> mCommonDynamicDictionary;
        private HashSet<Type> mCommonSerializedBinary;
        private string mCommonSerializerNameSpace;
        private string mCommonNameSpace = string.Empty;
        private string mTypeMD5;     // 类型成员MD5
        private string mLastMD5;     // 上次生成文件时保存的MD5
        private string mCommonSerializerName;

        public CodeGenerator_ClassSerializer(string rFilePath, string rNameSpace, Type rClassType, string rCommonSerializerName)
            : base(rFilePath)
        {
            this.mCommonSerializerName = rCommonSerializerName;
            this.mCommonNameSpace = rNameSpace;
            this.ClassType = rClassType;
            this.mTypeMD5 = this.GetTypeMD5(rClassType);
            this.mLastMD5 = this.GetLastMD5(rFilePath);
        }

        public void WriteSaveWithCommonSerializer(CodeGenerator_CommonSerializer rCommonSerializer)
        {
            this.mCommonSerializerNameSpace = rCommonSerializer.NameSpace;
            this.mCommonArray = rCommonSerializer.GetRefArrayType(this.ClassType);
            this.mCommonDynamicArray = rCommonSerializer.GetRefDynamicArrayType(this.ClassType);
            this.mCommonList = rCommonSerializer.GetRefListType(this.ClassType);
            this.mCommonDynamicList = rCommonSerializer.GetRefDynamicListType(this.ClassType);
            this.mCommonDictionary = rCommonSerializer.GetRefDictionaryType(this.ClassType);
            this.mCommonDynamicDictionary = rCommonSerializer.GetRefDynamicDictionaryType(this.ClassType);

            this.WriteHead();
            this.WriteClass(this.ClassType);
            this.WriteCommonSerializer();
            this.WriteEnd();
            this.Save();
        }

        public override void WriteHead()
        {
            if (this.mCommonList.Count > 0 || this.mCommonDynamicList.Count > 0
                || this.mCommonDictionary.Count > 0 || this.mCommonDynamicDictionary.Count > 0)
            {
                this.StringBuilder?
                    .A("using System.Collections.Generic;").N();
            }
            this.StringBuilder?
                .A("using System.IO;").N()
                .A("using Knight.Core;").N()
                .A("using Knight.Framework.Serializer;").N()
                .A($"using {this.mCommonNameSpace};").N()
                .L(1)
                .A("//ScriptMD5:").A(this.mTypeMD5).N()
                .L(1)
                .A("/// <summary>").N()
                .A("/// Auto generate code, not need modify.").N()
                .A("/// </summary>").N();
        }

        public override void WriteEnd()
        {
            this.Write();
        }

        public void WriteClass(Type rType)
        {
            var rClassTypeName = rType.Name;
            rClassTypeName = rClassTypeName + " : ISerializerBinary";
            if (rType.IsDefined(typeof(SBFileReadWriteAttribute), false))
            {
                rClassTypeName = rClassTypeName + ", ISBReadWriteFile";
            }
            this.StringBuilder?
                .F("namespace {0}", rType.Namespace).N()
                .A("{").N()
                .T(1).F("public partial class {0}", rClassTypeName).N()
                .T(1).A("{").N()
                    .T(2).A("public void Serialize(BinaryWriter rWriter)").N()
                    .T(2).A("{").N();

            var rAllSerializeMembers = SerializerAssists.FindSerializeMembers(rType);
            rAllSerializeMembers.Sort((a, b) => a.Name.CompareTo(b.Name));
            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                bool bDynamic = (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), true) &&
                    !SerializerAssists.IsBaseType(SerializerAssists.GetMemberType(rMemberInfo), false));
                this.WriteMemberSerializer(rMemberInfo, bDynamic);
            }
            this.StringBuilder
                    .T(2).A("}").N();

            this.StringBuilder
                    //.T(2).A("public static ").A(this.ClassType.FullName).A(" Deserialize(BinaryReader rReader)").N()
                    .T(2).A("public void Deserialize(BinaryReader rReader)").N()
                    .T(2).A("{").N();

            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];

                bool bDynamic = (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), false) &&
                    !SerializerAssists.IsBaseType(SerializerAssists.GetMemberType(rMemberInfo), false));
                this.WriteMemberDeserializer(rMemberInfo, bDynamic);
            }
            this.StringBuilder
                .T(2).A("}").N();


            if (rType.IsDefined(typeof(SBFileReadWriteAttribute), false))
            {
                this.StringBuilder
                    .T(2).A("public void Save(string rFilePath)").N()
                    .T(2).A("{").N()
                        .T(3).A("using (var fs = new FileStream(rFilePath, FileMode.Create, FileAccess.ReadWrite))").N()
                        .T(3).A("{").N()
                            .T(4).A("fs.SetLength(0);").N()
                            .T(4).A("using (var bw = new BinaryWriter(fs))").N()
                            .T(4).A("{").N();
                this.WriteSaveCode(rAllSerializeMembers);
                this.StringBuilder
                            .T(4).A("}").N()
                        .T(3).A("}").N()
                    .T(2).A("}").N();

                this.StringBuilder
                    .T(2).A("public void Read(byte[] rBytes)").N()
                    .T(2).A("{").N()
                        .T(3).A("using (var ms = new MemoryStream(rBytes))").N()
                        .T(3).A("{").N()
                            .T(4).A("using (var br = new BinaryReader(ms))").N()
                            .T(4).A("{").N();
                this.WriteReadCode(rAllSerializeMembers);
                this.StringBuilder
                            .T(4).A("}").N()
                        .T(3).A("}").N()
                    .T(2).A("}").N();
            }

            this.StringBuilder
                .T(1).A("}").N()
                .A("}").N();
        }

        // 写仅子类使用的BinaryWriter类型解析代码
        public void WriteCommonSerializer()
        {
            this.StringBuilder?
               .L(1)
               .A("/// <summary>").N()
               .A("/// ").A(this.mCommonSerializerName).A(" private ref").N()
               .A("/// </summary>").N()
               .A($"namespace {this.mCommonSerializerNameSpace}").N()
               .A("{").N()
               .T(1).A("public static partial class ").A(this.mCommonSerializerName).N()
               .T(1).A("{").N();

            this.WriteClassTypeSerializedBinary();
            foreach (Type rType in this.mCommonArray)
                CodeGenerator_CommonSerializer.WriteArray(this.StringBuilder, rType, false, this.mCommonSerializerName);
            foreach (Type rType in this.mCommonDynamicArray)
                CodeGenerator_CommonSerializer.WriteArray(this.StringBuilder, rType, true, this.mCommonSerializerName);

            foreach (Type rType in this.mCommonList)
                CodeGenerator_CommonSerializer.WriteList(this.StringBuilder, rType, false, this.mCommonSerializerName);
            foreach (Type rType in this.mCommonDynamicList)
                CodeGenerator_CommonSerializer.WriteList(this.StringBuilder, rType, true, this.mCommonSerializerName);

            foreach (Type rType in this.mCommonDictionary)
                CodeGenerator_CommonSerializer.WriteDictionary(this.StringBuilder, rType, false, this.mCommonSerializerName);
            foreach (Type rType in this.mCommonDynamicDictionary)
                CodeGenerator_CommonSerializer.WriteDictionary(this.StringBuilder, rType, true, this.mCommonSerializerName);

            this.StringBuilder?
                .T(1).A("}").N()
                .A("}");
        }

        // 根据类和序列化成员计算MD5
        private string GetTypeMD5(Type rType)
        {
            List<string> rList = new List<string>();
            var rSerializeMemberInfo = SerializerAssists.FindSerializeMembers(rType);
            foreach (MemberInfo rMemberInfo in rSerializeMemberInfo)
            {
                if (rMemberInfo.MemberType == MemberTypes.Field || rMemberInfo.MemberType == MemberTypes.Property)
                    rList.Add(rMemberInfo.ToString());
            }
            rList.Sort();
            StringBuilder rSB = new StringBuilder();
            rSB.A(rType.Namespace).N();
            rSB.A(rType.Name).N();
            for (int i = 0; i < rList.Count; ++i)
                rSB.A(rList[i]).N();
            return MD5Tool.GetStringMD5(rSB.ToString()).ToHEXString();
        }

        // 根据类和序列化成员计算MD5
        private string GetLastMD5(string rPath)
        {
            string rMD5 = "";
            if (!File.Exists(rPath))
                return rMD5;
            StreamReader rSR = new StreamReader(File.OpenRead(rPath));
            while (!rSR.EndOfStream)
            {
                string rLine = rSR.ReadLine();
                if (rLine.StartsWith("//ScriptMD5:"))
                {
                    rMD5 = rLine.Substring(12);
                    break;
                }
            }
            rSR.Close();
            return rMD5;
        }

        // 是否需要增量更新
        public bool NeedGenIcrement()
        {
            return this.mLastMD5 != this.mTypeMD5;
        }

        private void WriteMemberDeserializer(MemberInfo rMemberInfo, bool isDynamic)
        {
            string rMemberDummyText = SerializerAssists.GetClassMemberDummyText(rMemberInfo);
            var rMemberText = SerializerAssists.GetClassMemberTypeText(rMemberInfo);
            bool bSimpleType = IsSimpleType(SerializerAssists.GetMemberType(rMemberInfo), false);

            string rDynamic = isDynamic ? "Dynamic" : "";
            if (bSimpleType)
            {
                this.StringBuilder.T(3)
                    .F("this.{0} = {1}rReader.Deserialize{2}({3});", rMemberInfo.Name, rMemberText, rDynamic, rMemberDummyText)
                    .N();
            }
            else
            {
                this.StringBuilder.T(3)
                    .F("this.{0} = {1}{2}.Deserialize{3}(rReader, {4});", rMemberInfo.Name, rMemberText, this.mCommonSerializerName, rDynamic, rMemberDummyText)
                    .N();
            }
        }

        private void WriteMemberSerializer(MemberInfo rMemberInfo, bool isDynamic)
        {
            string rMemberDummyText = SerializerAssists.GetClassMemberDummyText(rMemberInfo);
            bool bSimpleType = IsSimpleType(SerializerAssists.GetMemberType(rMemberInfo));

            string rDynamic = isDynamic ? "Dynamic" : "";
            if (bSimpleType)
            {
                this.StringBuilder.T(3)
                    .F("rWriter.Serialize{0}({1});", rDynamic, rMemberDummyText)
                    .N();
            }
            else
            {
                this.StringBuilder.T(3)
                    .F("{0}.Serialize{1}(rWriter, {2});", this.mCommonSerializerName, rDynamic, rMemberDummyText)
                    .N();
            }
        }

        private void WriteSaveCode(List<MemberInfo> rAllSerializeMembers)
        {
            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                string rMemberDummyText = SerializerAssists.GetClassMemberDummyText(rMemberInfo);
                bool bSimpleType = IsSimpleType(SerializerAssists.GetMemberType(rMemberInfo));
                if (bSimpleType)
                    this.StringBuilder.T(5).A($"bw.Serialize({rMemberDummyText});").N();
                else
                    this.StringBuilder.T(5).A(this.mCommonSerializerName).A($".Serialize(bw, this.{rMemberInfo.Name});").N();
            }
        }

        private void WriteReadCode(List<MemberInfo> rAllSerializeMembers)
        {
            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                string rMemberDummyText = SerializerAssists.GetClassMemberDummyText(rMemberInfo);
                var rMemberText = SerializerAssists.GetClassMemberTypeText(rMemberInfo);
                bool bSimpleType = IsSimpleType(SerializerAssists.GetMemberType(rMemberInfo), false);
                if (bSimpleType)
                    this.StringBuilder.T(5).A($"this.{rMemberInfo.Name} = {rMemberText}br.Deserialize({rMemberDummyText});").N();
                else
                    this.StringBuilder.T(5).A("this.").A(rMemberInfo.Name).A(" = ").A(this.mCommonSerializerName).A($".Deserialize(br, this.{rMemberInfo.Name});").N();
            }
        }

        public void WriteClassTypeSerializedBinary()
        {
            var rTypeName = SerializerAssists.GetTypeName(this.ClassType);
            this.StringBuilder?
                .T(2).F("public static {0} Deserialize(BinaryReader rReader, {1} value)", rTypeName, rTypeName).N()
                .T(2).A("{").N()
                    .T(3).A("var bValid = rReader.Deserialize(default(bool));").N()
                    .T(3).A("if (!bValid) return null;").N()
                    .L(1)
                    .T(3).F("var rResult = new {0}();", rTypeName).N()
                    .T(3).F("rResult.Deserialize(rReader);", rTypeName).N()
                    .T(3).A("return rResult;").N()
                .T(2).A("}").N()
                .L(1);
        }
    }
}
