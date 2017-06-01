//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.AssetBundles
{
    public class ABHistory
    {
        public class Entry
        {
            public string   Time;
            public string   Path;
            public string[] ABItems;
        }
    }
}
