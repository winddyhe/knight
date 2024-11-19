#if UNITY_EDITOR
#define _UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knight.Framework.ShaderVariant
{
    public class ShaderCollection : MonoBehaviour
    {
        public const String AssetPath = "Assets/Resources/Shaders/ShaderCollection.prefab";

        Dictionary<String, Shader> m_shaderLut = null;
        static ShaderCollection s_ShaderCollection = null;

        public ShaderItem[] shaders
        {
            get
            {
                return m_shaders;
            }
        }

        public ShaderVariantCollection[] shaderVariantCollections
        {
            get
            {
                return m_shaderVariantCollections;
            }
        }

        protected void Awake()
        {
            Debug.Log("ShaderCollection.OnInitialize");
            if (s_ShaderCollection == null)
            {
                s_ShaderCollection = this;
            }
            CheckInit();
        }

        protected void OnDestroy()
        {
            Debug.Log("ShaderCollection.OnUninitialize");
            m_shaderLut = null;
            m_shaders = null;
            if (s_ShaderCollection == this)
            {
                s_ShaderCollection = null;
            }
        }

        void CheckInit()
        {
            if (m_shaderLut != null)
            {
                return;
            }
            Debug.LogFormat("ShaderCollection Count: {0}", m_shaders != null ? m_shaders.Length : 0);
            m_shaderLut = new Dictionary<String, Shader>(m_shaders != null ? m_shaders.Length : 0);
            if (m_shaders != null)
            {
                try
                {
                    for (int i = 0; i < m_shaders.Length; ++i)
                    {
                        var s = m_shaders[i];
                        if (s.shader)
                        {
                            var shaderName = s.shader.name;
                            Debug.Assert(shaderName == s.name);
                            if (!m_shaderLut.ContainsKey(shaderName))
                            {
                                m_shaderLut.Add(shaderName, s.shader);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Shader {0} already in cache.", shaderName);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        Shader _Find(String name)
        {
            CheckInit();
            Shader ret = null;
            if (m_shaderLut != null)
            {
                m_shaderLut.TryGetValue(name, out ret);
            }
            return ret ?? Shader.Find(name);
        }

        public static Shader Find(String name)
        {
            if (ShaderCollection.s_ShaderCollection)
            {
                return s_ShaderCollection._Find(name);
            }
            else
            {
                if (Application.isPlaying)
                {
                    var msg = "You should initialize ShaderCollection before use it.";
                    Debug.LogWarning(msg);
                }
            }
            return Shader.Find(name);
        }

        public void Dump()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("ShaderCollection dump:").AppendLine();
            sb.AppendFormat("Count: {0}", m_shaders != null ? m_shaders.Length : 0).AppendLine();
            if (m_shaders != null)
            {
                for (int i = 0; i < m_shaders.Length; ++i)
                {
                    var item = m_shaders[i];
                    sb.AppendFormat("[{0}]: {1}", i, item.name, item.shader ? "loaded." : "null").AppendLine();
                }
            }
            sb.Append("ShaderCollection end.").AppendLine();
            Debug.Log(sb.ToString());
        }

#if _UNITY_EDITOR

        /// <summary>
        /// Shader目录列表
        /// </summary>
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
        public readonly static String[] PathList = new String[] {
        "Assets/__unity_default_shaders",
        "Assets/Shaders",
        "Assets/Resources/Shaders",
        "Assets/Spine/spine-unity/Shaders",
        "Assets/TextMesh Pro/Resources/Shaders",
    };

        /// <summary>
        /// Shader排除列表，使用通配符
        /// </summary>
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
        public readonly static String[] ExcludePatternList = new String[] {
        "Assets/Shaders/Demo/*.shader",
        "Assets/Shaders/Experimental/*.shader",
    };

        static bool _CheckIsExcluded(String path, HashSet<String> fset)
        {
            if (ExcludePatternList != null)
            {
                for (int i = 0; i < ExcludePatternList.Length; ++i)
                {
                    if (StringUtils.MatchWildcard(path, ExcludePatternList[i], true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool FileFilter_shader(String fileName)
        {
            return fileName.EndsWith(".shader", StringComparison.CurrentCultureIgnoreCase);
        }

        static ShaderItem[] _CollectAllShaders(ref ShaderVariantCollection[] shaderVariantCollections)
        {
            var shaders = new List<ShaderItem>();
            var variantCollections = new List<ShaderVariantCollection>();
            for (int i = 0; i < PathList.Length; ++i)
            {
                var fs = FileUtils.GetFileList(PathList[i], FileFilter_shader);
                var fset = new HashSet<String>(fs);
                for (int j = 0; j < fs.Count; ++j)
                {
                    if (_CheckIsExcluded(fs[j], fset))
                    {
                        continue;
                    }
                    var s = UnityEditor.AssetDatabase.LoadAssetAtPath(fs[j], typeof(Shader)) as Shader;
                    if (s != null)
                    {
                        shaders.Add(new ShaderItem { name = s.name, shader = s });
                    }
                    var svcPath = fs[j].Insert(0, "Assets/SVC/ShaderVariants/");
                    svcPath = System.IO.Path.ChangeExtension(svcPath, ".shadervariants");
                    var svc = UnityEditor.AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(svcPath);
                    if (svc != null)
                    {
                        variantCollections.Add(svc);
                    }

                    var patch_svcPath = fs[j].Insert(0, "Assets/SVC/ShaderVariants/__patches/");
                    patch_svcPath = System.IO.Path.ChangeExtension(patch_svcPath, ".shadervariants");
                    var patch_svc = UnityEditor.AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(patch_svcPath);
                    if (patch_svc != null)
                    {
                        variantCollections.Add(patch_svc);
                    }
                }
            }
            if (shaders.Count == 0)
            {
                return null;
            }
            shaders.Sort((l, r) => l.name.CompareTo(r.name));
            variantCollections.Sort((l, r) => l.name.CompareTo(r.name));
            shaderVariantCollections = variantCollections.ToArray();
            return shaders.ToArray();
        }

#if ODIN_INSPECTOR
    [Button( "Update List" )]
#endif
        public bool UpdateList()
        {
            var oldShaders = m_shaders;
            var oldSVCs = m_shaderVariantCollections;
            m_shaders = _CollectAllShaders(ref m_shaderVariantCollections);
            if (m_shaders != null && oldShaders != null)
            {
                if (!m_shaders.SequenceEqual(oldShaders))
                {
                    return true;
                }
            }
            else
            {
                var n1 = m_shaders != null ? m_shaders.Length : 0;
                var n2 = oldShaders != null ? oldShaders.Length : 0;
                if (n1 != n2)
                {
                    return true;
                }
            }
            if (m_shaderVariantCollections != null && oldSVCs != null)
            {
                if (!m_shaderVariantCollections.SequenceEqual(oldSVCs))
                {
                    return true;
                }
            }
            else
            {
                var n1 = m_shaderVariantCollections != null ? oldSVCs.Length : 0;
                var n2 = m_shaderVariantCollections != null ? oldSVCs.Length : 0;
                if (n1 != n2)
                {
                    return true;
                }
            }
            return false;
        }
#endif

        public int totalCount
        {
            get
            {
                return m_shaders != null ? m_shaders.Length : 0;
            }
        }

        public int loadedCount
        {
            get
            {
                CheckInit();
                return m_shaderLut != null ? m_shaderLut.Count : 0;
            }
        }

        [Serializable]
        public struct ShaderItem : IEquatable<ShaderItem>
        {
            [SerializeField]
            internal String name;
            [SerializeField]
            internal Shader shader;
            public bool Equals(ShaderItem other)
            {
                return shader == other.shader && String.Equals(name, other.name);
            }
        }

        [SerializeField]
        ShaderItem[] m_shaders = null;

        [SerializeField]
        ShaderVariantCollection[] m_shaderVariantCollections = null;
    }
}