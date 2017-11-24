using System;
using System.Collections.Generic;

namespace Core.Serializer.Editor
{
    public class CodeGenerator_ClassSerializer : CodeGenerator
    {
        public string GroupName = string.Empty;

        public CodeGenerator_ClassSerializer(string rFilePath)
            : base(rFilePath)
        {

        }

        public override void WriteHead()
        {
            this.Write(@"
using System.IO;
using Core;
using Core.Serializer;
using Game.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>");
        }

        public override void WriteEnd()
        {
            this.Write();
        }

        public void WriteClass(Type rType)
        {
            this.Write(
$"namespace {rType.Namespace}");
            this.Write(
"{");
            this.Write(
$"    public partial class {rType.Name}");
            this.Write(
@"    {
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);");

            var rAllSerializeMembers = SerializerAssists.FindSerializeMembers(rType);
            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                var rParamText = SerializerAssists.GetClassMemberDummyText(rAllSerializeMembers[i]);

                if (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), true) && 
                    !SerializerAssists.IsBaseType(SerializerAssists.GetMemberType(rMemberInfo), false))
                    this.Write($"            rWriter.SerializeDynamic({rParamText});");
                else
                    this.Write($"            rWriter.Serialize({rParamText});");
            }
            this.Write(
"        }");

            this.Write(@"	
        public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);");

            for (int i = 0; i < rAllSerializeMembers.Count; i++)
            {
                var rMemberInfo = rAllSerializeMembers[i];
                var rMemberText = SerializerAssists.GetClassMemberTypeText(rMemberInfo);
                var rMemberDummyText = SerializerAssists.GetClassMemberDummyText(rMemberInfo);

                if (rMemberInfo.IsDefined(typeof(SBDynamicAttribute), false) &&
                    !SerializerAssists.IsBaseType(SerializerAssists.GetMemberType(rMemberInfo), false))
                    this.Write($"            this.{rMemberInfo.Name} = {rMemberText}rReader.DeserializeDynamic({rMemberDummyText});");
                else
                    this.Write($"            this.{rMemberInfo.Name} = {rMemberText}rReader.Deserialize({rMemberDummyText});");
            }
            this.Write(
@"        }
    }
}
");
            this.GroupName = string.Empty;
            var rAttributes = rType.GetCustomAttributes<SBGroupAttribute>(true);
            if (rAttributes.Length > 0)
                this.GroupName = rAttributes[0].GroupName;
        }
    }
}
