using System;
using System.IO;
using Knight.Framework.Serializer;
using System.Collections.Generic;
using Knight.Core;

namespace Game
{
    [SerializerBinary]
    [SBGroup("GameConfig")]
    /// <summary>
    /// Auto generate code, don't modify it.
    /// </summary>
    public partial class Sample1Config
    {
        public int ID;
        public long Test1;
        public float Test2;
        public double Test3;
        public string Test4;
        public string Test5_Lan;

        [SBIgnore]
        public string Test5
        {
            get
            {
                return LocalizationTool.Instance.GetLanguage(Test5_Lan);
            }
        }

        public int[] Test6;
        public long[] Test7;
        public float[] Test8;
        public double[] Test9;
        public string[] Test10;
        public int[][] Test11;
        public long[][] Test12;
        public float[][] Test13;
        public double[][] Test14;
        public string[][] Test15;
    }
}