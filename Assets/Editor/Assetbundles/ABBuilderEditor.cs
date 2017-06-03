//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace UnityEditor.AssetBundles
{
    /// <summary>
    /// 资源统一打包工具
    /// </summary>
    public class ABBuilderEditor
    {
        /// <summary>
        /// 资源打包
        /// </summary>
        [MenuItem("Tools/AssetBundle/AssetBundle Build")]
        public static void Build()
        {
            ABBuilder.Instance.BuildAssetbundles(BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression);
        }

        /// <summary>
        /// 资源预处理
        /// </summary>
        [MenuItem("Tools/AssetBundle/AssetBundle Preprocess")]
        public static void Preprocess()
        {
            ABBuilder.Instance.AssetbundleEntry_Building();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("AssetBundle Preprocess Success!");
        }
    }
}