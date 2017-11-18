//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using Debug = UnityEngine.Debug;

namespace Core
{
    public class Logger
    {
        public static void Info(string rMsg)
        {
            Debug.Log(rMsg);
        }

        public static void Warn(string rMsg)
        {
            Debug.LogWarning(rMsg);
        }

        public static void Error(string rMsg)
        {
            Debug.LogError(rMsg);
        }

        public static void Break()
        {
            Debug.Break();
        }

        public static void Exception(Exception rException)
        {
            Debug.LogException(rException);
        }
    }
}
