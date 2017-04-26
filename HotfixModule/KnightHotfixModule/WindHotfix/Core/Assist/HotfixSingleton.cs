//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Reflection;

namespace WindHotfix.Core
{
    public class THotfixSingleton<T> where T : class
    {
        private static object           __sync_root = new object();
        private static T                __instance;

        public static readonly Type[]   EmptyTypes = new Type[0];

        public static T                 Instance
        {
            get
            {
                if (__instance == null)
                {
                    lock (__sync_root)
                    {
                        if (__instance == null)
                        {
                            ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, EmptyTypes, null);
                            if (ci == null) { throw new InvalidOperationException("class must contain a private constructor"); }
                            __instance = (T)ci.Invoke(null);
                        }
                    }
                }
                return __instance;
            }
        }
    }

    public class HotfixSingleton<T> where T : new()
    {
        private static T            __instance;
        private static object       __lock = new object();

        private HotfixSingleton()
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
