//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Core.WindJson;
using Excel;
using System.Data;
using System;
using System.Reflection;
using Framework;

namespace Core.Editor
{
    public class ExcelFormat
    {
        /// <summary>
        /// Excel文件名
        /// </summary>
        public string   ExcelName;
        /// <summary>
        /// Sheet单个表名
        /// </summary>
        public string   SheetName;
        /// <summary>
        /// 对应的类名
        /// </summary>
        public string   ClassName;
        /// <summary>
        /// 对应的主键
        /// </summary>
        public string   PrimaryKey;
    }

    /// <summary>
    /// 根据配置读取Excel表格的内容，并生成相应的Json数据 
    /// </summary>
    public class ExcelReader
    {
        public string ExcelFormatConfigPath = "Assets/Editor/ExcelReader/excel_format_config.json";
        public string ExcelConfigRootPath   = "Assets/Game/Knight/GameAsset/Config/GameConfig/";

        public string HotfixDllPath         = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";

        /// <summary>
        /// Excel的表格配置
        /// </summary>
        public List<ExcelFormat> ExcelFormatConfig;

        public void Load()
        {
            string rFormatJsonTxt = File.ReadAllText(this.ExcelFormatConfigPath, System.Text.Encoding.UTF8);
            var rFormatJson = JsonParser.Parse(rFormatJsonTxt);
            this.ExcelFormatConfig = rFormatJson.ToList<ExcelFormat>();
        }

        /// <summary>
        /// 导出所有的Excel
        /// </summary>
        public void ExportAll()
        {
            for (int i = 0; i < this.ExcelFormatConfig.Count; i++)
            {
                this.Export(this.ExcelFormatConfig[i]);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        public void Export(ExcelFormat rExcelFormat)
        {
            string rConfigFile = UtilTool.PathCombine(this.ExcelConfigRootPath, "Excel", rExcelFormat.ExcelName);
            string rExportDir  = UtilTool.PathCombine(this.ExcelConfigRootPath, "Text");

            FileStream rStream = File.Open(rConfigFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader rExcelReader = ExcelReaderFactory.CreateOpenXmlReader(rStream);
            DataSet rResult = rExcelReader.AsDataSet();
            DataTable rDataTable = rResult.Tables[rExcelFormat.SheetName];
            if (rDataTable == null)
            {
                Debug.LogErrorFormat("Excel {0} has not sheet {1}.", rExcelFormat.ExcelName, rExcelFormat.SheetName);
                rExcelReader.Close();
                rStream.Close();
                return;
            }
            int rColumns = rDataTable.Columns.Count;
            int rRows = rDataTable.Rows.Count;
            if (rRows == 0)
            {
                Debug.LogErrorFormat("Excel {0} has empty rows.", rExcelFormat.ExcelName);
                rExcelReader.Close();
                rStream.Close();
                return;
            }

            Assembly rHotfixAssembly = Assembly.LoadFile(this.HotfixDllPath);
            Type rDataType = rHotfixAssembly.GetType(rExcelFormat.ClassName);
            if (rDataType == null)
            {
                Debug.LogErrorFormat("Excel {0} can not find Class {1}, please check it.", rExcelFormat.ExcelName, rExcelFormat.ClassName);
                rExcelReader.Close();
                rStream.Close();
                return;
            }

            var rTitleRow = rDataTable.Rows[0];
            var rFields = new Dict<string, FieldInfo>();
            var rKeyIDs = new Dict<string, int>();
            for (int i = 0; i < rColumns; i++)
            {
                FieldInfo rFileInfo = rDataType.GetField(rTitleRow[i].ToString());
                rFields.Add(rTitleRow[i].ToString(), rFileInfo);
                rKeyIDs.Add(rTitleRow[i].ToString(), i);
            }
            rHotfixAssembly = null;

            JsonNode rDataJson = new JsonClass();
            for (int i = 1; i < rRows; i++)
            {
                JsonNode rItemJson = new JsonClass();
                foreach (var rPair in rFields)
                {
                    string rFieldValue = rDataTable.Rows[i][rKeyIDs[rPair.Key]].ToString();
                    JsonParser rJsonParser = new JsonParser(rFieldValue);
                    JsonNode rTempNode = null;
                    try {
                        rTempNode = rJsonParser.Parser();
                    }
                    catch (Exception) {
                        rJsonParser.isValid = false;
                    }
                    if (!rJsonParser.isValid)
                        rTempNode = new JsonData(rFieldValue);

                    rItemJson.Add(rPair.Key, rTempNode);
                }
                rDataJson.Add(rDataTable.Rows[i][rKeyIDs[rExcelFormat.PrimaryKey]].ToString(), rItemJson);
            }
            File.WriteAllText(UtilTool.PathCombine(rExportDir, rExcelFormat.SheetName + ".json"), rDataJson.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            rExcelReader.Close();
            rStream.Close();
        }
        
        [MenuItem("Assets/Export Excel")]
        public static void Export()
        {
            if (Selection.activeObject == null) return;
            string rAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string rAssetExt = Path.GetExtension(rAssetPath).ToLower();
            if (!rAssetExt.Equals(".xls") && !rAssetExt.Equals(".xlsx")) return;

            string rAssetName = Path.GetFileName(rAssetPath).ToLower();
            ExcelReader rExcelReader = new ExcelReader();
            rExcelReader.Load();
            ExcelFormat rExcelFormat = rExcelReader.ExcelFormatConfig.Find((rItem)=> { return rItem.ExcelName.ToLower().Equals(rAssetName); });
            if (rExcelFormat == null) return;

            rExcelReader.Export(rExcelFormat);
        }
    }
}
