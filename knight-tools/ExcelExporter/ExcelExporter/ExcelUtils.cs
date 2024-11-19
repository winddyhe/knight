using System;
using System.Collections.Generic;
using System.Text;

namespace Knight.Tools.ExcelExporter
{
    public static class ExcelUtils
    {

        public static Dictionary<string, ExcelSheetType> ExcelStringTypesToExcelTypes = new Dictionary<string, ExcelSheetType>()
        {
            { "", ExcelSheetType.Null },
            { "int32", ExcelSheetType.Int32 },
            { "int64", ExcelSheetType.Int64 },
            { "float", ExcelSheetType.Float },
            { "double", ExcelSheetType.Double },
            { "string", ExcelSheetType.String },
            { "lan_string", ExcelSheetType.LanString },
            { "int32[]", ExcelSheetType.Int32Array },
            { "int64[]", ExcelSheetType.Int64Array },
            { "float[]", ExcelSheetType.FloatArray },
            { "double[]", ExcelSheetType.DoubleArray },
            { "string[]", ExcelSheetType.StringArray },
            { "int32[][]", ExcelSheetType.Int32Array2D },
            { "int64[][]", ExcelSheetType.Int64Array2D },
            { "float[][]", ExcelSheetType.FloatArray2D },
            { "double[][]", ExcelSheetType.DoubleArray2D },
            { "string[][]", ExcelSheetType.StringArray2D },
        };

        public static Dictionary<ExcelSheetType, string> ExcelTypesToPrimitiveTypes = new Dictionary<ExcelSheetType, string>()
        {
            { ExcelSheetType.Int32, "int" },
            { ExcelSheetType.Int64, "long" },
            { ExcelSheetType.Float, "long" },
            { ExcelSheetType.Double, "long" },
            { ExcelSheetType.String, "long" },
            { ExcelSheetType.LanString, "string" },
            { ExcelSheetType.Int32Array, "int[]" },
            { ExcelSheetType.Int64Array, "long[]" },
            { ExcelSheetType.FloatArray, "float[]" },
            { ExcelSheetType.DoubleArray, "double[]" },
            { ExcelSheetType.StringArray, "string[]" },
            { ExcelSheetType.Int32Array2D, "int[][]" },
            { ExcelSheetType.Int64Array2D, "long[][]" },
            { ExcelSheetType.FloatArray2D, "float[][]" },
            { ExcelSheetType.DoubleArray2D, "double[][]" },
            { ExcelSheetType.StringArray2D, "string[][]" },
        };

        public static Dictionary<ExcelSheetType, Type> ExcelTypesToSystemTypes = new Dictionary<ExcelSheetType, Type>()
        {
            { ExcelSheetType.Int32, typeof(int) },
            { ExcelSheetType.Int64, typeof(long) },
            { ExcelSheetType.Float, typeof(float) },
            { ExcelSheetType.Double, typeof(double) },
            { ExcelSheetType.String, typeof(string) },
            { ExcelSheetType.LanString, typeof(string) },
            { ExcelSheetType.Int32Array, typeof(int[]) },
            { ExcelSheetType.Int64Array, typeof(long[]) },
            { ExcelSheetType.FloatArray, typeof(float[]) },
            { ExcelSheetType.DoubleArray, typeof(double[]) },
            { ExcelSheetType.StringArray, typeof(string[]) },
            { ExcelSheetType.Int32Array2D, typeof(int[][]) },
            { ExcelSheetType.Int64Array2D, typeof(long[][]) },
            { ExcelSheetType.FloatArray2D, typeof(float[][]) },
            { ExcelSheetType.DoubleArray2D, typeof(double[][]) },
            { ExcelSheetType.StringArray2D, typeof(string[][]) },
        };

        public static int ParseInt32(string rValueString)
        {
            return int.Parse(rValueString);
        }

        public static long ParseInt64(string rValueString)
        {
            return long.Parse(rValueString);
        }

        public static float ParseFloat(string rValueString)
        {
            return float.Parse(rValueString);
        }

        public static double ParseDouble(string rValueString)
        {
            return double.Parse(rValueString);
        }

        public static int[] ParseInt32Array(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray = new int[rValueSplitStrs.Length];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                rArray[i] = int.Parse(rValueSplitStrs[i]);
            }
            return rArray;
        }

        public static long[] ParseInt64Array(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray = new long[rValueSplitStrs.Length];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                rArray[i] = long.Parse(rValueSplitStrs[i]);
            }
            return rArray;
        }

        public static float[] ParseFloatArray(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray = new float[rValueSplitStrs.Length];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                rArray[i] = float.Parse(rValueSplitStrs[i]);
            }
            return rArray;
        }

        public static double[] ParseDoubleArray(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray = new double[rValueSplitStrs.Length];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                rArray[i] = double.Parse(rValueSplitStrs[i]);
            }
            return rArray;
        }

        public static string[] ParseStringArray(string rValueString)
        {
            return rValueString.Split('|');
        }

        public static int[][] ParseInt32Array2D(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray2D = new int[rValueSplitStrs.Length][];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                var rSecondValueSplitStrs = rValueSplitStrs[i].Split(',');
                rArray2D[i] = new int[rSecondValueSplitStrs.Length];
                for (int j = 0; j < rSecondValueSplitStrs.Length; j++)
                {
                    rArray2D[i][j] = int.Parse(rSecondValueSplitStrs[j]);
                }
            }
            return rArray2D;
        }

        public static long[][] ParseInt64Array2D(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray2D = new long[rValueSplitStrs.Length][];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                var rSecondValueSplitStrs = rValueSplitStrs[i].Split(',');
                rArray2D[i] = new long[rSecondValueSplitStrs.Length];
                for (int j = 0; j < rSecondValueSplitStrs.Length; j++)
                {
                    rArray2D[i][j] = long.Parse(rSecondValueSplitStrs[j]);
                }
            }
            return rArray2D;
        }

        public static float[][] ParseFloatArray2D(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray2D = new float[rValueSplitStrs.Length][];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                var rSecondValueSplitStrs = rValueSplitStrs[i].Split(',');
                rArray2D[i] = new float[rSecondValueSplitStrs.Length];
                for (int j = 0; j < rSecondValueSplitStrs.Length; j++)
                {
                    rArray2D[i][j] = float.Parse(rSecondValueSplitStrs[j]);
                }
            }
            return rArray2D;
        }

        public static double[][] ParseDoubleArray2D(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray2D = new double[rValueSplitStrs.Length][];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                var rSecondValueSplitStrs = rValueSplitStrs[i].Split(',');
                rArray2D[i] = new double[rSecondValueSplitStrs.Length];
                for (int j = 0; j < rSecondValueSplitStrs.Length; j++)
                {
                    rArray2D[i][j] = double.Parse(rSecondValueSplitStrs[j]);
                }
            }
            return rArray2D;
        }

        public static string[][] ParseStringArray2D(string rValueString)
        {
            var rValueSplitStrs = rValueString.Split('|');
            var rArray2D = new string[rValueSplitStrs.Length][];
            for (int i = 0; i < rValueSplitStrs.Length; i++)
            {
                rArray2D[i] = rValueSplitStrs[i].Split(',');
            }
            return rArray2D;
        }
    }
}
