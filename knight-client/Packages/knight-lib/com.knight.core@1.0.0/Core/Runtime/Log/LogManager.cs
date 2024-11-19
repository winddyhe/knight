using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Core
{
    public class LogManager
    {
        [System.Diagnostics.Conditional("ENABLE_LOG_RELEASE")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogRelease(object text)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#00EEEE>{text}</color>");
#else
            Debug.Log(text);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ASSERT")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_LOG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void Log(object text)
        {
            Debug.Log(text);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_LOG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void Log(object message, UnityEngine.Object context)
        {
            Debug.Log(message, context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_LOG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_WARNING")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogWarning(object text)
        {
            Debug.LogWarning(text);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_WARNING")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogWarning(object text, UnityEngine.Object rAsset)
        {
            Debug.LogWarning(text);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_WARNING")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ERROR")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogError(object text)
        {
            Debug.LogError(text);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ERROR")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogError(object message, UnityEngine.Object context)
        {
            Debug.LogError(message, context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ERROR")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogErrorFormat(string text, params object[] args)
        {
            Debug.LogErrorFormat(text, args);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ERROR")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogException(Exception e, UnityEngine.Object context)
        {
            Debug.LogException(e, context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_ERROR")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        public static void LogException(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
