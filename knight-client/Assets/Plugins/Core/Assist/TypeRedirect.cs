//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Core
{
    public class ITypeRedirect
    {
        public static Func<Type, Type> GetRedirectTypeHandler;
        
        public static Type GetRedirectType(Type rSrcType)
        {
            if (GetRedirectTypeHandler == null)
                return rSrcType;
            return GetRedirectTypeHandler(rSrcType);
        }
    }
}
