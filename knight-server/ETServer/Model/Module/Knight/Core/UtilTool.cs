//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;

namespace Knight.Core
{
    public static class UtilTool
    {
        public static void SafeExecute(Action rAction)
        {
            if (rAction != null) rAction();
        }

        public static void SafeExecute<T>(Action<T> rAction, T rObj)
        {
            if (rAction != null) rAction(rObj);
        }

        public static void SafeExecute<T1, T2>(Action<T1, T2> rAction, T1 rObj1, T2 rObj2)
        {
            if (rAction != null) rAction(rObj1, rObj2);
        }

        public static void SafeExecute<T1, T2, T3>(Action<T1, T2, T3> rAction, T1 rObj1, T2 rObj2, T3 rObj3)
        {
            if (rAction != null) rAction(rObj1, rObj2, rObj3);
        }

        public static void SafeExecute<T1, T2, T3, T4>(Action<T1, T2, T3, T4> rAction, T1 rObj1, T2 rObj2, T3 rObj3, T4 rObj4)
        {
            if (rAction != null) rAction(rObj1, rObj2, rObj3, rObj4);
        }

        public static TResult SafeExecute<TResult>(Func<TResult> rFunc)
        {
            if (rFunc == null) return default(TResult);
            return rFunc();
        }

        public static TResult SafeExecute<T, TResult>(Func<T, TResult> rFunc, T rObj)
        {
            if (rFunc == null) return default(TResult);
            return rFunc(rObj);
        }

        public static TResult SafeExecute<T1, T2, TResult>(Func<T1, T2, TResult> rFunc, T1 rObj1, T2 rObj2)
        {
            if (rFunc == null) return default(TResult);
            return rFunc(rObj1, rObj2);
        }

        public static TResult SafeExecute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> rFunc, T1 rObj1, T2 rObj2, T3 rObj3)
        {
            if (rFunc == null) return default(TResult);
            return rFunc(rObj1, rObj2, rObj3);
        }

        public static TResult SafeExecute<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> rFunc, T1 rObj1, T2 rObj2, T3 rObj3, T4 rObj4)
        {
            if (rFunc == null) return default(TResult);
            return rFunc(rObj1, rObj2, rObj3, rObj4);
        }

        public static string PathCombine(char rDirectoryChar, params string[] rPaths)
        {
            var rReplaceChar = rDirectoryChar == '\\' ? '/' : '\\';
            if (rPaths.Length == 0)
                return string.Empty;

            var rFirstPath = rPaths[0].Replace(rReplaceChar, rDirectoryChar);
            if (rFirstPath.Length > 0 && rFirstPath[rFirstPath.Length - 1] == rDirectoryChar)
                rFirstPath = rFirstPath.Substring(0, rFirstPath.Length - 1);

            var rBuilder = new StringBuilder(rFirstPath);
            for (int nIndex = 1; nIndex < rPaths.Length; ++nIndex)
            {
                if (string.IsNullOrEmpty(rPaths[nIndex]))
                    continue;

                var rPath = rPaths[nIndex].Replace(rReplaceChar, rDirectoryChar);
                if (rPath[0] != rDirectoryChar)
                    rPath = rDirectoryChar + rPath;
                if (rPath[rPath.Length - 1] == rDirectoryChar)
                    rPath = rPath.Substring(0, rPath.Length - 1);
                rBuilder.Append(rPath);
            }

            return rBuilder.ToString();
        }

        public static string PathCombine(params string[] rPaths)
        {
            return PathCombine('/', rPaths);
        }

        public static string GetParentPath(string rPath)
        {
            return Path.GetDirectoryName(rPath);
        }
        
        public static void WriteAllText(string rPath, string rContents)
        {
            string rDir = Path.GetDirectoryName(rPath);
            if (!Directory.Exists(rDir)) Directory.CreateDirectory(rDir);
            File.WriteAllText(rPath, rContents);
        }

        public static void WriteAllText(string rPath, string rContents, Encoding rEncoding)
        {
            string rDir = Path.GetDirectoryName(rPath);
            if (!Directory.Exists(rDir)) Directory.CreateDirectory(rDir);
            File.WriteAllText(rPath, rContents, rEncoding);
        }

        public static void WriteAllBytes(string rPath, byte[] rBytes)
        {
            string rDir = Path.GetDirectoryName(rPath);
            if (!Directory.Exists(rDir)) Directory.CreateDirectory(rDir);
            File.WriteAllBytes(rPath, rBytes);
        }
    }
}