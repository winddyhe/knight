using System;
using Knight.Core;
using Knight.Core.Editor;

namespace Game.Editor
{
    public class GameConfigViewModel_CodeGenerator : CodeGenerator
    {
        private Type mClassType;
        private string mClassTypeFiledName;

        public GameConfigViewModel_CodeGenerator(string rFilePath, Type rClassType, string rClassTypeFiledName)
            : base(rFilePath)
        {
            this.mClassType = rClassType;
            this.mClassTypeFiledName = rClassTypeFiledName;
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
                .AL("using Knight.Framework.UI;")
                .AL("using System;")
                .N()
                .AL("/// <summary>")
                .AL("/// Auto generate code, not need modify!")
                .AL("/// </summary>")
                .AL("namespace Game")
                .AL("{");
        }

        private void WriteClass()
        {
            var rClassName = this.mClassType.Name;

            this.StringBuilder?
                .T(1).AL("[DataBinding]")
                .T(1).AL($"public class {rClassName}ViewModel : ViewModel")
                .T(1).AL("{");

            var rAllFiledInfos = this.mClassType.GetFields();
            foreach (var rFieldInfo in rAllFiledInfos)
            {
                var rFieldName = rFieldInfo.Name;
                var rFieldType = rFieldInfo.FieldType;
                var rFieldTypeName = rFieldType.Name;

                if (rFieldName.Equals("ID"))
                {
                    this.StringBuilder?
                        .T(2).AL("[DataBinding]")
                        .T(2).AL($"public {rFieldTypeName} {rFieldName} {{ get; set; }}")
                        .N();
                }
                else
                {
                    this.StringBuilder?
                        .T(2).AL("[DataBindingRelated(\"ID\")]")
                        .T(2).AL($"public {rFieldTypeName} {rFieldName} => GameConfig.Instance.{this.mClassTypeFiledName}.Table[this.ID]?.{rFieldName} ?? default({rFieldTypeName});")
                        .N(); 
                } 
            }

            this.StringBuilder?
                .T(1).AL("}");
        }

        private void WriteEnd()
        {
            this.StringBuilder?
                .AL("}")
                .N();
        }
    }
}
