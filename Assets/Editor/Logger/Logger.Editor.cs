//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using System.IO;

namespace Core.Editor
{
    public class LoggerEditor
    {
        [OnOpenAssetAttribute(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string rStackTrace = GetStackTrace();
            return AnalysisStackTrace(rStackTrace);
        }

        private static string GetStackTrace()
        {
            var rConsoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo rFieldInfo = rConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var rConsoleWindow = rFieldInfo.GetValue(null) as EditorWindow;
            var rConsoleWindowInstance = rFieldInfo.GetValue(null);

            if (rConsoleWindowInstance != null)
            {
                if ((object)EditorWindow.focusedWindow == rConsoleWindowInstance)
                {
                    var rListViewStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");

                    rFieldInfo = rConsoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
                    var listView = rFieldInfo.GetValue(rConsoleWindowInstance);

                    rFieldInfo = rListViewStateType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
                    int row = (int)rFieldInfo.GetValue(listView);

                    rFieldInfo = rConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    string activeText = rFieldInfo.GetValue(rConsoleWindowInstance).ToString();

                    return activeText;
                }
            }

            return string.Empty;
        }

        private static bool AnalysisStackTrace(string rStackTraceStr)
        {
            if (string.IsNullOrEmpty(rStackTraceStr)) return false;
            string[] rStacks = rStackTraceStr.Split('\n');

            string rTracePath = string.Empty;
            for (int i = 0; i < rStacks.Length; i++)
            {
                string rLineLogStr = rStacks[i].Trim();
                if (string.IsNullOrEmpty(rLineLogStr)) continue;

                if (!rLineLogStr.Contains("(at Assets/Plugins/Core/Logger/Logger.cs:")) continue;
                if (i + 1 < rStacks.Length)
                {
                    rTracePath = rStacks[i + 1].Trim();
                    break;
                }
            }
            Match rRegexMatch = Regex.Match(rTracePath, @"\(at\s([^\{^\}]*)(:\d*)\)");
            rTracePath = rRegexMatch.Value;
            rTracePath = rTracePath.Replace("(at ", "").Replace(")", "").Trim();
            if (string.IsNullOrEmpty(rTracePath)) return false;

            string[] rTraceArgs = rTracePath.Split(':');
            if (rTraceArgs.Length != 2) return false;

            string rPath = rTraceArgs[0].Trim().Replace("\\\\", "");
            int nLine = 0;
            int.TryParse(rTraceArgs[1], out nLine);

            if (File.Exists(rPath))
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(rPath, nLine);

            return true;
        }
    }
}
