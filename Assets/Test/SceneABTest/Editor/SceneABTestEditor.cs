//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

public class SceneABTestEditor
{
    //[MenuItem("Tools/打包SceneTest")]
    private static void ABTest()
    {
        BuildPipeline.BuildAssetBundles("Assets/Test/SceneABTest/ABS", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
