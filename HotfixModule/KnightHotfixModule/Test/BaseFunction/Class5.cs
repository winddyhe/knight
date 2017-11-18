//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using WindHotfix.Core;

namespace WindHotfix.Test
{
    public class Class5 : THotfixMB<Class5>
    {
        public string name = "Class5";

        public override void Awake()
        {
            this.name = "OnInitialize Class5";
        }
    }
}
