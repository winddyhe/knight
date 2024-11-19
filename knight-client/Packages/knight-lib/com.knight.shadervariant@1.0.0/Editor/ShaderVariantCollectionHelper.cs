using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Reflection;
using System.Text.RegularExpressions;

using Debug = UnityEngine.Debug;

namespace Knight.Framework.ShaderVariant.Editor
{
    public static class ShaderInspectorPlatformsPopup
    {

        static Type s_Type = null;
        static Type internalType
        {
            get
            {
                if (s_Type == null)
                {
                    s_Type = Utils.FindType("UnityEditor.ShaderInspectorPlatformsPopup", "UnityEditor");
                }
                return s_Type;
            }
        }
        public static int currentMode
        {
            get
            {
                return (int)Utils.RflxGetValue(internalType, "currentMode");
            }
            set
            {
                Utils.RflxSetValue(internalType, "currentMode", value);
            }
        }
        public static int currentPlatformMask
        {
            get
            {
                return (int)Utils.RflxGetValue(internalType, "currentPlatformMask");
            }
            set
            {
                Utils.RflxSetValue(internalType, "currentPlatformMask", value);
            }
        }
        public static int currentVariantStripping
        {
            get
            {
                return (int)Utils.RflxGetValue(internalType, "currentVariantStripping");
            }
            set
            {
                Utils.RflxSetValue(internalType, "currentVariantStripping", value);
            }
        }
    }

    public class ShaderParsedCombinations
    {

        internal static readonly Regex REG_FILE_HEADER = new Regex(@"^// Total snippets: (\d+)");
        internal static readonly Regex REG_SNIPPET_ID = new Regex(@"^// Snippet #(\d+) platforms ([0-9a-fA-F]+):");
        internal static readonly Regex REG_SHADER_FEATURE_KEYWORDS = new Regex(@"^Keywords stripped away when not used: (.+)$");
        internal static readonly Regex REG_MULTI_COMPILE_KEYWORDS = new Regex(@"^Keywords always included into build: (.+)$");
        internal static readonly Regex REG_BUILTIN_KEYWORDS = new Regex(@"^Builtin keywords used: (.+)$");
        internal static readonly Regex REG_VARIANTS_NUM = new Regex(@"^(\d+) keyword variants used in scene:$");
        internal const String TAG_NO_KEYWORDS_DEFINED = "<no keywords defined>";

        public class Snippet
        {
            public int id;
            public int platformBits;
            public String[] shader_features;
            public String[] multi_compiles;
            public String[] builtins;
            public List<String[]> variants;
        }
        public Shader shader;
        public List<Snippet> snippets;

        public bool IsValidKeyword(String keyword)
        {
            if (snippets != null)
            {
                for (int i = 0; i < snippets.Count; ++i)
                {
                    var snippet = snippets[i];
                    if (snippet.shader_features != null)
                    {
                        if (Array.IndexOf(snippet.shader_features, keyword) >= 0)
                        {
                            return true;
                        }
                    }
                    if (snippet.multi_compiles != null)
                    {
                        if (Array.IndexOf(snippet.multi_compiles, keyword) >= 0)
                        {
                            return true;
                        }
                    }
                    if (snippet.builtins != null)
                    {
                        if (Array.IndexOf(snippet.builtins, keyword) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public String Dump()
        {
            String result;
            var sb = new StringBuilder();
            sb.Append(ShaderUtils.DumpShaderPasses(shader));

            if (snippets != null && snippets.Count > 0)
            {
                sb.AppendFormat("// Total snippets: {0}", snippets.Count).AppendLine();
                sb.AppendLine();

                for (int i = 0; i < snippets.Count; ++i)
                {
                    var snippet = snippets[i];
                    sb.AppendFormat("// Snippet #{0} platforms {1:x}:", snippet.id, snippet.platformBits).AppendLine();
                    sb.AppendLine();

                    if (snippet.shader_features != null && snippet.shader_features.Length > 0)
                    {
                        sb.AppendFormat("Keywords stripped away when not used:\n\n{0}", String.Join("\n", snippet.shader_features)).AppendLine();
                        sb.AppendLine();
                    }
                    if (snippet.multi_compiles != null && snippet.multi_compiles.Length > 0)
                    {
                        sb.AppendFormat("Keywords always included into build:\n\n{0}", String.Join("\n", snippet.multi_compiles)).AppendLine();
                        sb.AppendLine();
                    }
                    if (snippet.builtins != null && snippet.builtins.Length > 0)
                    {
                        sb.AppendFormat("Builtin keywords used:\n\n{0}", String.Join("\n", snippet.builtins)).AppendLine();
                        sb.AppendLine();
                    }
                    sb.AppendLine();

                    if (snippet.variants != null && snippet.variants.Count > 0)
                    {
                        sb.AppendFormat("{0} keyword variants used in scene:", snippet.variants.Count).AppendLine();
                        sb.AppendLine();
                        for (int j = 0; j < snippet.variants.Count; ++j)
                        {
                            sb.AppendFormat("[{0}]:", j).AppendLine();
                            sb.Append(String.Join("\n", snippet.variants[j].ToArray()));
                            sb.AppendLine();
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        sb.Append(TAG_NO_KEYWORDS_DEFINED).AppendLine();
                    }
                    sb.AppendLine();
                }
                result = sb.ToString();
            }
            else
            {
                result = "Empty ShaderParsedCombinations.";
            }
            Debug.Log(result);
            return result;
        }
    }

    public static class ShaderUtils
    {

        public enum ShaderCompilerPlatformType
        {
            OpenGL,
            D3D9,
            Xbox360,
            PS3,
            D3D11,
            OpenGLES20,
            OpenGLES20Desktop,
            Flash,
            D3D11_9x,
            OpenGLES30,
            PSVita,
            PS4,
            XboxOne,
            PSM,
            Metal,
            OpenGLCore,
            N3DS,
            WiiU,
            Vulkan,
            Switch,
            Count
        }

        static MethodInfo _GetShaderVariantEntries = null;

        internal static MethodInfo GetShaderVariantEntriesMethod
        {
            get
            {
                if (_GetShaderVariantEntries == null)
                {
                    _GetShaderVariantEntries = typeof(ShaderUtil).GetMethod(
                        "GetShaderVariantEntries", BindingFlags.NonPublic | BindingFlags.Static);
                }
                return _GetShaderVariantEntries;
            }
        }

        public static List<Texture> GetTextures(Material mat)
        {
            var list = new List<Texture>();
            var count = GetPropertyCount(mat);
            for (var i = 0; i < count; i++)
            {
                if (GetPropertyType(mat, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    list.Add((Texture)GetProperty(mat, i));
                }
            }
            return list;
        }

        public static int GetPropertyCount(Material mat)
        {
            return ShaderUtil.GetPropertyCount(mat.shader);
        }

        public static ShaderUtil.ShaderPropertyType GetPropertyType(Material mat, int index)
        {
            return ShaderUtil.GetPropertyType(mat.shader, index);
        }

        public static string GetPropertyName(Material mat, int index)
        {
            return ShaderUtil.GetPropertyName(mat.shader, index);
        }

        public static void SetProperty(Material material, int index, object value)
        {
            var name = GetPropertyName(material, index);
            var type = GetPropertyType(material, index);
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    material.SetColor(name, (Color)value);
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    material.SetVector(name, (Vector4)value);
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    material.SetFloat(name, (float)value);
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    material.SetFloat(name, (float)value);
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    material.SetTexture(name, (Texture)value);
                    break;
            }
        }

        public static object GetProperty(Material material, int index)
        {
            var name = GetPropertyName(material, index);
            var type = GetPropertyType(material, index);
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    return material.GetColor(name);
                case ShaderUtil.ShaderPropertyType.Vector:
                    return material.GetVector(name);
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range:
                    return material.GetFloat(name);
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    return material.GetTexture(name);
            }
            return null;
        }

        public static bool DoesShaderHasAnyOneOfProperties(Shader shader, String name, params ShaderUtil.ShaderPropertyType[] types)
        {
            for (int i = 0, count = ShaderUtil.GetPropertyCount(shader); i < count; ++i)
            {
                if (ShaderUtil.GetPropertyName(shader, i) == name)
                {
                    var type = ShaderUtil.GetPropertyType(shader, i);
                    if (Array.IndexOf(types, type) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int FindPropertyIndex(Shader shader, String name, ShaderUtil.ShaderPropertyType? type = null)
        {
            var count = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < count; ++i)
            {
                var _name = ShaderUtil.GetPropertyName(shader, i);
                var _type = ShaderUtil.GetPropertyType(shader, i);
                if (_name == name && (type == null || type.Value == _type))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void OpenShaderCombinations(Shader shader, bool usedBySceneOnly, bool withLog = true)
        {
            if (!HasCodeSnippets(shader))
            {
                Debug.LogErrorFormat("{0} is precompiled shader.", shader.name);
                return;
            }
            if (withLog)
            {
                Debug.LogFormat("OpenShaderCombinations: {0}, usedBySceneOnly = {1}", shader.name, usedBySceneOnly);
            }
            //Utils.RflxStaticCall(
            //    typeof( ShaderUtil ),
            //    "OpenShaderCombinations",
            //    new object[] { shader, usedBySceneOnly } );
        }

        public static bool HasCodeSnippets(Shader shader)
        {
            return HasShaderSnippets(shader) || HasSurfaceShaders(shader) || HasFixedFunctionShaders(shader);
        }

        public static void OpenCurrentCompiledShader(Shader shader)
        {
            if (!HasCodeSnippets(shader))
            {
                Debug.LogErrorFormat("{0} is precompiled shader.", shader.name);
                return;
            }
            OpenCompiledShader(shader, ShaderInspectorPlatformsPopup.currentMode, ShaderInspectorPlatformsPopup.currentPlatformMask, ShaderInspectorPlatformsPopup.currentVariantStripping == 0);
        }

        public static void OpenCompiledShader(Shader shader, int mode, int externPlatformsMask, bool includeAllVariants)
        {
            if (!HasCodeSnippets(shader))
            {
                Debug.LogErrorFormat("{0} is precompiled shader.", shader.name);
                return;
            }
            Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "OpenCompiledShader",
                new object[] { shader, mode, externPlatformsMask, includeAllVariants });
        }

        public static bool HasShaderSnippets(Shader shader)
        {
            return (bool)Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "HasShaderSnippets",
                new object[] { shader });
        }

        public static bool HasSurfaceShaders(Shader shader)
        {
            return (bool)Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "HasSurfaceShaders",
                new object[] { shader });
        }

        public static bool HasFixedFunctionShaders(Shader shader)
        {
            return (bool)Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "HasFixedFunctionShaders",
                new object[] { shader });
        }

        public static void ClearCurrentShaderVariantCollection()
        {
            Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "ClearCurrentShaderVariantCollection", null);
        }

        public static void SaveCurrentShaderVariantCollection(String path)
        {
            Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "SaveCurrentShaderVariantCollection", new object[] { path });
        }

        public static int GetCurrentShaderVariantCollectionShaderCount()
        {
            var shaderCount = Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "GetCurrentShaderVariantCollectionShaderCount", null);
            return (int)shaderCount;
        }

        public static int GetCurrentShaderVariantCollectionVariantCount()
        {
            var shaderVariantCount = Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "GetCurrentShaderVariantCollectionVariantCount", null);
            return (int)shaderVariantCount;
        }

        public static ulong GetVariantCount(Shader s, bool usedBySceneOnly)
        {
            var shaderVariantCount = Utils.RflxStaticCall(
                typeof(ShaderUtil),
                "GetVariantCount", new object[] { s, usedBySceneOnly });
            return (ulong)shaderVariantCount;
        }

        class ShaderParsedCombinationsItem
        {
            internal ShaderParsedCombinations data;
            internal long lastWriteTime;
        }

        static Dictionary<Shader, ShaderParsedCombinationsItem> s_ShaderParsedCombinationsCache = new Dictionary<Shader, ShaderParsedCombinationsItem>();

        static String GetProjectUnityTempPath()
        {
            var rootPath = Environment.CurrentDirectory.Replace('\\', '/');
            rootPath += "/Temp";
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

        public static ShaderParsedCombinations ParseShaderCombinations(Shader shader, bool usedBySceneOnly, bool withLog = true)
        {
            if (shader == null || !ShaderUtils.HasCodeSnippets(shader))
            {
                return null;
            }
            var assetPath = AssetDatabase.GetAssetPath(shader);
            var deps = AssetDatabase.GetDependencies(assetPath);
            long lastWriteTime = 0;
            for (int i = 0; i < deps.Length; ++i)
            {
                var fi = new FileInfo(deps[i]);
                if (fi.Exists)
                {
                    if (fi.LastWriteTime.ToFileTime() > lastWriteTime)
                    {
                        lastWriteTime = fi.LastWriteTime.ToFileTime();
                    }
                }
            }
            ShaderParsedCombinationsItem item;
            if (s_ShaderParsedCombinationsCache.TryGetValue(shader, out item))
            {
                if (item.lastWriteTime != lastWriteTime)
                {
                    s_ShaderParsedCombinationsCache.Remove(shader);
                }
                else
                {
                    return item.data;
                }
            }

            ShaderParsedCombinations result = null;
            try
            {
                var fixedShaderName = shader.name.Replace('/', '-');
                var combFilePath = String.Format("{0}/ParsedCombinations-{1}.shader", GetProjectUnityTempPath(), fixedShaderName);
                if (File.Exists(combFilePath))
                {
                    File.Delete(combFilePath);
                }
                Func<String, String[]> keywordsSpliter = src =>
                {
                    var srcKeywords = src.Split(' ');
                    var dstKeywords = new List<String>();
                    for (int j = 0; j < srcKeywords.Length; ++j)
                    {
                        var x = srcKeywords[j].Trim();
                        if (!String.IsNullOrEmpty(x) && !dstKeywords.Contains(x))
                        {
                            dstKeywords.Add(x);
                        }
                    }
                    if (dstKeywords.Count > 0)
                    {
                        return dstKeywords.ToArray();
                    }
                    return null;
                };
                ShaderUtils.OpenShaderCombinations(shader, true, withLog);
                if (File.Exists(combFilePath))
                {
                    var lines = File.ReadAllLines(combFilePath);
                    ShaderParsedCombinations.Snippet curSnippet = null;
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        var line = lines[i];
                        if (String.IsNullOrEmpty(line) || Char.IsWhiteSpace(line[0]))
                        {
                            continue;
                        }
                        Match match = ShaderParsedCombinations.REG_FILE_HEADER.Match(line);
                        if (match.Success)
                        {
                            if (match.Groups.Count > 1)
                            {
                                int num;
                                if (int.TryParse(match.Groups[1].Value, out num) && num > 0)
                                {
                                    result = new ShaderParsedCombinations();
                                    result.shader = shader;
                                    result.snippets = new List<ShaderParsedCombinations.Snippet>(num);
                                }
                            }
                        }
                        else if (result != null && (match = ShaderParsedCombinations.REG_SNIPPET_ID.Match(line)).Success)
                        {
                            if (match.Groups.Count > 2)
                            {
                                int id;
                                if (int.TryParse(match.Groups[1].Value, out id))
                                {
                                    int bits;
                                    if (int.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber, null, out bits))
                                    {
                                        var snippet = new ShaderParsedCombinations.Snippet();
                                        curSnippet = snippet;
                                        snippet.id = id;
                                        snippet.platformBits = bits;
                                        result.snippets.Add(snippet);
                                    }
                                }
                            }
                        }
                        else if (curSnippet != null && (match = ShaderParsedCombinations.REG_SHADER_FEATURE_KEYWORDS.Match(line)).Success)
                        {
                            if (match.Groups.Count > 1)
                            {
                                var keywords = keywordsSpliter(match.Groups[1].Value);
                                if (keywords != null)
                                {
                                    curSnippet.shader_features = keywords;
                                }
                            }
                        }
                        else if (curSnippet != null && (match = ShaderParsedCombinations.REG_MULTI_COMPILE_KEYWORDS.Match(line)).Success)
                        {
                            if (match.Groups.Count > 1)
                            {
                                var keywords = keywordsSpliter(match.Groups[1].Value);
                                if (keywords != null)
                                {
                                    curSnippet.multi_compiles = keywords;
                                }
                            }
                        }
                        else if (curSnippet != null && (match = ShaderParsedCombinations.REG_BUILTIN_KEYWORDS.Match(line)).Success)
                        {
                            if (match.Groups.Count > 1)
                            {
                                var keywords = keywordsSpliter(match.Groups[1].Value);
                                if (keywords != null)
                                {
                                    curSnippet.builtins = keywords;
                                }
                            }
                        }
                        else if (curSnippet != null && (match = ShaderParsedCombinations.REG_VARIANTS_NUM.Match(line)).Success)
                        {
                            if (match.Groups.Count > 1)
                            {
                                int num;
                                if (int.TryParse(match.Groups[1].Value, out num) && num > 0)
                                {
                                    curSnippet.variants = new List<String[]>(num);
                                }
                            }
                        }
                        else if (curSnippet != null && line.StartsWith(ShaderParsedCombinations.TAG_NO_KEYWORDS_DEFINED))
                        {
                            if (curSnippet.variants != null)
                            {
                                curSnippet.variants = null;
                            }
                        }
                        else if (curSnippet != null && curSnippet.variants != null)
                        {
                            var keywords = keywordsSpliter(line);
                            if (keywords != null)
                            {
                                curSnippet.variants.Add(keywords);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            s_ShaderParsedCombinationsCache.Add(shader,
                new ShaderParsedCombinationsItem { data = result, lastWriteTime = lastWriteTime });
            return result;
        }

        public static String DumpShaderPasses(Shader shader)
        {
            String result;
            var sb = new StringBuilder();
            var shaderData = ShaderUtil.GetShaderData(shader);
            sb.AppendFormat("// Subshader Count: {0}", shaderData.SubshaderCount).AppendLine();
            sb.AppendLine();

            for (int i = 0; i < shaderData.SubshaderCount; ++i)
            {
                var subShader = shaderData.GetSubshader(i);
                sb.AppendFormat("// Subshader {0}", i);
                if (shaderData.ActiveSubshaderIndex == i)
                {
                    sb.Append("-(Active)");
                }
                sb.Append(":").AppendLine();

                for (int j = 0; j < subShader.PassCount; ++j)
                {
                    var name = subShader.GetPass(j).Name;
                    sb.AppendFormat("  -Pass: \"{0}\"", String.IsNullOrEmpty(name) ? "<noname>" : name).AppendLine();
                }
                sb.AppendLine();
            }
            result = sb.ToString();
            return result;
        }

        public static List<String> GetShaderKeywords(Shader shader)
        {
            int[] types = null;
            String[] keywords = null;
            object[] args = new object[] { shader, new ShaderVariantCollection(), types, keywords };
            ShaderUtils.GetShaderVariantEntriesMethod.Invoke(null, args);
            keywords = args[3] as String[];
            var result = new List<String>();
            foreach (var keyword in keywords)
            {
                foreach (var t in keyword.Split(' '))
                {
                    if (!result.Contains(t))
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public static void ClearCache()
        {
            s_ShaderParsedCombinationsCache.Clear();
        }
    }

    public static class ShaderVariantCollectionHelper
    {

        static void StableSort<T>(T[] list, Comparison<T> comparison = null) where T : IComparable<T>
        {
            int count = list.Length;
            for (int j = 1; j < count; j++)
            {
                T key = list[j];
                int i = j - 1;
                for (; i >= 0 && ((comparison != null && comparison(list[i], key) > 0) || (list[i].CompareTo(key) > 0)); i--)
                {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }
        }

        public struct ShaderVariant : IComparable<ShaderVariant>, IEquatable<ShaderVariant>
        {
            public Shader shader;
            public PassType passType;
            public string[] keywords;
            public string[] sorted_keywords;
            String desc;
            public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
            {
                var tempKeywords = new List<string>(keywords);
                tempKeywords.Remove("FOG_LINEAR");
                tempKeywords.Remove("_MRT_PREPARE_NORMAL");
                keywords = tempKeywords.ToArray();

                this.shader = shader;
                this.passType = passType;
                this.keywords = keywords;
                if (keywords != null)
                {
                    this.sorted_keywords = new string[keywords.Length];
                    Array.Copy(keywords, this.sorted_keywords, keywords.Length);
                    StableSort(this.sorted_keywords);
                }
                else
                {
                    this.sorted_keywords = null;
                }
                desc = null;
            }
            public override string ToString()
            {
                desc = desc ?? String.Format("{0}, [{1}] : '{2}'", shader.name, passType, sorted_keywords != null ? String.Join(" ", sorted_keywords) : "--");
                return desc;
            }
            public int CompareTo(ShaderVariant other)
            {
                return this.ToString().CompareTo(other.ToString());
            }
            public bool Equals(ShaderVariant other)
            {
                return this.ToString().Equals(other.ToString());
            }
        }

        public class RawData : Dictionary<Shader, List<ShaderVariant>>
        {
        }

        public static RawData GetShaderVariantEntries(Shader shader)
        {
            if (shader == null)
            {
                return null;
            }
            int[] types = null;
            String[] keywords = null;
            object[] args = new object[] { shader, new ShaderVariantCollection(), types, keywords };
            ShaderUtils.GetShaderVariantEntriesMethod.Invoke(null, args);
            types = args[2] as int[];
            keywords = args[3] as String[];
            var result = new RawData();
            for (int i = 0; i < keywords.Length; ++i)
            {
                var keyword = keywords[i];
                var sv = new ShaderVariant(shader, (PassType)types[i], keyword.Split(' '));
                List<ShaderVariant> variants;
                if (!result.TryGetValue(shader, out variants))
                {
                    variants = new List<ShaderVariant>();
                    result.Add(shader, variants);
                }
                variants.Add(sv);
            }
            return result;
        }

        public static RawData ExtractData(ShaderVariantCollection svc)
        {
            var shaderVariants = new RawData();
            using (var so = new SerializedObject(svc))
            {
                var array = so.FindProperty("m_Shaders.Array");
                if (array != null && array.isArray)
                {
                    var arraySize = array.arraySize;
                    for (int i = 0; i < arraySize; ++i)
                    {
                        var shaderRef = array.FindPropertyRelative(String.Format("data[{0}].first", i));
                        var shaderShaderVariants = array.FindPropertyRelative(String.Format("data[{0}].second.variants", i));
                        if (shaderRef != null && shaderRef.propertyType == SerializedPropertyType.ObjectReference &&
                            shaderShaderVariants != null && shaderShaderVariants.isArray)
                        {
                            var shader = shaderRef.objectReferenceValue as Shader;
                            if (shader == null)
                            {
                                continue;
                            }
                            var shaderAssetPath = AssetDatabase.GetAssetPath(shader);
                            List<ShaderVariant> variants = null;
                            if (!shaderVariants.TryGetValue(shader, out variants))
                            {
                                variants = new List<ShaderVariant>();
                                shaderVariants.Add(shader, variants);
                            }
                            var variantCount = shaderShaderVariants.arraySize;
                            for (int j = 0; j < variantCount; ++j)
                            {
                                var prop_keywords = shaderShaderVariants.FindPropertyRelative(String.Format("Array.data[{0}].keywords", j));
                                var prop_passType = shaderShaderVariants.FindPropertyRelative(String.Format("Array.data[{0}].passType", j));
                                if (prop_keywords != null && prop_passType != null && prop_keywords.propertyType == SerializedPropertyType.String)
                                {
                                    var srcKeywords = prop_keywords.stringValue;
                                    var keywords = srcKeywords.Split(' ');
                                    var pathType = (UnityEngine.Rendering.PassType)prop_passType.intValue;
                                    variants.Add(new ShaderVariant(shader, pathType, keywords));
                                }
                            }
                        }
                    }
                }
            }
            return shaderVariants;
        }
    }
}
