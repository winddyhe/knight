using Knight.Core.Editor;
using Knight.Core;
using System;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Game.Editor
{
    public class GameNetProtoViewModel_CodeGenerator : CodeGenerator
    {
        private string mNameSpace;
        private System.Type mClassType;

        public GameNetProtoViewModel_CodeGenerator(string rFilePath, string rNameSpace, Type rClassType)
            : base(rFilePath)
        {
            this.mNameSpace = rNameSpace;
            this.mClassType = rClassType;
        }

        public override void CodeGenerate()
        {
            this.WriteHead();
            this.WriteClass();
            this.WriteEnd();
            this.Save();
        }

        private void WriteHead()
        {
            this.StringBuilder?
                .AL($"using {this.mNameSpace};")
                .AL("using Google.Protobuf;")
                .AL("using Knight.Core;")
                .AL("using Knight.Framework.UI;")
                .AL("using System.Collections.Generic;")
                .AL("using System;")
                .N()
                .AL("/// <summary>")
                .AL("/// Auto generate code, not need modify.")
                .AL("/// </summary>")
                .AL("namespace Game")
                .AL("{");
        }

        private void WriteClass()
        {
            var rClassName = this.mClassType.Name;

            this.StringBuilder?
                .T(1).AL($"[DataBinding]")
                .T(1).AL($"public class {rClassName}ProtoViewModel : ViewModel")
                .T(1).AL("{");

            var rAllPropertyInfos = this.mClassType.GetProperties(ReflectTool.flags_public);

            for (int i = 0; i < rAllPropertyInfos.Length; i++)
            {
                var rPropertyInfo = rAllPropertyInfos[i];
                var rPropertyType = rPropertyInfo.PropertyType;
                var rPropertyName = rPropertyInfo.Name;
                var rPropertyTypeName = rPropertyType.FullName;
                rPropertyTypeName = rPropertyTypeName.Replace("+", ".");

                if (this.IsBaseType(rPropertyType))
                {
                    this.StringBuilder?
                        .T(2).AL("[DataBinding]")
                        .T(2).AL($"public {rPropertyTypeName} {rPropertyName} {{ get; set; }}")
                        .N();
                }
                else if (rPropertyType.IsEnum ||
                         rPropertyType == typeof(Google.Protobuf.WellKnownTypes.Timestamp))
                {
                    this.StringBuilder?
                        .T(2).AL("[DataBinding]")
                        .T(2).AL($"public {rPropertyTypeName} {rPropertyName} {{ get; set; }}")
                        .N();
                }
                else if (typeof(IMessage).IsAssignableFrom(rPropertyType))
                {
                    this.StringBuilder?
                        .T(2).AL("[DataBinding]")
                        .T(2).AL($"public {rPropertyType.Name}ProtoViewModel {rPropertyName} {{ get; set; }}")
                        .N();
                }
                else if (rPropertyType.IsGenericType && rPropertyType.GetGenericTypeDefinition() == typeof(RepeatedField<>))
                {
                    var rGenericArgType =rPropertyType.GetGenericArguments()[0];
                    this.StringBuilder?
                        .T(2).AL("[DataBinding]")
                        .T(2).AL($"public List<{rGenericArgType.Name}ProtoViewModel> {rPropertyName} {{ get; set; }}")
                        .N();
                }
            }

            this.StringBuilder?
                .T(2).AL("public override void SyncData(IMessage rMessage)")
                .T(2).AL("{")
                    .T(3).AL("if (rMessage == null)")
                    .T(3).AL("{")
                        .T(4).AL("this.ResetData();")
                        .T(4).AL("return;")
                    .T(3).AL("}")
                    .T(3).AL($"var r{rClassName} = rMessage as {rClassName};")
                    .T(3).AL($"if (r{rClassName} == null)")
                    .T(3).AL("{")
                        .T(4).AL("LogManager.LogError(\"SyncData error: message is not {rClassName}\");")
                        .T(4).AL("return;")
                    .T(3).AL("}").N();

            var rMsgVariableName = "r" + rClassName;

            for (int i = 0; i < rAllPropertyInfos.Length; i++)
            {
                var rPropertyInfo = rAllPropertyInfos[i];
                var rPropertyType = rPropertyInfo.PropertyType;
                var rPropertyName = rPropertyInfo.Name;

                if (this.IsBaseType(rPropertyType) ||
                    rPropertyType.IsEnum)
                {
                    this.StringBuilder?
                        .T(3).AL($"if (this.{rPropertyName} != {rMsgVariableName}.{rPropertyName})")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName} = {rMsgVariableName}.{rPropertyName};")
                        .T(3).AL("}");
                }
                else if (rPropertyType == typeof(Google.Protobuf.WellKnownTypes.Timestamp))
                {
                    this.StringBuilder?
                        .T(3).AL($"if (this.{rPropertyName} != {rMsgVariableName}.{rPropertyName})")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName}.MergeFrom({rMsgVariableName}.{rPropertyName});")
                        .T(3).AL("}");
                }
                else if (typeof(IMessage).IsAssignableFrom(rPropertyType))
                {
                    this.StringBuilder?
                        .T(3).AL($"if (this.{rPropertyName} == null)")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName} = new {rPropertyType.Name}ProtoViewModel();")
                        .T(3).AL("}")
                        .T(3).AL($"this.{rPropertyName}.SyncData({rMsgVariableName}.{rPropertyName});");
                }
                else if (rPropertyType.IsGenericType && rPropertyType.GetGenericTypeDefinition() == typeof(RepeatedField<>))
                {
                    var rGenericArgType = rPropertyType.GetGenericArguments()[0];
                    var rGenericArgTypeViewModelName = rGenericArgType.Name + "ProtoViewModel";

                    this.StringBuilder?
                        .T(3).AL($"if ({rMsgVariableName}.{rPropertyName} != null && {rMsgVariableName}.{rPropertyName}.Count > 0)")
                        .T(3).AL("{")
                            .T(4).AL($"for (int i = 0; i < {rMsgVariableName}.{rPropertyName}.Count; i++)")
                            .T(4).AL("{")
                                .T(5).AL($"if (i >= this.{rPropertyName}.Count)")
                                .T(5).AL("{")
                                    .T(6).AL($"this.{rPropertyName}.Add(new {rGenericArgTypeViewModelName}());")
                                .T(5).AL("}")
                                .T(5).AL($"this.{rPropertyName}[i].SyncData({rMsgVariableName}.{rPropertyName}[i]);")
                            .T(4).AL("}")
                            .T(4).AL($"if ({rMsgVariableName}.{rPropertyName}.Count < this.{rPropertyName}.Count)")
                            .T(4).AL("{")
                                .T(5).AL($"for (int i = {rMsgVariableName}.{rPropertyName}.Count; i < this.{rPropertyName}.Count; i++)")
                                .T(5).AL("{")
                                    .T(6).AL($"this.{rPropertyName}.RemoveAt(i);")
                                .T(5).AL("}")
                            .T(4).AL("}")
                        .T(3).AL("}")
                        .T(3).AL("else")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName}.Clear();")
                        .T(3).AL("}");
                }
            }
            this.StringBuilder?
                    .T(2).AL("}").N();

            this.StringBuilder?
                .T(2).AL("public void ResetData()")
                .T(2).AL("{");

            for (int i = 0; i < rAllPropertyInfos.Length; i++)
            {
                var rPropertyInfo = rAllPropertyInfos[i];
                var rPropertyType = rPropertyInfo.PropertyType;
                var rPropertyName = rPropertyInfo.Name;

                if (this.IsBaseType(rPropertyType))
                {
                    this.StringBuilder?
                        .T(3).AL($"this.{rPropertyName} = default({rPropertyType.FullName});");
                }
                else if (rPropertyType.IsEnum)
                {
                    var rFullEnumName = rPropertyType.FullName;
                    rFullEnumName = rFullEnumName.Replace("+", ".");
                    this.StringBuilder?
                        .T(3).AL($"this.{rPropertyName} = default({rFullEnumName});");
                }
                else if (rPropertyType == typeof(Google.Protobuf.WellKnownTypes.Timestamp))
                {
                    this.StringBuilder?
                        .T(3).AL($"this.{rPropertyName} = new Google.Protobuf.WellKnownTypes.Timestamp();");
                }
                else if (typeof(IMessage).IsAssignableFrom(rPropertyType))
                {
                    this.StringBuilder?
                        .T(3).AL($"if (this.{rPropertyName} != null)")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName}.ResetData();")
                        .T(3).AL("}")
                        .T(3).AL("else")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName} = new {rPropertyType.Name}ProtoViewModel();")
                        .T(3).AL("}");
                }
                else if (rPropertyType.IsGenericType && rPropertyType.GetGenericTypeDefinition() == typeof(RepeatedField<>))
                {
                    var rGenericArgType = rPropertyType.GetGenericArguments()[0];
                    var rGenericArgTypeViewModelName = rGenericArgType.Name + "ProtoViewModel";

                    this.StringBuilder?
                        .T(3).AL($"if (this.{rPropertyName} != null)")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName}.Clear();")
                        .T(3).AL("}")
                        .T(3).AL("else")
                        .T(3).AL("{")
                            .T(4).AL($"this.{rPropertyName} = new List<{rGenericArgTypeViewModelName}>();")
                        .T(3).AL("}");
                }
            }
            this.StringBuilder?
                    .T(2).AL("}");

            this.StringBuilder?
                .T(1).AL("}");
        }

        private void WriteEnd()
        {
            this.StringBuilder?
                .AL("}");
        }

        public bool IsBaseType(System.Type rType)
        {
            return
                (rType == typeof(char)) || (rType == typeof(byte)) || (rType == typeof(sbyte)) ||
                (rType == typeof(short) || (rType == typeof(ushort)) || (rType == typeof(int)) ||
                (rType == typeof(uint)) || (rType == typeof(long)) || (rType == typeof(ulong)) ||
                (rType == typeof(float)) || (rType == typeof(double)) || (rType == typeof(decimal)) ||
                (rType == typeof(string)));
        }
    }
}
