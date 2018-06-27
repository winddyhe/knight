//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using System;
using System.Collections.Generic;

namespace Knight.Hotfix.Core.Editor
{
    public class HotfixCodeGenerator_ClassSerializer : HotfixCodeGenerator
    {
        public string GroupName = string.Empty;

        public HotfixCodeGenerator_ClassSerializer(string rFilePath)
            : base(rFilePath)
        {
        }

        public override void WriteHead()
        {
            this.StringBuilder?
                .A("using System.IO;").N()
                .A("using Knight.Hotfix.Core;").N()
                .A("using Knight.Core;").N()
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
            this.StringBuilder?
                .F("namespace {0}", rType.Namespace).N()
                .A("{").N()
                .T(1).F("public partial class {0}", rType.Name).N()
                .T(1).A("{").N()
                    .T(2).A("public override void Serialize(BinaryWriter rWriter)").N()
                    .T(2).A("{").N()
                        .T(3).A("base.Serialize(rWriter);").N();

            var rAllSerializeMembers = HotfixSerializerAssists.FindSerializeMembers(rType);
            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                var rParamText = HotfixSerializerAssists.GetClassMemberDummyText(rAllSerializeMembers[i]);

                if (rMemberInfo.IsDefined(typeof(HotfixSBDynamicAttribute), true) &&
                    !HotfixSerializerAssists.IsBaseType(HotfixSerializerAssists.GetMemberType(rMemberInfo), false))
                    this.StringBuilder?
                        .T(3).F("rWriter.SerializeDynamic({0});", rParamText).N();
                else
                    this.StringBuilder?
                        .T(3).F("rWriter.Serialize({0});", rParamText).N();
            }
            this.StringBuilder
                    .T(2).A("}").N();

            this.StringBuilder
                    .T(2).A("public override void Deserialize(BinaryReader rReader)").N()
                    .T(2).A("{").N()
                        .T(3).A("base.Deserialize(rReader);").N();

            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                var rMemberText = HotfixSerializerAssists.GetClassMemberTypeText(rMemberInfo);
                var rMemberDummyText = HotfixSerializerAssists.GetClassMemberDummyText(rMemberInfo);

                if (rMemberInfo.IsDefined(typeof(HotfixSBDynamicAttribute), false) &&
                    !HotfixSerializerAssists.IsBaseType(HotfixSerializerAssists.GetMemberType(rMemberInfo), false))
                    this.StringBuilder
                        .T(3).F("this.{0} = {1}rReader.DeserializeDynamic({2});", rMemberInfo.Name, rMemberText, rMemberDummyText).N();
                else
                    this.StringBuilder
                        .T(3).F("this.{0} = {1}rReader.Deserialize({2});", rMemberInfo.Name, rMemberText, rMemberDummyText).N();
            }

            this.StringBuilder
                    .T(2).A("}").N()
                .T(1).A("}").N()
                .A("}").N();
        }
    }
}
