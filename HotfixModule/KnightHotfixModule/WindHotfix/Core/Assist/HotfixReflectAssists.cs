//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WindHotfix.Core
{
    public static class HotfixReflectAssists
    {
        public static readonly BindingFlags flags_common    =   BindingFlags.Instance    |
                                                                BindingFlags.SetField    | BindingFlags.GetField |
                                                                BindingFlags.GetProperty | BindingFlags.SetProperty;

        public static readonly BindingFlags flags_public    =   flags_common | BindingFlags.Public;
        public static readonly BindingFlags flags_nonpublic =   flags_common | BindingFlags.NonPublic;
        public static readonly BindingFlags flags_all       =   flags_common | BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly Type[]       empty_types     =   new Type[0];

        public static object Construct(Type rType, params object[] param)
        {
            var rParamType = new Type[param.Length];
            for (int nIndex = 0; nIndex < param.Length; ++nIndex)
                rParamType[nIndex] = param[nIndex].GetType();
            var rConstructor = rType.GetConstructor(rParamType);
            return rConstructor.Invoke(param);
        }

        public static T Construct<T>(params object[] param)
        {
            return (T)Construct(typeof(T), param);
        }

        public static T TConstruct<T>(Type rType, params object[] param)
        {
            return (T)Construct(rType, param);
        }

        public static object GetAttrMember(object rObject, string rMemberName, BindingFlags rBindFlags)
        {
            if (rObject == null) return null;
            Type rType = rObject.GetType();
            return rType.InvokeMember(rMemberName, rBindFlags, null, rObject, new object[] { });
        }

        public static void SetAttrMember(object rObject, string rMemberName, BindingFlags rBindFlags, params object[] rParams)
        {
            if (rObject == null) return;
            Type rType = rObject.GetType();
            rType.InvokeMember(rMemberName, rBindFlags, null, rObject, rParams);
        }

        public static object MethodMember(object rObject, string rMemberName, BindingFlags rBindFlags, params object[] rParams)
        {
            if (rObject == null) return null;
            Type rType = rObject.GetType();
            return rType.InvokeMember(rMemberName, rBindFlags, null, rObject, rParams);
        }

        public static object TypeConvert(Type rType, string rValueStr)
        {
            return Convert.ChangeType(rValueStr, rType);
        }

        public static Type GetType(string rClassName)
        {
            return Type.GetType(rClassName);
        }
    }
}
