//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using Knight.Core;

namespace Knight.Hotfix.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HotfixTSIgnoreAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class HotfixTSIgnoreInheritedAttribute : HotfixTSIgnoreAttribute {}

	[AttributeUsage(AttributeTargets.Field)]
	public class HotfixTypeSearchAttribute : Attribute
	{
		public Type TypeSearchType;
		
		public HotfixTypeSearchAttribute(Type rTypeSearch)
		{
			TypeSearchType = rTypeSearch;
		}

        public List<string> TypeFullNames
        {
            get
            {
                return HotfixTypeSearchBase.GetTypeFullNames(TypeSearchType);
            }
        }
	}
    /// <summary>
    /// TypeSearchCore
    /// </summary>
    public class HotfixTypeSearchCore : THotfixSingleton<HotfixTypeSearchCore>
    {
        public List<Type> GetSubClasses(Type rType)
        {
            if (!mSearchTypes.ContainsKey(rType))
                return new List<Type>();

            return (List<Type>)mSearchTypes[rType];
        }

        private HotfixTypeSearchCore()
        {
            var rTypeSearchSubClasses = new List<KeyValuePair<Type, Type>>();

            foreach (var rType in TypeResolveManager.Instance.GetTypes("Game.Hotfix"))
            {
                if (typeof(HotfixTypeSearchBase).IsAssignableFrom(rType) &&
                    typeof(HotfixTypeSearchFull<,>) != rType &&
                    typeof(HotfixTypeSearchDefault<>) != rType &&
                    typeof(HotfixTypeSearchBase) != rType)
                {
                    var rSearchType = GetNoPublicField<Type>(rType.HotfixSearchBaseTo(typeof(HotfixTypeSearchBase)), "_Type");
                    var rIgnoreType = GetNoPublicField<Type>(rType.HotfixSearchBaseTo(typeof(HotfixTypeSearchBase)), "_IgnoreAttributeType");
                    if (null != rSearchType && null != rIgnoreType)
                        rTypeSearchSubClasses.Add(new KeyValuePair<Type, Type>(rSearchType as Type, rIgnoreType));
                }
            }

            foreach (var rType in TypeResolveManager.Instance.GetTypes("Game.Hotfix"))
            {
                foreach (var rTypeSearchSubClass in rTypeSearchSubClasses)
                {
                    if (rTypeSearchSubClass.Key.IsAssignableFrom(rType) &&
                        !rType.HotfixIsApplyAttr(rTypeSearchSubClass.Value, true))
                    {
                        var rTypeList = ReceiveTypeList(rTypeSearchSubClass.Key);
                        if (!rTypeList.Contains(rType))
                        {
                            rTypeList.Add(rType);
                        }
                    }
                }
            }
        }

        protected T GetNoPublicField<T>(Type rType, string name, T rDefault = default(T))
        {
            var rFieldInfo = rType.GetField(name, BindingFlags.Static|BindingFlags.NonPublic);
            if(null == rFieldInfo)
                return rDefault;

            return (T)rFieldInfo.GetValue(null);
        }

        protected List<Type> ReceiveTypeList(Type rType)
        {
            if (!mSearchTypes.ContainsKey(rType))
                mSearchTypes.Add(rType, new List<Type>());
            return (List<Type>)mSearchTypes[rType];
        }

        protected Hashtable mSearchTypes = new Hashtable();
    }

    public class HotfixTypeSearchBase
    {
        public static List<Type> GetTypes(Type rType)
        {
            var rPropertyInfo = GetStorePropertyInfo(rType, "Types");
            return rPropertyInfo.GetValue(null, null) as List<Type>;
        }
        public static List<string> GetTypeFullNames(Type rType)
        {
            var rPropertyInfo = GetStorePropertyInfo(rType, "TypeFullNames");
            return rPropertyInfo.GetValue(null, null) as List<string>;
        }
        public static List<string> GetTypeNames(Type rType)
        {
            var rPropertyInfo = GetStorePropertyInfo(rType, "TypeNames");
            return rPropertyInfo.GetValue(null, null) as List<string>;
        }
        public static PropertyInfo GetStorePropertyInfo(Type rType, string rStorePropertyName)
        {
            return rType.HotfixSearchBaseTo<HotfixTypeSearchBase>().GetProperty(rStorePropertyName,
                BindingFlags.Static | BindingFlags.Public);
        }
    }

    /// <summary>
    /// TypeSearch
    ///     不支持抽象类
    /// </summary>
    public class HotfixTypeSearchFull<TSearchType, TIgnoreType> : HotfixTypeSearchBase
        where TIgnoreType : System.Attribute
    {
        public static List<Type> Types
        {
            get
            {
                if (null == GTypes)
                    GTypes = HotfixTypeSearchCore.Instance.GetSubClasses(_Type);
                return GTypes;
            }
        }
        public static List<string> TypeFullNames
        {
            get
            {
                if (null == GTypeFullNames)
                {
                    GTypeFullNames = new List<string>();
                    foreach (var rType in Types)
                        GTypeFullNames.Add(rType.FullName);
                }
                return GTypeFullNames;
            }
        }
        public static List<string> TypeNames
        {
            get
            {
                if (null == GTypeNames)
                {
                    GTypeNames = new List<string>();
                    foreach (var rType in Types)
                        GTypeNames.Add(rType.Name);
                }
                return GTypeNames;
            }
        }

        private static List<Type>   GTypes;
        private static List<string> GTypeFullNames;
        private static List<string> GTypeNames;

        private static Type         _Type                = typeof(TSearchType);
        #pragma warning disable 414
        private static Type         _IgnoreAttributeType = typeof(TIgnoreType);
        #pragma warning restore 414
    }

    public class HotfixTypeSearchDefault<TSearchType> : HotfixTypeSearchFull<TSearchType, HotfixTSIgnoreAttribute> { }

}