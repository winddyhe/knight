using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using Testura.Code.Builders;
using Testura.Code;
using Testura.Code.Saver;
using Testura.Code.Models;
using Testura.Code.Generators.Common.Arguments.ArgumentTypes;
using Testura.Code.Models.Types;
using Testura.Code.Generators.Class;
using Testura.Code.Generators.Common;
using Testura.Code.Statements;
using Testura.Code.Models.Properties;
using Testura.Code.Models.References;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Knight.Tools.ExcelExporter
{
    public class ExcelExportConfig
    {
        public string ExcelPath;
        public string JsonPath;
        public string ConfigCodePath;
        public string ConfigBattleCodePath;
        public string ConfigBattleConvertPath;
        public List<string> BattleExcelNames;
    }

    public class ExcelExporter
    {
        private ExcelExportConfig mExportConfig;

        public void Initialize(string rConfigJsonPath)
        {
            var rConfigContent = File.ReadAllText(rConfigJsonPath);
            this.mExportConfig = JsonConvert.DeserializeObject<ExcelExportConfig>(rConfigContent);

            // 创建不存在的目录
            if (!Directory.Exists(this.mExportConfig.JsonPath))
            {
                Directory.CreateDirectory(this.mExportConfig.JsonPath);
            }
            if (!Directory.Exists(this.mExportConfig.ConfigCodePath))
            {
                Directory.CreateDirectory(this.mExportConfig.ConfigCodePath);
            }

            // 获取所有的Excel文件
            var rExcelSheetDatas = new List<ExcelSheetData>();
            var rDirectoryInfo = new DirectoryInfo(this.mExportConfig.ExcelPath);
            var rAllExcelFiles = rDirectoryInfo.GetFiles("*.xlsx");
            for (int i = 0; i < rAllExcelFiles.Length; i++)
            {
                if (rAllExcelFiles[i].Name.Contains("~$")) continue;
                var rExcelSheetData = this.ProcessExcelFile(rAllExcelFiles[i].FullName);
                rExcelSheetDatas.AddRange(rExcelSheetData);
            }
            this.GenerateGameConfigClassCode(rExcelSheetDatas);
            this.GenerateBattleConfigConverterClassCode(rExcelSheetDatas);
        }

        private List<ExcelSheetData> ProcessExcelFile(string rExcelFilePath)
        {
            var rExcelSheetDatas = this.BuildExcelSheetData(rExcelFilePath);
            // 生成Json文件
            for (int i = 0; i < rExcelSheetDatas.Count; i++)
            {
                this.GenerateJsonFile(rExcelSheetDatas[i]);
                this.GenerateExcelClassCode(rExcelSheetDatas[i], "", this.mExportConfig.ConfigCodePath, false);
                if (this.mExportConfig.BattleExcelNames != null && this.mExportConfig.BattleExcelNames.Contains(rExcelSheetDatas[i].SheetName))
                {
                    this.GenerateExcelClassCode(rExcelSheetDatas[i], "Battle", this.mExportConfig.ConfigBattleCodePath, true);
                }
            }
            // 生成类代码
            return rExcelSheetDatas;
        }

        private List<ExcelSheetData> BuildExcelSheetData(string rExcelFilePath)
        {
            var rExcelSheetDatas = new List<ExcelSheetData>();

            var rStreamData = File.Open(rExcelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var rReaderData = ExcelReaderFactory.CreateOpenXmlReader(rStreamData))
            {
                var nSheetNum = rReaderData.ResultsCount;
                for (int i = 0; i < nSheetNum; i++)
                {
                    var rExcelSheetData = new ExcelSheetData();
                    rExcelSheetData.RowNum = rReaderData.RowCount;
                    rExcelSheetData.ColNum = rReaderData.FieldCount;
                    rExcelSheetData.SheetName = rReaderData.Name;
                    rExcelSheetData.FieldNames = new List<string>();
                    rExcelSheetData.FieldTypes = new List<string>();
                    rExcelSheetData.FieldComments = new List<string>();
                    rExcelSheetData.JsonNode = new JObject();

                    // 一行都没得，直接跳过该表
                    if (rExcelSheetData.RowNum == 0 || rExcelSheetData.ColNum == 0)
                    {
                        rReaderData.NextResult();
                        continue;
                    }
                    // 读取第一行
                    rReaderData.Read();
                    // 判断第一行第一列的值是不是ID，如果不是则跳过这张表，切换到下一张表
                    var rFirstRowColValue = rReaderData.GetString(0);
                    if (rFirstRowColValue == null || !rFirstRowColValue.Equals("ID"))
                    {
                        rReaderData.NextResult();
                        continue;
                    }
                    for (int j = 0; j < rExcelSheetData.ColNum; j++)
                    {
                        rExcelSheetData.FieldNames.Add(rReaderData.GetString(j));
                    }
                    // 读取第二行
                    rReaderData.Read();
                    for (int j = 0; j < rExcelSheetData.ColNum; j++)
                    {
                        rExcelSheetData.FieldTypes.Add(rReaderData.GetString(j));
                    }
                    // 读取第三行
                    rReaderData.Read();
                    for (int j = 0; j < rExcelSheetData.ColNum; j++)
                    {
                        rExcelSheetData.FieldComments.Add(rReaderData.GetString(j));
                    }
                    // 从第四行开始读数据
                    for (int j = 3; j < rExcelSheetData.RowNum; j++)
                    {
                        rReaderData.Read();
                        var rRowDataJsonNode = new JObject();
                        for (int k = 0; k < rExcelSheetData.ColNum; k++)
                        {
                            if (string.IsNullOrEmpty(rExcelSheetData.FieldTypes[k])) continue;

                            var rFieldName = rExcelSheetData.FieldNames[k];
                            var rFieldType = ExcelUtils.ExcelStringTypesToExcelTypes[rExcelSheetData.FieldTypes[k]];

                            if (rReaderData.GetValue(k) == null)
                            {
                                continue;
                            }
                            switch (rFieldType)
                            {
                                case ExcelSheetType.Int32:
                                    {
                                        rRowDataJsonNode[rFieldName] = ExcelUtils.ParseInt32(rReaderData.GetValue(k).ToString());
                                        break;
                                    }
                                case ExcelSheetType.Int64:
                                    {
                                        rRowDataJsonNode[rFieldName] = ExcelUtils.ParseInt64(rReaderData.GetValue(k).ToString());
                                        break;
                                    }
                                case ExcelSheetType.Float:
                                    {
                                        rRowDataJsonNode[rFieldName] = ExcelUtils.ParseFloat(rReaderData.GetValue(k).ToString());
                                        break;
                                    }
                                case ExcelSheetType.Double:
                                    {
                                        rRowDataJsonNode[rFieldName] = ExcelUtils.ParseDouble(rReaderData.GetValue(k).ToString());
                                        break;
                                    }
                                case ExcelSheetType.String:
                                    {
                                        rRowDataJsonNode[rFieldName] = rReaderData.GetValue(k).ToString();
                                        break;
                                    }
                                case ExcelSheetType.LanString:
                                    {
                                        rRowDataJsonNode[rFieldName] = ExcelUtils.ParseInt64(rReaderData.GetValue(k).ToString());
                                        break;
                                    }
                                case ExcelSheetType.Int32Array:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseInt32Array(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.Int64Array:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseInt64Array(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.FloatArray:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseFloatArray(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.DoubleArray:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseDoubleArray(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.StringArray:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseStringArray(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.Int32Array2D:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseInt32Array2D(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.Int64Array2D:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseInt64Array2D(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.FloatArray2D:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseFloatArray2D(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.DoubleArray2D:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseDoubleArray2D(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                                case ExcelSheetType.StringArray2D:
                                    {
                                        var rColValueStr = string.Empty;
                                        if (rReaderData.GetFieldType(k) != typeof(string))
                                            rColValueStr = rReaderData.GetValue(k).ToString();
                                        else
                                            rColValueStr = rReaderData.GetString(k);
                                        var rArray = ExcelUtils.ParseStringArray2D(rColValueStr);
                                        rRowDataJsonNode[rFieldName] = JArray.FromObject(rArray);
                                        break;
                                    }
                            }
                        }
                        rExcelSheetData.JsonNode[rReaderData.GetValue(0).ToString()] = rRowDataJsonNode;
                    }
                    rReaderData.NextResult();
                    rExcelSheetDatas.Add(rExcelSheetData);
                }
                return rExcelSheetDatas;
            }
        }

        private void GenerateJsonFile(ExcelSheetData rExcelSheetData)
        {
            var rExcelSheetJsonContent = JsonConvert.SerializeObject(rExcelSheetData.JsonNode, Formatting.Indented);
            var rFilePath = this.mExportConfig.JsonPath + "/" + rExcelSheetData.SheetName + ".json";
            File.WriteAllText(rFilePath, rExcelSheetJsonContent);
        }

        private void GenerateExcelClassCode(ExcelSheetData rExcelSheetData, string rSheetNamePrefix, string rConfigCodePath, bool bIsBattle)
        {
            var rRealSheetName = rSheetNamePrefix + rExcelSheetData.SheetName;
            var rExcelSheetClass = new ClassBuilder(rRealSheetName + "Config", "Game");
            if (!bIsBattle)
            {
                rExcelSheetClass.WithUsings("System", "System.IO", "Knight.Framework.Serializer", "System.Collections.Generic", "Knight.Core");
            }
            else
            {
                rExcelSheetClass.WithUsings("System", "System.IO", "System.Collections.Generic", "Knight.Core");
            }
            if (!bIsBattle)
            {
                rExcelSheetClass
                    .WithAttributes(
                        new Attribute("SerializerBinary"),
                        new Attribute("SBGroup", new List<IArgument>() { new ValueArgument("GameConfig") }));
            }
            rExcelSheetClass.WithModifiers(Modifiers.Public, Modifiers.Partial)
                            .WithSummary("Auto generate code, don't modify it.");
            for (int i = 0; i < rExcelSheetData.FieldTypes.Count; i++)
            {
                if (string.IsNullOrEmpty(rExcelSheetData.FieldTypes[i])) continue;

                var rFieldType = ExcelUtils.ExcelStringTypesToExcelTypes[rExcelSheetData.FieldTypes[i]];
                var rType = ExcelUtils.ExcelTypesToSystemTypes[rFieldType];
                var bIsLanguageField = rFieldType == ExcelSheetType.LanString;
                rExcelSheetClass.WithFields(
                    new Field(bIsLanguageField ? rExcelSheetData.FieldNames[i] + "_Lan" : rExcelSheetData.FieldNames[i], rType, new List<Modifiers>() { Modifiers.Public }));
                // 如果是多语言字段 要再加一个Property属性直接返回多语言的数值
                if (bIsLanguageField)
                {
                    rExcelSheetClass.WithProperties(
                        PropertyGenerator.Create(
                            new BodyProperty(
                                rExcelSheetData.FieldNames[i],
                                typeof(string),
                                BodyGenerator.Create(
                                    Statement.Jump.Return(Statement.Expression.Invoke("LocalizationTool.Instance", "GetLanguage", new List<IArgument>() { new VariableArgument(rExcelSheetData.FieldNames[i] + "_Lan") }).AsExpression())
                                    ), 
                                new List<Modifiers>() { Modifiers.Public }, 
                                new List<Attribute> { new Attribute("SBIgnore") })
                            )
                        );
                }
            }

            var rCodeSaver = new CodeSaver();
            var rFilePath = rConfigCodePath + "/" + rRealSheetName + ".cs";
            rCodeSaver.SaveCodeToFile(rExcelSheetClass.Build(), rFilePath);

            if (!bIsBattle)
            {
                var rExcelSheetTableClass = new ClassBuilder(rRealSheetName + "ConfigTable", "Game")
                    .WithUsings("System", "System.IO", "Knight.Framework.Serializer", "System.Collections.Generic")
                    .WithModifiers(Modifiers.Public, Modifiers.Partial)
                    .WithAttributes(new Attribute("SerializerBinary"),
                                    new Attribute("SBGroup", new List<IArgument>() { new ValueArgument("GameConfig") }),
                                    new Attribute("SBFileReadWrite"))
                    .WithSummary("Auto generate code, don't modify it.");
                var rFirstType = ExcelUtils.ExcelTypesToSystemTypes[ExcelUtils.ExcelStringTypesToExcelTypes[rExcelSheetData.FieldTypes[0]]];
                rExcelSheetTableClass.WithFields(
                    new Field("Table", CustomType.Create($"Dictionary<{rFirstType}, {rRealSheetName}Config>"), new List<Modifiers>() { Modifiers.Public }));
                rCodeSaver = new CodeSaver();
                rFilePath = rConfigCodePath + "/" + rRealSheetName + "Table.cs";
                rCodeSaver.SaveCodeToFile(rExcelSheetTableClass.Build(), rFilePath);
            }
        }

        private void GenerateGameConfigClassCode(List<ExcelSheetData> rExcelSheetDatas)
        {
            var rGameConfigClass = new ClassBuilder("GameConfig", "Game")
                .WithUsings("System", "System.IO", "System.Collections.Generic", "Knight.Core", "Knight.Framework.Serializer")
                .WithModifiers(Modifiers.Public, Modifiers.Partial)
                .WithAttributes(new Attribute("SerializerBinary"), 
                                new Attribute("SBGroup", new List<IArgument>() { new ValueArgument("GameConfig") }))
                .WithSummary("Auto generate code, don't modify it.");

            for (int i = 0; i < rExcelSheetDatas.Count; i++)
            {
                var rSheetName = rExcelSheetDatas[i].SheetName;
                if (rSheetName.Equals("Sample1") || rSheetName.Equals("Sample2")) continue;

                var rType = CustomType.Create(rSheetName + "ConfigTable");
                rGameConfigClass.WithFields(
                    new Field(
                        rExcelSheetDatas[i].SheetName,
                        rType,
                        new List<Modifiers>() { Modifiers.Public },
                        new List<Attribute>() { new Attribute("JsonPath", new List<IArgument>() { new ValueArgument(rExcelSheetDatas[i].SheetName) }) }));
            }
            var rCodeSaver = new CodeSaver();
            var rFilePath = this.mExportConfig.ConfigCodePath + "/" + "GameConfig.Excel.cs";
            rCodeSaver.SaveCodeToFile(rGameConfigClass.Build(), rFilePath);
        }

        private void GenerateBattleConfigConverterClassCode(List<ExcelSheetData> rExcelSheetDatas)
        {
            var rBattleConfigConverterClass = new ClassBuilder("BattleConfigConverter", "Game")
                .WithUsings("System", "System.IO", "System.Collections.Generic", "Knight.Core")
                .WithModifiers(Modifiers.Public, Modifiers.Partial)
                .WithSummary("Auto generate code, don't modify it.");

            for (int i = 0; i < rExcelSheetDatas.Count; i++)
            {
                var rSheetName = rExcelSheetDatas[i].SheetName;
                if (rSheetName.Equals("Sample1") || rSheetName.Equals("Sample2")) continue;
                if (!this.mExportConfig.BattleExcelNames.Contains(rSheetName)) continue;

                var rType = CustomType.Create($"{rSheetName}Config");
                var rBattleType = CustomType.Create($"Battle{rSheetName}Config");
                
                var rStatements = new List<StatementSyntax>();
                for (int j = 0; j < rExcelSheetDatas[i].FieldNames.Count; j++)
                {
                    var rFieldName = rExcelSheetDatas[i].FieldNames[j];

                    var rStatement = Statement.Declaration.Assign(
                        new VariableReference("rBattleConfig", new MemberReference($"{rFieldName}")), 
                        new VariableReference("rConfig", new MemberReference(rFieldName)));
                    rStatements.Add(rStatement);
                }
                var rBody = BodyGenerator.Create(rStatements.ToArray());

                rBattleConfigConverterClass.WithMethods(
                    new MethodBuilder($"{rSheetName}ToBattle")
                    .WithParameters(new Parameter("rBattleConfig", rBattleType, ParameterModifiers.Ref), new Parameter("rConfig", rType))
                    .WithModifiers(Modifiers.Public, Modifiers.Static)
                    .WithBody(rBody)
                    .Build());
            }

            var rCodeSaver = new CodeSaver();
            var rFilePath = this.mExportConfig.ConfigBattleConvertPath + "/" + "BattleConfigConverter.cs";
            rCodeSaver.SaveCodeToFile(rBattleConfigConverterClass.Build(), rFilePath);
        }
    }
}
