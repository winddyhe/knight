using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Knight.Framework.ShaderVariant
{
    public static class Utils
    {

        static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static long _epochTicks = _epoch.Ticks;
        static Stopwatch _globalTimer = new Stopwatch();
        static long _startupTicksMS = NowUnixTimeMS();
        public readonly static object[] boxedEmpty = new object[] { };

        static Utils()
        {
            _startupTicksMS = NowUnixTimeMS();
            _globalTimer.Start();
        }

        public static long NowUnixTimeMS()
        {
            return (DateTime.UtcNow.Ticks - _epochTicks) / TimeSpan.TicksPerMillisecond;
        }

        public static int GetSystemTicksMS()
        {
            // TickCount cycles between Int32.MinValue, which is a negative 
            // number, and Int32.MaxValue once every 49.8 days. This sample
            // removes the sign bit to yield a nonnegative number that cycles 
            // between zero and Int32.MaxValue once every 24.9 days.
            return (int)_globalTimer.ElapsedMilliseconds;
        }

        public static Type FindTypeInAssembly(String typeName, Assembly assembly = null)
        {
            Type type = null;
            if (assembly == null)
            {
                type = Type.GetType(typeName, false);
            }
            if (type == null && assembly != null)
            {
                var types = assembly.GetTypes();
                for (int j = 0; j < types.Length; ++j)
                {
                    var b = types[j];
                    if (b.FullName == typeName)
                    {
                        type = b;
                        break;
                    }
                }
            }
            if (type == null)
            {
                Debug.LogWarningFormat("FindType( \"{0}\", \"{1}\" ) failed!",
                    typeName, assembly != null ? assembly.FullName : "--");
            }
            return type;
        }

        public static Type FindType(String typeName, String assemblyName = null)
        {
            Type type = null;
            try
            {
                if (String.IsNullOrEmpty(assemblyName))
                {
                    type = Type.GetType(typeName, false);
                }
                if (type == null)
                {
                    var asm = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < asm.Length; ++i)
                    {
                        var a = asm[i];
                        if (String.IsNullOrEmpty(assemblyName) || a.GetName().Name == assemblyName)
                        {
                            var types = a.GetTypes();
                            for (int j = 0; j < types.Length; ++j)
                            {
                                var b = types[j];
                                if (b.FullName == typeName)
                                {
                                    type = b;
                                    goto END;
                                }
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (Exception inner in ex.LoaderExceptions)
                {
                    Debug.LogError(inner.Message);
                }
            }
        END:
            if (type == null)
            {
                Debug.LogWarningFormat("FindType( \"{0}\", \"{1}\" ) failed!",
                    typeName, assemblyName ?? String.Empty);
            }
            return type;
        }

        public static object RflxGetValue(String typeName, String memberName, String assemblyName = null)
        {
            object value = null;
            var type = FindType(typeName, assemblyName);
            if (type != null)
            {
                var smembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0, count = smembers.Length; i < count && value == null; ++i)
                {
                    var m = smembers[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        var pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            value = pi.GetValue(null, null);
                        }
                        else
                        {
                            var fi = m as FieldInfo;
                            if (fi != null)
                            {
                                value = fi.GetValue(null);
                            }
                        }
                    }
                }
            }
            if (value == null)
            {
                Debug.LogErrorFormat("RflxGetValue( \"{0}\", \"{1}\", \"{2}\" ) failed!",
                    typeName, memberName, assemblyName ?? String.Empty);
            }
            return value;
        }

        public static bool RflxSetValue(String typeName, String memberName, object value, String assemblyName = null)
        {
            var type = FindType(typeName, assemblyName);
            if (type != null)
            {
                var smembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0; i < smembers.Length; ++i)
                {
                    var m = smembers[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        var pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            pi.SetValue(null, value, null);
                            return true;
                        }
                        else
                        {
                            var fi = m as FieldInfo;
                            if (fi != null)
                            {
                                if (fi.IsLiteral == false && fi.IsInitOnly == false)
                                {
                                    fi.SetValue(null, value);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            Debug.LogErrorFormat("RflxSetValue( \"{0}\", \"{1}\", {2}, \"{3}\" ) failed!",
                typeName, memberName, value != null ? value : "null", assemblyName ?? String.Empty);
            return false;
        }

        public static object RflxGetValue(Type type, String memberName, object owner = null)
        {
            object value = null;
            if (type != null || owner != null)
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic;
                if (type == null)
                {
                    type = owner.GetType();
                    flags |= BindingFlags.Instance;
                }
                else
                {
                    owner = null;
                    flags |= BindingFlags.Static;
                }
                var smembers = type.GetMembers(flags);
                for (int i = 0, count = smembers.Length; i < count && value == null; ++i)
                {
                    var m = smembers[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        var pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            value = pi.GetValue(owner, null);
                        }
                        else
                        {
                            var fi = m as FieldInfo;
                            if (fi != null)
                            {
                                value = fi.GetValue(owner);
                            }
                        }
                    }
                }
            }
            if (value == null)
            {
                Debug.LogErrorFormat("RflxGetValue( \"{0}\", \"{1}\", \"{2}\" ) failed!",
                    type != null ? type.FullName : "null", memberName, owner ?? "null");
            }
            return value;
        }

        public static bool RflxSetValue(Type type, String memberName, object value, object owner = null)
        {
            if (type != null || owner != null)
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic;
                if (type == null)
                {
                    type = owner.GetType();
                    flags |= BindingFlags.Instance;
                }
                else
                {
                    owner = null;
                    flags |= BindingFlags.Static;
                }
                var smembers = type.GetMembers(flags);
                for (int i = 0; i < smembers.Length; ++i)
                {
                    var m = smembers[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        var pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            pi.SetValue(owner, value, null);
                            return true;
                        }
                        else
                        {
                            var fi = m as FieldInfo;
                            if (fi != null && fi.IsLiteral == false && fi.IsInitOnly == false)
                            {
                                fi.SetValue(owner, value);
                                return true;
                            }
                        }
                    }
                }
            }
            Debug.LogErrorFormat("RflxSetValue( \"{0}\", \"{1}\", {2} ) failed!",
                type != null ? type.FullName : "null", memberName, value ?? "null", owner ?? "null");
            return false;
        }

        public static object RflxStaticCall(String typeName, String funcName, object[] parameters = null, String assemblyName = null)
        {
            var type = FindType(typeName, assemblyName);
            if (type != null)
            {
                var f = type.GetMethod(funcName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (f != null)
                {
                    var r = f.Invoke(null, parameters ?? boxedEmpty);
                    return r;
                }
            }
            Debug.LogErrorFormat("RflxStaticCall( \"{0}\", \"{1}\", {2}, \"{3}\" ) failed!",
                typeName, funcName, parameters ?? boxedEmpty, assemblyName ?? String.Empty);
            return null;
        }

        public static object RflxStaticCall(Type type, String funcName, object[] parameters = null)
        {
            if (type != null)
            {
                var f = type.GetMethod(funcName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (f != null)
                {
                    var r = f.Invoke(null, parameters ?? boxedEmpty);
                    return r;
                }
            }
            Debug.LogErrorFormat("RflxStaticCall( \"{0}\", \"{1}\", {2} ) failed!",
                type != null ? type.FullName : "null", funcName, parameters ?? boxedEmpty);
            return null;
        }

        public static object RflxCall(object owner, String funcName, object[] parameters = null)
        {
            if (owner != null)
            {
                var f = owner.GetType().GetMethod(funcName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                {
                    var r = f.Invoke(owner, parameters ?? boxedEmpty);
                    return r;
                }
                Debug.LogErrorFormat("RflxCall( \"{0}\", \"{1}\", {2} ) failed!",
                    owner.GetType().FullName, funcName, parameters ?? boxedEmpty);
            }
            return null;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = b;
            b = a;
            a = temp;
        }

        public static bool Swap<T>(IList<T> list, int a, int b)
        {
            if (a != b && a < list.Count && b < list.Count && a >= 0 && b >= 0)
            {
                var temp = list[b];
                list[b] = list[a];
                list[a] = temp;
                return true;
            }
            return false;
        }
    }

    public static class ListExtra
    {

        public static int FindIndex<T, C>(this IList<T> list, C ctx, Func<T, C, bool> match)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
            {
                if (match(list[i], ctx))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int RemoveAllEx<T, C>(List<T> list, C ctx, Func<T, C, bool> match)
        {
            var count = list.Count;
            var removeCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (match(list[i], ctx))
                {
                    var newCount = i++;
                    for (; i < count; ++i)
                    {
                        if (!match(list[i], ctx))
                        {
                            list[newCount++] = list[i];
                        }
                    }
                    removeCount = count - newCount;
                    list.RemoveRange(newCount, removeCount);
                    break;
                }
            }
            return removeCount;
        }

        public static int RemoveAll<T, C>(this List<T> list, C ctx, Func<T, C, bool> match)
        {
            return RemoveAllEx(list, ctx, match);
        }
    }

    public static class StringUtils
    {

        static bool _Char_Equal(char l, char r)
        {
            return l == r;
        }

        static bool _Char_Equal_IgnoreCase(char l, char r)
        {
            return Char.ToLower(l) == Char.ToLower(r);
        }

        delegate bool CharEqualFunc(char l, char r);

        static CharEqualFunc Char_Equal_Func = _Char_Equal;
        static CharEqualFunc Char_Equal_IgnoreCase_Func = _Char_Equal_IgnoreCase;

        static bool _MatchWildcard(String src, String wildcard, int srcOfs = 0, int wcOfs = 0, bool ignoreCase = false)
        {
            int sp = srcOfs;
            int wp = wcOfs;
            var cmp = ignoreCase ? Char_Equal_IgnoreCase_Func : Char_Equal_Func;
            // skip the obviously the same starting substring
            while (
                sp < src.Length &&
                wp < wildcard.Length &&
                wildcard[wp] != '\0' &&
                wildcard[wp] != '*' &&
                wildcard[wp] != '?')
            {
                if (cmp(src[sp], wildcard[wp]) == false)
                {
                    // must be exact match unless there's a wildcard character in the wildcard String
                    return false;
                }
                else
                {
                    ++sp;
                    ++wp;
                }
            }
            if (sp >= src.Length || src[sp] == '\0')
            {
                // this will only match if there are no non-wild characters in the wildcard
                for (; wp < wildcard.Length && wildcard[wp] != '\0'; ++wp)
                {
                    if (wildcard[wp] != '*' && wildcard[wp] != '?')
                    {
                        return false;
                    }
                }
                return true;
            }
            if (wp >= wildcard.Length)
            {
                return false;
            }
            switch (wildcard[wp])
            {
                case '\0':
                    return false; // the only way to match them after the leading non-wildcard characters is !*pString, which was already checked
                                  // we have a wildcard with wild character at the start.
                case '*':
                    {
                        // merge consecutive ? and *, since they are equivalent to a single *
                        while (wp < wildcard.Length && (wildcard[wp] == '*' || wildcard[wp] == '?'))
                        {
                            ++wp;
                        }
                        if (wp >= wildcard.Length || wildcard[wp] == '\0')
                        {
                            // the rest of the String doesn't matter: the wildcard ends with *
                            return true;
                        }
                        for (; sp < src.Length && src[sp] != '\0'; ++sp)
                        {
                            if (_MatchWildcard(src, wildcard, sp, wp, ignoreCase))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case '?':
                    return _MatchWildcard(src, wildcard, sp + 1, wp + 1, ignoreCase) || _MatchWildcard(src, wildcard, sp, wp + 1, ignoreCase);
                default:
                    return false;
            }
        }

        public static bool MatchWildcard(String src, String wildcard, bool ingoreCase = false)
        {
            return _MatchWildcard(src, wildcard, 0, 0, ingoreCase);
        }

        public static String StandardisePath(String init)
        {
            String path = init.Replace('\\', '/');
            if (path.Length > 0 && path[path.Length - 1] != '/')
            {
                path += '/';
            }
            return path;
        }
    }

    public static class UnityUtils
    {
        public static T RequireComponent<T>(GameObject go, out bool isNew) where T : Component
        {
            T ret = go.GetComponent<T>();
            if (ret == null)
            {
                isNew = true;
                ret = go.AddComponent<T>();
            }
            else
            {
                isNew = false;
            }
            return ret;
        }
    }

    public static class FileUtils
    {

        public enum TreeWalkerCmd
        {
            Continue,
            Skip,
            Exit,
        }

        public interface ITreeWalker
        {
            bool IsRecursive();
            // will be called for each file while WalkTree is running
            TreeWalkerCmd DoFile(String name);
            // will be called for each directory while WalkTree is running
            TreeWalkerCmd DoDirectory(String name);
            // wildmatch pattern
            String FileSearchPattern();
            String DirectorySearchPattern();
        }

        public static void WalkTree(String dirName, ITreeWalker walker)
        {
            var dirCount = 0;
            dirName = StringUtils.StandardisePath(dirName);
            Stack<String> dirStack = new Stack<String>();
            dirStack.Push(dirName);
            while (dirStack.Count > 0)
            {
                String lastPath = dirStack.Pop();
                DirectoryInfo di = new DirectoryInfo(lastPath);
                if (!di.Exists || ((di.Attributes & FileAttributes.Hidden) != 0 && dirCount > 0))
                {
                    continue;
                }
                ++dirCount;
                foreach (FileInfo fileInfo in di.GetFiles(walker.FileSearchPattern()))
                {
                    // compose full file name from dirName
                    String f = lastPath;
                    if (f[f.Length - 1] == '/')
                    {
                        f += fileInfo.Name;
                    }
                    else
                    {
                        f = f + "/" + fileInfo.Name;
                    }
                    var cmd = walker.DoFile(f);
                    switch (cmd)
                    {
                        case TreeWalkerCmd.Skip:
                            continue;
                        case TreeWalkerCmd.Exit:
                            goto EXIT;
                    }
                }
                if (walker.IsRecursive())
                {
                    foreach (DirectoryInfo dirInfo in di.GetDirectories(walker.DirectorySearchPattern()))
                    {
                        // compose full path name from dirName
                        String p = lastPath;
                        if (p[p.Length - 1] == '/')
                        {
                            p += dirInfo.Name;
                        }
                        else
                        {
                            p = p + "/" + dirInfo.Name;
                        }
                        FileAttributes fa = File.GetAttributes(p);
                        if ((fa & FileAttributes.Hidden) == 0)
                        {
                            var cmd = walker.DoDirectory(p);
                            switch (cmd)
                            {
                                case TreeWalkerCmd.Skip:
                                    continue;
                                case TreeWalkerCmd.Exit:
                                    goto EXIT;
                            }
                            dirStack.Push(p);
                        }
                    }
                }
            }
        EXIT:
            ;
        }

        public class BaseTreeWalker : ITreeWalker
        {
            public virtual bool IsRecursive() { return true; }
            public virtual TreeWalkerCmd DoFile(String name) { return TreeWalkerCmd.Continue; }
            public virtual TreeWalkerCmd DoDirectory(String name) { return TreeWalkerCmd.Continue; }
            public virtual String FileSearchPattern() { return "*"; }
            public virtual String DirectorySearchPattern() { return "*"; }
        }

        class FileScanner : BaseTreeWalker
        {
            List<String> m_allFiles;
            Func<String, Boolean> m_fileFilter;
            Func<String, Boolean> m_directoryFilter;
            bool m_recursive;
            public FileScanner(List<String> fs, Func<String, Boolean> filter, bool recursive, Func<String, Boolean> directoryFilter)
            {
                m_allFiles = fs;
                m_fileFilter = filter;
                m_recursive = recursive;
                m_directoryFilter = directoryFilter;
            }
            public override bool IsRecursive()
            {
                return m_recursive;
            }
            public override TreeWalkerCmd DoDirectory(string name)
            {
                if (m_directoryFilter != null && !m_directoryFilter(name))
                {
                    return TreeWalkerCmd.Skip;
                }
                return base.DoDirectory(name);
            }
            public override TreeWalkerCmd DoFile(String name)
            {
                if (m_fileFilter == null || m_fileFilter(name))
                {
                    m_allFiles.Add(name);
                }
                return TreeWalkerCmd.Continue;
            }
        }

        public static List<String> GetFileList(String path, Func<String, Boolean> filter, bool recursive = true)
        {
            var ret = new List<String>();
            if (!String.IsNullOrEmpty(path))
            {
                var fs = new FileScanner(ret, filter, recursive, null);
                WalkTree(path, fs);
            }
            return ret;
        }

        // create a directory
        // each sub directories will be created if any of them don't exist.
        public static bool CreateDirectory(String path)
        {
            try
            {
                // first remove file name and extension;
                var ext = Path.GetExtension(path);
                String fileNameAndExt = Path.GetFileName(path);
                if (!String.IsNullOrEmpty(fileNameAndExt) && !string.IsNullOrEmpty(ext))
                {
                    path = path.Substring(0, path.Length - fileNameAndExt.Length);
                }
                int i = 0;
                var sb = new StringBuilder();
                var folderNames = path.Split('/', '\\');
                while (String.IsNullOrEmpty(folderNames[i]))
                {
                    // 跳过前面的空分割文件夹名
                    // 比如path = "/a/b/c".Split( '/' ) => [ "", "a", "b", "c" ];
                    // 注意前面会有空字符串产生，跳过
                    ++i;
                }
                if (folderNames.Length > i)
                {
                    // 如果输入路径以'/'起始，说明是posix系统的全路径
                    if (path[0] == '/')
                    {
                        // 追加全路径标记
                        folderNames[i] = "/" + folderNames[i];
                    }
                }
                for (; i < folderNames.Length; ++i)
                {
                    if (String.IsNullOrEmpty(folderNames[i]))
                    {
                        // 如果中间有空格，忽略
                        // 如果输入路径最后是'/'，那么最后一个dirs[ i ]一定是空，跳过
                        continue;
                    }
                    if (sb.Length > 0 && sb[sb.Length - 1] != '/')
                    {
                        sb.Append('/');
                    }
                    sb.Append(folderNames[i]);
                    var cur = sb.ToString();
                    if (String.IsNullOrEmpty(cur))
                    {
                        continue;
                    }
                    if (!Directory.Exists(cur))
                    {
                        var info = Directory.CreateDirectory(cur);
                        if (null == info)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }
    }

    public struct EnumValue
    {
        public int index;
        public object boxed;
        public String name;
        public Int64 value;
    }

    public static class EnumHelper
    {

        internal class EnumValues
        {
            internal Dictionary<Int64, EnumValue> _infoLUT = null;
            internal Dictionary<String, Int64> _name2ValueLUT = null;
            internal EnumValue[] _values = null;
        }
        static Dictionary<Type, EnumValues> s_enumValueCache = new Dictionary<Type, EnumValues>();

        internal static EnumValues GetCache(Type t)
        {
            if (!t.IsEnum)
            {
                return null;
            }
            EnumValues cache;
            if (!s_enumValueCache.TryGetValue(t, out cache))
            {
                var enumSize = Marshal.SizeOf(Enum.GetUnderlyingType(t));
                var intSize = Marshal.SizeOf(typeof(Int64));
                if (enumSize > intSize)
                {
                    System.Diagnostics.Debug.Assert(false, String.Format("EnumInt64<{0}>: Can't convert {0} to Int64.", t.FullName));
                    return null;
                }
                cache = new EnumValues();
                var values = Enum.GetValues(t);
                var names = Enum.GetNames(t);
                cache._infoLUT = new Dictionary<Int64, EnumValue>(values.Length);
                cache._name2ValueLUT = new Dictionary<String, Int64>(names.Length);
                cache._values = new EnumValue[values.Length];
                try
                {
                    for (int i = 0; i < values.Length; ++i)
                    {
                        var str = names.GetValue(i) as String;
                        Debug.Assert(!String.IsNullOrEmpty(str));
                        var boxed = values.GetValue(i);
                        var ival = Convert.ToInt64(boxed);
                        var eval = new EnumValue
                        {
                            index = i,
                            boxed = boxed,
                            name = str,
                            value = ival
                        };
                        cache._values[i] = eval;
                        if (!cache._infoLUT.ContainsKey(ival))
                        {
                            cache._infoLUT.Add(ival, eval);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                s_enumValueCache.Add(t, cache);
            }
            return cache;
        }

        public static object ToObject(Type enumType, Int32 value)
        {
            return ToObject(enumType, (Int64)value);
        }

        public static object ToObject(Type enumType, Int64 value)
        {
            try
            {
                var cache = GetCache(enumType);
                if (cache != null)
                {
                    EnumValue e;
                    if (cache._infoLUT.TryGetValue(value, out e))
                    {
                        return e.boxed;
                    }
                }
                else if (enumType.IsEnum)
                {
                    return Enum.ToObject(enumType, value);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }
    }

    public struct EnumInt32<T> where T : struct, IConvertible
    {

        static EnumInt32()
        {
            try
            {
                var et = typeof(T);
                if (et.IsEnum)
                {
                    var enumSize = Marshal.SizeOf(Enum.GetUnderlyingType(et));
                    var intSize = Marshal.SizeOf(typeof(Int32));
                    if (enumSize == intSize)
                    {
                        return;
                    }
                }
            }
            catch
            {
            }
            System.Diagnostics.Debug.Assert(false, String.Format("EnumInt32<{0}>: Can't convert {0} to Int32.", typeof(T).FullName));
        }

        static EnumHelper.EnumValues s_cache = null;

        static void _EnsureLUTs()
        {
            if (s_cache == null)
            {
                s_cache = EnumHelper.GetCache(typeof(T));
            }
        }

        public static EnumValue[] GetDefines()
        {
            _EnsureLUTs();
            return s_cache._values;
        }
    }
}
//EOF
