using System;
using System.Reflection;

namespace Knight.Core
{
    public static class ReflectTool
    {        
        public static readonly BindingFlags flags_common        = BindingFlags.Instance     |
                                                                  BindingFlags.SetField     | BindingFlags.GetField     |
                                                                  BindingFlags.GetProperty  | BindingFlags.SetProperty;
        public static readonly BindingFlags flags_public        = flags_common              | BindingFlags.Public;
        public static readonly BindingFlags flags_nonpublic     = flags_common              | BindingFlags.NonPublic;
        public static readonly BindingFlags flags_all           = flags_common              | BindingFlags.Public       | BindingFlags.NonPublic;
        
        public static readonly BindingFlags flags_method        = BindingFlags.InvokeMethod | BindingFlags.Public       | BindingFlags.NonPublic;
        public static readonly BindingFlags flags_method_inst   = flags_method              | BindingFlags.Instance;
        public static readonly BindingFlags flags_method_static = flags_method              | BindingFlags.Static;
        
        public static readonly Type[] empty_types = new Type[0];

        public static T Construct<T>(params object[] param)
        {
            return (T)Construct(typeof(T), param);
        }

        public static T TConstruct<T>(Type rType, params object[] param)
        {
            return (T)Construct(rType, param);
        }

        public static object Construct(Type rType)
        {
            var rConstructorInfo = rType.GetConstructor(empty_types);
            return rConstructorInfo.Invoke(new object[0]);
        }

        public static object Construct(Type rType, params object[] rParam)
        {
            var rParamType = new Type[rParam.Length];
            for (int nIndex = 0; nIndex < rParam.Length; ++nIndex)
                rParamType[nIndex] = rParam[nIndex].GetType();
            return Construct(rType, rParamType, rParam);
        }

        public static object Construct(Type rType, Type[] rParamTypes, object[] rParams)
        {
            var rConstructorInfo = rType.GetConstructor(rParamTypes);
            return rConstructorInfo.Invoke(rParams);
        }

        public static object CreateInstance(Type rType, BindingFlags rBindFlags)
        {
            ConstructorInfo rConstructorInfo = GetConstructorInfo(rBindFlags, rType, empty_types);
            return rConstructorInfo.Invoke(null);
        }

        public static ConstructorInfo GetConstructorInfo(BindingFlags rBindFlags, Type rType, Type[] rTypes)
        {
            return rType.GetConstructor(rBindFlags, null, rTypes, null);
        }

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

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider rProvider, bool bInherit) where T : System.Attribute
        {
            var rAttributes = rProvider.GetCustomAttributes(typeof(T), bInherit);
            var rResultAttrs = new T[rAttributes.Length];
            for (int nIndex = 0; nIndex < rAttributes.Length; ++nIndex)
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
            var rMethodInfo = rType.GetMethod(rMemberName, rBindFlags);
            return rMethodInfo.Invoke(rObject, rParams);
        }

        public static object MethodMember(Type rType, string rMemberName, BindingFlags rBindFlags, params object[] rParams)
        {
            return rType.InvokeMember(rMemberName, rBindFlags, null, null, rParams);
        }
    }
}
