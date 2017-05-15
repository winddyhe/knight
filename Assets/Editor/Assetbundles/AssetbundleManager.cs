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
    public class AssetbundleManager
    {
        [MenuItem("Tools/资源统一打包")]
        public static void Build()
        {
            AssetbundleHelper.Instance.BuildAssetbundles();
            Debug.Log("资源打包完成！");
        }
    }
}