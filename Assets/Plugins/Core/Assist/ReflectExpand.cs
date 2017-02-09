//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Reflection;
using System;

namespace Core
{
    public static class ReflectExpand
    {
        public static object Construct(Type rType, params object[] param)
        {
            Debugger.AssertE(null != rType, "rType invalid");

            var rParamType = new Type[param.Length];
            for (int nIndex = 0; nIndex < param.Length; ++nIndex)
                rParamType[nIndex] = param[nIndex].GetType();

            var rConstructor = rType.GetConstructor(rParamType);
            Debugger.AssertE(null != rConstructor, "Invalid Constructor");

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
    }

    public static class ICustomAttributeProviderExpand
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider rProvider, bool bInherit)
            where T : System.Attribute
        {
            var rAttributes = rProvider.GetCustomAttributes(typeof(T), bInherit);
            if (rAttributes.Length == 0)
                return default(T);
            return (T)(rAttributes[0]);
        }
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider rProvider, bool bInherit)
            where T : System.Attribute
        {
            var rAttributes = rProvider.GetCustomAttributes(typeof(T), bInherit);
            var rResultAttrs = new T[rAttributes.Length];
            for (int nIndex = 0; nIndex < rAttributes.Length; ++ nIndex)
                rResultAttrs[nIndex] = rAttributes[nIndex] as T;
            return rResultAttrs;
        }

        /// <summary>
        /// IsApplyAttr<T>/IsApplyAttr
        ///     IsDefined函数功能是判定一个Attribute的使用被定义在该类中或者父类中。但如果使用的Attribute标记的是Inherit=false
        ///     该函数返回true，但无法通过GetCustomAttributes获取。IsApplyAttr为了和GetCustomAttributes的结果对应，
        ///     IsApplyAttr返回true，GetCustomAttributes一定能获得。
        /// </summary>
        public static bool IsApplyAttr<T>(this ICustomAttributeProvider rProvider, bool bInherit)
        {
            return rProvider.GetCustomAttributes(typeof(T), bInherit).Length > 0;
        }
        public static bool IsApplyAttr(this ICustomAttributeProvider rProvider, Type rType, bool bInherit)
        {
            return rProvider.GetCustomAttributes(rType, bInherit).Length > 0;
        }
    }
}