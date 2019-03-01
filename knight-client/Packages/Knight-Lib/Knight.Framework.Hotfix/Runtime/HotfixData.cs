//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knight.Framework.Hotfix
{
    [System.Serializable]
    public class UnityObject
    {
        public string       Name;
        public Object       Object;
        public string       Type;
    }

    [System.Serializable]
    public class BaseDataDisplayObject
    {
        public string       Name;
        public int          IntObject;
        public long         LongObject;
        public float        FloatObject;
        public double       DoubleObject;
        public string       StringObject;
        public string       Type;
    }

    public class BaseDataObject
    {
        public string       Name;
        public object       Object;
        public string       Type;
    }

    public enum BaseDataType
    {
        Int,
        Long,
        Float,
        Double,
        String,
    }
}
