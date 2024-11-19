using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knight.Tools.ExcelExporter
{
    public enum ExcelSheetType
    {
        Null,
        Int32,
        Int64,
        Float,
        Double,
        String,
        LanString,
        Int32Array,
        Int64Array,
        FloatArray,
        DoubleArray,
        StringArray,
        Int32Array2D,
        Int64Array2D,
        FloatArray2D,
        DoubleArray2D,
        StringArray2D
    }

    public class ExcelSheetData
    {
        public int RowNum;
        public int ColNum;
        public string SheetName;
        public List<string> FieldNames;
        public List<string> FieldTypes;
        public List<string> FieldComments;
        public JObject JsonNode;
    }
}
