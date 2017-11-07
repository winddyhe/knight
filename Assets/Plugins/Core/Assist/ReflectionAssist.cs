//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 用于反射的帮助类
    /// </summary>
    public class ReflectionAssist
    {
        public static readonly BindingFlags flags_common    = BindingFlags.Instance     |
                                                              BindingFlags.SetField     | BindingFlags.GetField     |
                                                              BindingFlags.GetProperty  | BindingFlags.SetProperty;
        public static readonly BindingFlags flags_public    = flags_common              | BindingFlags.Public;
        public static readonly BindingFlags flags_nonpublic = flags_common              | BindingFlags.NonPublic;
        public static readonly BindingFlags flags_all       = flags_common              | BindingFlags.Public       | BindingFlags.NonPublic;

        public static readonly BindingFlags flags_method_inst = BindingFlags.Instance     | BindingFlags.InvokeMethod | BindingFlags.Public     | BindingFlags.NonPublic;
        public static readonly BindingFlags flags_method      = BindingFlags.InvokeMethod | BindingFlags.Public       | BindingFlags.NonPublic;

        public static readonly Type[]       empty_types     = new Type[0];

        public static ConstructorInfo GetConstructorInfo(BindingFlags rBindFlags, Type rType, Type[] rTypes)
        {
            return rType.GetConstructor(rBindFlags, null, rTypes, null);
        }

        public static object CreateInstance(Type rType, BindingFlags rBindFlags)
        {
            ConstructorInfo rConstructorInfo = GetConstructorInfo(rBindFlags, rType, empty_types);
            return rConstructorInfo.Invoke(null);
        }

        public static object Construct(Type rType)
        {
            ConstructorInfo rConstructorInfo = GetConstructorInfo(flags_all, rType, empty_types);
            return rConstructorInfo.Invoke(null);
        }

        public static object Construct(Type rType, Type[] rTypes, params object[] rParams)
        {
            ConstructorInfo rConstructorInfo = GetConstructorInfo(flags_all, rType, rTypes);
            return rConstructorInfo.Invoke(null, rParams);
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
    }
}
