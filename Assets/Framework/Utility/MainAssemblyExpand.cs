//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Reflection;

namespace Framework
{
    public static class MainAssemblyExpand
    {
        public static Type GetType(string rClassName)
        {
            return Type.GetType(rClassName);
        }
    }
}
