//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Core.Serializer.Editor
{
    public class SerializerBinaryEditor1 : UnityEditor.Editor
    {
        private const string    mGeneratePathRoot      = "Assets/Generate/SerializerBinary/";
        private const string    mGeneratePath          = mGeneratePathRoot + "Runtime/";
        private const string    mCommonSerializerPath  = mGeneratePath     + "CommonSerializer.cs";
        
        [MenuItem("Tools/CodeGenerate/Test1")]
        public static void CodeGenerate()
        {
            var rCommonSerializer = new CodeGenerator_CommonSerializer("D:/common.cs");
            
            rCommonSerializer.WriteHead();
            bool bIsDynamic = true;
            rCommonSerializer.WriteArray(typeof(UnityEngine.AssetBundles.ABVersion[]), bIsDynamic);
            rCommonSerializer.WriteList(typeof(List<UnityEngine.AssetBundles.ABVersion>), bIsDynamic);
            rCommonSerializer.WriteDictionary(typeof(Dict<int, UnityEngine.AssetBundles.ABVersion>), bIsDynamic);
            rCommonSerializer.WriteDictionary(typeof(Dictionary<int, UnityEngine.AssetBundles.ABVersion>), bIsDynamic);
            rCommonSerializer.WriteEnd();
            rCommonSerializer.Save();

            var rClassSerializer = new CodeGenerator_ClassSerializer("D:/version.cs");
            rClassSerializer.WriteHead();
            rClassSerializer.WriteClass(typeof(UnityEngine.AssetBundles.ABVersion));
            rClassSerializer.WriteEnd();
            rClassSerializer.Save();
        }
    }
}
