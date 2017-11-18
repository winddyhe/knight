//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;

namespace Core
{
    public static class TypeExpand
    {
        public static Type SearchBaseTo(this Type rType, Type rBaseType)
        {
            var rSearchType = rType;
            while (rSearchType.BaseType != rBaseType && null != rSearchType.BaseType)
                rSearchType = rSearchType.BaseType;

            return rSearchType.BaseType == rBaseType ? rSearchType : null;
        }
        public static Type SearchBaseTo<T>(this Type rType)
        {
            return SearchBaseTo(rType, typeof(T));
        }
    }
}