//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace WindHotfix.Core
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
            foreach (var rAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var rType in rAssembly.GetTypes())
                {
                    if (typeof(HotfixTypeSearchBase).IsAssignableFrom(rType) &&
                        typeof(HotfixTypeSearchFull<,>) != rType &&
                        typeof(HotfixTypeSearchDefault<>) != rType &&
                        typeof(HotfixTypeSearchBase) != rType)
                    {
                        //Console.WriteLine(rType);
                        var rSearchType = GetNoPublicField<Type>(rType.HotfixSearchBaseTo(typeof(HotfixTypeSearchBase)), "_Type");
                        var rIgnoreType = GetNoPublicField<Type>(rType.HotfixSearchBaseTo(typeof(HotfixTypeSearchBase)), "_IgnoreAttributeType");
                        if (null != rSearchType && null != rIgnoreType)
                            rTypeSearchSubClasses.Add(new KeyValuePair<Type, Type>(rSearchType as Type, rIgnoreType));
                    }
                }
            }
            foreach (var rAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var rType in rAssembly.GetTypes())
                {

                    foreach (var rTypeSearchSubClass in rTypeSearchSubClasses)
                    {
                        if (rTypeSearchSubClass.Key.IsAssignableFrom(rType) &&
                            !rType.HotfixIsApplyAttr(rTypeSearchSubClass.Value, true))
                        {
                            ReceiveTypeList(rTypeSearchSubClass.Key).Add(rType);
                        }
                    }
                }
            }
        }
        protected T GetNoPublicField<T>(Type rType, string name, T rDefault = default(T))
        {
            var rFieldInfo = rType.GetField(name, BindingFlags.Static|BindingFlags.NonPublic);
            //Console.WriteLine(rFieldInfo);
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
            //Debugger.ValidValueE(rType, "TypeSearchBase.GetTypes => rType is empty!");
            var rPropertyInfo = GetStorePropertyInfo(rType, "Types");
            //Debugger.ValidValueE(rPropertyInfo, "TypeSearchBase.GetTypes => rType is invalid!");

            return rPropertyInfo.GetValue(null, null) as List<Type>;
        }
        public static List<string> GetTypeFullNames(Type rType)
        {
            //Debugger.ValidValueE(rType, "TypeSearchBase.GetTypeFullNames => rType is empty!");
            var rPropertyInfo = GetStorePropertyInfo(rType, "TypeFullNames");
            //Debugger.ValidValueE(rPropertyInfo, "TypeSearchBase.GetTypeFullNames => rType is invalid!");

            return rPropertyInfo.GetValue(null, null) as List<string>;
        }
        public static List<string> GetTypeNames(Type rType)
        {
            //Debugger.ValidValueE(rType, "TypeSearchBase.GetTypeNames => rType is empty!");
            var rPropertyInfo = GetStorePropertyInfo(rType, "TypeNames");
            //Debugger.ValidValueE(rPropertyInfo, "TypeSearchBase.GetTypeNames => rType is invalid!");

            return rPropertyInfo.GetValue(null, null) as List<string>;
        }
        public static PropertyInfo GetStorePropertyInfo(Type rType, string rStorePropertyName)
        {
            //Debugger.ValidValueE(rType,             "TypeSearchBase.GetStorePropertyInfo => rType is empty!");
            //Debugger.ValidValueE(rStorePropertyName,"TypeSearchBase.GetStorePropertyInfo => rStorePropertyName is empty!");

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