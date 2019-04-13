//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knight.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GameBindingAttribute : Attribute
    {
        public string           Name;
        public int              Index;

        public GameBindingAttribute(string rName = "")
        {
            this.Name           = rName;
        }

        public GameBindingAttribute(int nIndex = -1)
        {
            this.Index          = nIndex;
        }
    }
}
