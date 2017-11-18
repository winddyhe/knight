//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TSIgnoreAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TSIgnoreInheritedAttribute : TSIgnoreAttribute {}

	[AttributeUsage(AttributeTargets.Field)]
	public class TypeSearchAttribute : Attribute
    {
		public Type TypeSearchType;
		
		public TypeSearchAttribute(Type rTypeSearch)
		{
			TypeSearchType = rTypeSearch;
		}

        public List<string> TypeFullNames
        {
            get
            {
                return TypeSearchBase.GetTypeFullNames(TypeSearchType);
            }
        }
	}
    /// <summary>
    /// TypeSearchCore
    /// </summary>
    public class TypeSearchCore : TSingleton<TypeSearchCore>
    {
        public List<Type> GetSubClasses(Type rType)
        {
            if (!mSearchTypes.ContainsKey(rType))
                return new List<Type>();

            return (List<Type>)mSearchTypes[rType];
        }

        private TypeSearchCore()
        {
            var rTypeSearchSubClasses = new List<KeyValuePair<Type, Type>>();
            foreach (var rAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var rType in rAssembly.GetTypes())
                {
                    if (typeof(TypeSearchBase).IsAssignableFrom(rType) &&
                        typeof(TypeSearchFull<,>) != rType &&
                        typeof(TypeSearchDefault<>) != rType &&
                        typeof(TypeSearchBase) != rType )
                    {
                        var rSearchType = GetNoPublicField<Type>(rType.SearchBaseTo(typeof(TypeSearchBase)), "_Type");
                        var rIgnoreType = GetNoPublicField<Type>(rType.SearchBaseTo(typeof(TypeSearchBase)), "_IgnoreAttributeType");
                        if (null != rSearchType && null != rIgnoreType)
                            rTypeSearchSubClasses.Add(new KeyValuePair<Type, Type>(rSearchType as Type, rIgnoreType));
                    }
                }
            }
            foreach (var rAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var rType in rAssembly.GetTypes())
                {
                    
                    foreach(var rTypeSearchSubClass in rTypeSearchSubClasses)
                    {
                        if (rTypeSearchSubClass.Key.IsAssignableFrom(rType) && 
                            !rType.IsApplyAttr(rTypeSearchSubClass.Value, true))
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

    public class TypeSearchBase
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
            return rType.SearchBaseTo<TypeSearchBase>().GetProperty(rStorePropertyName,
                BindingFlags.Static | BindingFlags.Public);
        }
    }
    /// <summary>
    /// TypeSearch
    ///     不支持抽象类
    /// </summary>
    public class TypeSearchFull<TSearchType, TIgnoreType> : TypeSearchBase
        where TIgnoreType : System.Attribute
    {
        public static List<Type> Types
        {
            get
            {
                if (null == GTypes)
                    GTypes = TypeSearchCore.Instance.GetSubClasses(_Type);
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

    public class TypeSearchDefault<TSearchType> : TypeSearchFull<TSearchType, TSIgnoreAttribute> { }

}