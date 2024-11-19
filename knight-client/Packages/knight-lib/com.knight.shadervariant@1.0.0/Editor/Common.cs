using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace Knight.Framework.ShaderVariant.Editor
{

    public static class EditorUtils
    {

        public readonly static String ProjectDirectory = String.Empty;

        static EditorUtils()
        {
            ProjectDirectory = EditorUtils.GetProjectUnityRootPath();
        }

        public class TaskAction
        {
            public String name;
            public Action action;
            public bool breakAble = true;
            public TaskAction(String _name, Action _action, bool _breakAble = true)
            {
                name = _name;
                action = _action;
                breakAble = _breakAble;
            }
        }

        public class TaskActions : List<TaskAction>
        {
            public void Add(String name, Action action, bool breakAble = true)
            {
                base.Add(new TaskAction(name, action, breakAble));
            }
        }

        public class EditorTaskInfo
        {
            internal static List<EditorTaskInfo> _allTasks = new List<EditorTaskInfo>();

            internal String _name;
            internal int _totalTaskCount = 0;
            internal int _curTaskCount = 0;
            internal String _curTask = String.Empty;
            internal bool _isWorking = false;
            internal bool _done = false;
            internal TaskActions _taskList = null;
            internal bool _breakIfException = false;

            public String name
            {
                get
                {
                    return _name;
                }
            }
            public bool isWorking
            {
                get
                {
                    return _isWorking;
                }
            }

            public EditorTaskInfo(String name)
            {
                _name = name;
            }

            public bool done
            {
                get
                {
                    return _done;
                }
            }

            public void StopTask()
            {
                if (_taskList != null)
                {
                    _taskList.Clear();
                }
            }

            public static void StopAllTasks()
            {
                for (int i = 0; i < _allTasks.Count; ++i)
                {
                    _allTasks[i].StopTask();
                }
            }

            public bool Append(String name, Action action)
            {
                if (_taskList != null)
                {
                    if (_totalTaskCount > 0)
                    {
                        ++_totalTaskCount;
                        _taskList.Insert(0, new TaskAction(name, action));
                        return true;
                    }
                }
                return false;
            }
        }

        public static EditorTaskInfo DoEditorTask(String title, TaskActions _taskList, bool breakIfException = false)
        {
            if (_taskList == null || _taskList.Count == 0)
            {
                return null;
            }
            title = title ?? "unknown editor task";
            var taskInfo = new EditorTaskInfo(title);
            EditorApplication.CallbackFunction tick = null;
            // we handle task item from the end of list
            _taskList.Reverse();
            int totalTaskCount = _taskList.Count;
            taskInfo._breakIfException = breakIfException;
            taskInfo._totalTaskCount = totalTaskCount;
            if (totalTaskCount > 0)
            {
                taskInfo._taskList = _taskList;
                EditorTaskInfo._allTasks.Add(taskInfo);
                tick = () =>
                {
                    if (_taskList.Count > 0)
                    {
                        var item = _taskList[_taskList.Count - 1];
                        _taskList.RemoveAt(_taskList.Count - 1);
                        taskInfo._curTaskCount = _taskList.Count;
                        taskInfo._curTask = item.name;
                        Debug.LogFormat("DoEditorTask: {0}", item.name);
                        if (EditorUtility.DisplayCancelableProgressBar(title, item.name, (totalTaskCount - _taskList.Count) / (float)totalTaskCount))
                        {
                            _taskList.RemoveAll(_e => _e.breakAble);
                            return;
                        }
                        try
                        {
                            if (item.action != null)
                            {
                                item.action();
                            }
                        }
                        catch (Exception e)
                        {
                            if (taskInfo._breakIfException)
                            {
                                _taskList.RemoveAll(_e => _e.breakAble);
                            }
                            Debug.LogErrorFormat("Process {0} failed: {1}", item.name, e.ToString());
                        }
                    }
                    else
                    {
                        EditorApplication.update -= tick;
                        taskInfo._isWorking = false;
                        taskInfo._totalTaskCount = 0;
                        taskInfo._curTask = String.Empty;
                        EditorUtility.ClearProgressBar();
                        if (taskInfo._curTaskCount == 0)
                        {
                            Debug.LogFormat(title + " done. total: {0}", totalTaskCount);
                        }
                        else
                        {
                            Debug.LogFormat(title + " canceled. total: {0}, done:{1}, remain: {2}", totalTaskCount, (totalTaskCount - taskInfo._curTaskCount), taskInfo._curTaskCount);
                        }
                        taskInfo._done = true;
                        taskInfo._curTaskCount = 0;
                        EditorTaskInfo._allTasks.Remove(taskInfo);
                    }
                };
                EditorApplication.update += tick;
            }
            return taskInfo;
        }

        public static String RelateToAssetsPath(String path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return "Assets" + path.Substring(Application.dataPath.Length);
            }
            return path;
        }

        public static bool IsUnityDefaultResource(String path)
        {
            return String.IsNullOrEmpty(path) == false &&
                (path == "Resources/unity_builtin_extra" ||
                path == "Library/unity default resources");
        }

        public static List<String> GetAllSelectedPath()
        {
            var paths = new List<String>();
            var sel = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            foreach (var obj in sel)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    paths.Add(path);
                }
            }
            return paths;
        }

        public static List<String> GetSelectedAssetPathList(Type type, Func<String, bool> fileFilter)
        {
            var allSelected = new HashSet<String>();
            var dirs = new HashSet<String>();
            var folders = EditorUtils.GetAllSelectedPath();
            if (folders.Count > 0)
            {
                Func<String, bool> _fileFilter = s =>
                {
                    if (s.EndsWith(".meta"))
                    {
                        return false;
                    }
                    return fileFilter == null || fileFilter(s);
                };
                for (int i = 0; i < folders.Count; ++i)
                {
                    var _f = FileUtils.GetFileList(folders[i], _fileFilter);
                    for (int j = 0; j < _f.Count; ++j)
                    {
                        var assetPath = EditorUtils.RelateToAssetsPath(_f[j]);
                        var dir = System.IO.Path.GetDirectoryName(assetPath);
                        dirs.Add(dir);
                        allSelected.Add(assetPath);
                    }
                }
            }
            UnityEngine.Object[] objs = null;
            if (type != null)
            {
                objs = Selection.objects.Where(
                    o =>
                    {
                        return type.IsInstanceOfType(o) && !String.IsNullOrEmpty(AssetDatabase.GetAssetPath(o));
                    }
                ).ToArray();
            }
            else
            {
                objs = Selection.objects;
            }
            for (int i = 0; i < objs.Length; ++i)
            {
                var assetPath = AssetDatabase.GetAssetPath(objs[i]);
                if (!String.IsNullOrEmpty(assetPath) && (fileFilter == null || fileFilter(assetPath)) && !assetPath.EndsWith(".meta"))
                {
                    var dir = System.IO.Path.GetDirectoryName(assetPath);
                    if (folders.Count == 0 || folders.Contains(dir))
                    {
                        // only assets in our selected directories
                        allSelected.Add(assetPath);
                    }
                }
            }
            return allSelected.ToList();
        }

        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            if (type == null)
            {
                type = assembly.GetType("UnityEditor.LogEntries");
            }
            var method = type.GetMethod("Clear");
            var o = method.Invoke(null, null);
            if (o != null)
            {
                Debug.Log(o);
            }
        }

        public static void ShowDebugConsole()
        {
            Debug.developerConsoleVisible = true;
            UnityEditor.EditorApplication.ExecuteMenuItem("Window/General/Console");
        }

        public static String GetProjectRootPath()
        {
            var curdir = Environment.CurrentDirectory.Replace('\\', '/');
            var rootPath = curdir + "/..";
            if (Directory.Exists(rootPath))
            {
                rootPath = Path.GetFullPath(rootPath);
                return rootPath.Replace('\\', '/');
            }
            else
            {
                return rootPath;
            }
        }

        public static String GetProjectUnityRootPath()
        {
            var rootPath = Environment.CurrentDirectory.Replace('\\', '/');
            if (Directory.Exists(rootPath))
            {
                rootPath = Path.GetFullPath(rootPath);
                return rootPath.Replace('\\', '/');
            }
            else
            {
                return rootPath;
            }
        }

        public static String GetProjectUnityDataPath()
        {
            var rootPath = Environment.CurrentDirectory.Replace('\\', '/');
            rootPath += "/Assets";
            if (Directory.Exists(rootPath))
            {
                rootPath = Path.GetFullPath(rootPath);
                return rootPath.Replace('\\', '/');
            }
            else
            {
                return rootPath;
            }
        }

        public static FileStream FileOpenRead(String filePath)
        {
            if (!System.IO.Path.IsPathRooted(filePath))
            {
                var _ProjectDirectory = ProjectDirectory;
                if (String.IsNullOrEmpty(_ProjectDirectory))
                {
                    _ProjectDirectory = EditorUtils.GetProjectUnityRootPath();
                }
                filePath = _ProjectDirectory + "/" + filePath;
                filePath = filePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }
            return File.OpenRead(filePath);
        }

        public static String[] ReadFileLines(String filePath)
        {
            String[] ret = null;
            if (!System.IO.Path.IsPathRooted(filePath))
            {
                var ProjectDirectory = EditorUtils.GetProjectUnityRootPath();
                var _ProjectDirectory = ProjectDirectory;
                if (String.IsNullOrEmpty(_ProjectDirectory))
                {
                    _ProjectDirectory = EditorUtils.GetProjectUnityRootPath();
                }
                filePath = _ProjectDirectory + "/" + filePath;
                filePath = filePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }
            try
            {
                ret = File.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return ret;
        }

        static String _Md5Asset(String filePath,
            MD5CryptoServiceProvider md5Service,
            byte[] buffer, StringBuilder sb)
        {
            try
            {
                int bytesRead = 0;
                using (var file = FileOpenRead(filePath))
                {
                    while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        md5Service.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    }
                }
                var meta = filePath + ".meta";
                if (File.Exists(meta))
                {
                    var lines = ReadFileLines(meta);
                    var idx = lines.FindIndex("timeCreated:", (name, tag) => name.StartsWith(tag));
                    if (idx != -1)
                    {
                        var _lines = lines.ToList();
                        _lines.RemoveAt(idx);
                        lines = _lines.ToArray();
                    }
                    var content = String.Join("\n", lines);
                    var bytes = Encoding.ASCII.GetBytes(content);
                    md5Service.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
                }
                md5Service.TransformFinalBlock(buffer, 0, 0);
                var hashBytes = md5Service.Hash;
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return String.Empty;
        }

        public static String Md5Asset(String filePath)
        {
            if (!File.Exists(filePath))
            {
                return String.Empty;
            }
            const int chunkSize = 10240;
            var _MD5Service = new MD5CryptoServiceProvider();
            var buffer = new byte[chunkSize];
            return _Md5Asset(filePath, _MD5Service, buffer, new StringBuilder());
        }

        public static String Md5File(String filePath)
        {
            try
            {
                using (var stream = new BufferedStream(FileOpenRead(filePath), 4096))
                {
                    var _MD5Service = new MD5CryptoServiceProvider();
                    var hashBytes = _MD5Service.ComputeHash(stream);
                    var sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return String.Empty;
        }

        public static String Md5String(String str)
        {
            var bytes = UTF8Encoding.Default.GetBytes(str);
            var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool DisplayCancelableProgressBarWithTimeout(String title, String info, int timeOutMS)
        {
            float start = Utils.GetSystemTicksMS();
            var now = start;
            try
            {
                for (; ; )
                {
                    if (now - start <= timeOutMS)
                    {
                        var p = (float)(timeOutMS - (now - start)) / timeOutMS;
                        if (EditorUtility.DisplayCancelableProgressBar(title, info, p))
                        {
                            return true;
                        }
                        now = Utils.GetSystemTicksMS();
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    break;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            return false;
        }

        public static bool FileFilter_prefab(String fileName)
        {
            return fileName.EndsWith(".prefab", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsEmptyShader(Shader shader)
        {
            return shader == null ||
                shader.name == "Hidden/InternalErrorShader" ||
                shader.name == "Hidden/Built-in/InternalErrorShader";
        }
    }

    public struct GUISetEnabled : IDisposable
    {

        public GUISetEnabled(bool enabled)
        {
            this.PreviousEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        public void Dispose()
        {
            GUI.enabled = this.PreviousEnabled;
        }

        [SerializeField]
        private bool PreviousEnabled;
    }

    public class CustomMessageBox : EditorWindow
    {

        public delegate void OnWindowClose(int button, int returnValue);
        public String Info = String.Empty;
        public Func<int> OnGUIFunc = null;
        public Action<int>[] OnButtonClicks = null;
        public OnWindowClose OnClose = null;
        public String[] Buttons = null;
        public int ReturnValue = 0;
        int m_closeButton = -1;

        public void OnDestroy()
        {
            if (OnClose != null)
            {
                try
                {
                    OnClose(m_closeButton, ReturnValue);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (!string.IsNullOrEmpty(Info))
            {
                EditorGUILayout.HelpBox(Info, MessageType.None);
            }
            EditorGUILayout.Space();
            if (OnGUIFunc != null)
            {
                ReturnValue = OnGUIFunc();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (Buttons != null)
            {
                for (int i = 0; i < Buttons.Length; ++i)
                {
                    if (GUILayout.Button(Buttons[i], GUILayout.MinWidth(80)))
                    {
                        m_closeButton = i;
                        if (OnButtonClicks != null)
                        {
                            if (i >= 0 && i <= OnButtonClicks.Length && OnButtonClicks[i] != null)
                            {
                                try
                                {
                                    OnButtonClicks[i](ReturnValue);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            }
                        }
                        EditorApplication.delayCall += () =>
                        {
                            Close();
                        };
                    }
                    GUILayout.Space(5);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }

    public static class LevelEditor
    {
        public const String BlackholeScenePath = "Packages/com.knight.shadervariant/Runtime/LightEnvs/Off.unity";
        public static void RefreshBuildScenes()
        {
            // TODO:
        }
    }
}
//EOF
