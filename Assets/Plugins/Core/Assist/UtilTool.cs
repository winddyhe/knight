//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core
{
    public static class UtilTool
    {
        public static readonly string SessionSecrect = "pomelo_session_secret_winddy";

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

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

        public static float WrapAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        public static byte[] GetMD5(string rContentFile)
        {
            return GetMD5(new List<string>() { rContentFile });
        }

        public static byte[] GetMD5(List<string> rContentFiles)
        {
            rContentFiles.Sort((a1, a2) => { return a1.CompareTo(a2); });

            HashAlgorithm rHasAlgo = HashAlgorithm.Create("MD5");
            byte[] rHashValue = new byte[20];

            byte[] rTempBuffer = new byte[4096];
            int rTempCount = 0;
            for (int i = 0; i < rContentFiles.Count; i++)
            {
                if (File.Exists(rContentFiles[i]))
                {
                    FileStream fs = File.OpenRead(rContentFiles[i]);
                    while (fs.Position != fs.Length)
                    {
                        rTempCount += fs.Read(rTempBuffer, 0, 4096 - rTempCount);
                        if (rTempCount == 4096)
                        {
                            if (rHasAlgo.TransformBlock(rTempBuffer, 0, rTempCount, null, 0) != 4096)
                                Debug.LogError("TransformBlock error.");
                            rTempCount = 0;
                        }
                    }
                    fs.Close();
                }
            }
            rHasAlgo.TransformFinalBlock(rTempBuffer, 0, rTempCount);
            rHashValue = rHasAlgo.Hash;
            return rHashValue;
        }

        public static string ToHEXString(this byte[] rSelf)
        {
            var rText = new StringBuilder();
            for (int nIndex = 0; nIndex < rSelf.Length; ++nIndex)
                rText.AppendFormat("{0:X}", rSelf[nIndex]);
            return rText.ToString();
        }

        public static string HashAlgorithmByString(string rText, string rHashName, Encoding rEncoding)
        {
            var rHashAlgorithm = HashAlgorithm.Create(rHashName);
            var rTextBytes = rEncoding.GetBytes(rText);
            rHashAlgorithm.TransformFinalBlock(rTextBytes, 0, rTextBytes.Length);
            return rHashAlgorithm.Hash.ToHEXString();
        }

        public static string GetMD5String(string rText, Encoding rEncoding)
        {
            return HashAlgorithmByString(rText, "MD5", rEncoding);
        }

        public static string GetMD5String(string rText)
        {
            return GetMD5String(rText, Encoding.Default);
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

        public static GameObject CreateGameObject(string rName, params Type[] rComps)
        {
            GameObject rGo = new GameObject(rName, rComps);

            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(GameObject rParentGo, string rName, params Type[] rComps)
        {
            GameObject rGo = new GameObject(rName, rComps);
            rGo.transform.parent = rParentGo.transform;

            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(GameObject rTemplateGo)
        {
            GameObject rGo = GameObject.Instantiate(rTemplateGo);

            rGo.name = rTemplateGo.name;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static GameObject CreateGameObject(GameObject rTemplateGo, GameObject rParentGo)
        {
            GameObject rGo = GameObject.Instantiate(rTemplateGo);
            rGo.transform.parent = rParentGo.transform;

            rGo.name = rTemplateGo.name;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            return rGo;
        }

        public static void SafeDestroy(UnityEngine.Object rObj)
        {
            if (rObj != null)
                GameObject.DestroyImmediate(rObj, true);
            rObj = null;
        }

        public static void SetLayer(GameObject rObj, string rLayerName, bool bIsIncludeChildren = false)
        {
            int nLayer = LayerMask.NameToLayer(rLayerName);
            SetLayer(rObj, nLayer, bIsIncludeChildren);
        }

        public static void SetLayer(GameObject rObj, int nLayer, bool bIsIncludeChildren)
        {
            rObj.layer = nLayer;
            if (bIsIncludeChildren)
            {
                int nChildNum = rObj.transform.childCount;
                for (int i = 0; i < nChildNum; i++)
                {
                    var rChildObj = rObj.transform.GetChild(i).gameObject;
                    SetLayer(rChildObj, nLayer, bIsIncludeChildren);
                }
            }
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

        public static string GetTransformPath(Transform rTrans)
        {
            string rPath = "";
            GetTransformPath(rTrans, ref rPath);
            return rPath;
        }

        public static void GetTransformPath(Transform rTrans, ref string rPath)
        {
            if (rTrans == null || rTrans.parent == null) return;

            rPath = rTrans.name + (string.IsNullOrEmpty(rPath) ? rPath : "/" + rPath);
            GetTransformPath(rTrans.parent, ref rPath);
        }
    }
}