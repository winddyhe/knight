//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

namespace Framework.Editor
{
    /// <summary>
    /// 在Build安装包时候的前后期处理
    /// </summary>
    public static class BuildDispose
    {
        /// <summary>
        /// 让BeforeBuild真正的只执行一次
        /// </summary>
        private static bool mIsBuilding = false;

        /// <summary>
        /// Build安装包之前做的操作
        /// </summary>
        [PostProcessScene]
        public static void BeforeBuild()
        {
            if (!mIsBuilding)
            {
                //TODO: 开始做构建安装包前的事情
                mIsBuilding = true;
            }
        }

        /// <summary>
        /// Build安装包之后做的操作
        /// </summary>
        public static void AfterBuild()
        {
            mIsBuilding = false;
        }
    }
}

