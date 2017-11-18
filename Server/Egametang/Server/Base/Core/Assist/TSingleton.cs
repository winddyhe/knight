//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Reflection;

namespace Core
{
    public class TSingleton<T> where T : class
    {
        static object   SyncRoot = new object();
        static T        instance;

        public static readonly Type[]   EmptyTypes = new Type[0];

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, EmptyTypes, null);
                            if (ci == null) { throw new InvalidOperationException("class must contain a private constructor"); }
                            instance = (T)ci.Invoke(null);
                        }
                    }
                }
                return instance;
            }
        }
    }

    public class Singleton<T> where T : new()
    {
        private static T __instance;
        private static object __lock = new object();

        private Singleton()
        {
        }

        public static T GetInstance()
        {
            if (__instance == null)
            {
                lock (__lock)
                {
                    if (__instance == null)
                    {
                        __instance = new T();
                    }
                }
            }
            return __instance;
        }
    }
}

