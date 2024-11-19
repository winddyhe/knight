// #define ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

using Debug = UnityEngine.Debug;
using UDebug = UnityEngine.Debug;

namespace Knight.Framework.ShaderVariant.Editor
{
    [InitializeOnLoad]
    public static class ShaderVariantCollectionExporter
    {
        private static readonly String[] emptyStringArray = new String[] { };

        const String TempCameraName = "__temp_camera";

        /// <summary>
        /// 专门拿一个Prefab把整个项目中用到的所有Shader都关联起来，连同对应的变体集
        /// </summary>
        const String ShaderCollectionPath = "Assets/Resources/Shaders/ShaderCollection.prefab";

        /// <summary>
        /// 搜集所有的游戏会使用的材质资源，从打包配置目录而来，
        /// 因为场景变体会专门收集，所以这里面不包含场景中引用的材质
        /// </summary>
        /// <returns></returns>
        static List<String> CollectAllMaterialAssetsForGame()
        {
            var rGUIDs = AssetDatabase.FindAssets("t:Material", new string[]
            {
                "Assets/GameAssets/Battle",
                "Assets/GameAssets/Role",
                "Assets/GameAssets/Effect",
                "Assets/GameAssets/GUI/Materials",
                "Assets/GameAssets/GUI/TMP/Fonts",
            });
            var rAssetPaths = new List<String>();
            for (int i = 0; i < rGUIDs.Length; i++)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDs[i]);
                rAssetPaths.Add(rAssetPath);
            }
            return rAssetPaths;
        }

        /// <summary>
        /// 搜集动态加载的资源关联的材质，选取其中支持光照的材质，无光照相关的材质忽略
        /// </summary>
        static Dictionary<String, RendererParams> _CollectDynamicLightingMaterials()
        {
            var materialsWithRenderer = new Dictionary<String, RendererParams>();
            var pathList = new DynamicRenderersSettings[] 
            {
                new DynamicRenderersSettings {
                    assetPathRoot = "",
                    wildcard = ""
                },
            };

            try
            {
                var allPrefabs = new HashSet<String>();
                for (int k = 0; k < pathList.Length; ++k)
                {
                    var pathInfo = pathList[k];
                    var prefabs = FileUtils.GetFileList(pathInfo.assetPathRoot, EditorUtils.FileFilter_prefab);
                    var prefabPath = String.Empty;
                    for (int i = 0; i < prefabs.Count; ++i)
                    {
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(prefabs[i]);
                        if (!String.IsNullOrEmpty(pathInfo.wildcard) && StringUtils.MatchWildcard(fileName, pathInfo.wildcard))
                        {
                            prefabPath = prefabs[i];
                            allPrefabs.Add(prefabPath);
                        }
                        else if (pathInfo.regex != null && pathInfo.regex.IsMatch(fileName))
                        {
                            prefabPath = prefabs[i];
                            allPrefabs.Add(prefabPath);
                        }
                    }
                }
                var prefabIndex = 0;

                foreach (var prefabPath in allPrefabs)
                {
                    if (!String.IsNullOrEmpty(prefabPath))
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Collect Dynamic Material with Renderer", prefabPath, (float)prefabIndex / allPrefabs.Count))
                        {
                            break;
                        }
                        var go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        var renderers = go.GetComponentsInChildren<Renderer>();
                        for (int i = 0; i < renderers.Length; ++i)
                        {
                            var r = renderers[i];
                            var mats = r.sharedMaterials;
                            for (int j = 0; j < mats.Length; ++j)
                            {
                                var mat = mats[j];
                                var matPath = AssetDatabase.GetAssetPath(mat);
                                if (!String.IsNullOrEmpty(matPath))
                                {
                                    if (!EditorUtils.IsEmptyShader(mat.shader))
                                    {
                                        bool userVertexLight;
                                        // 只搜集支持光照的材质
                                        var supportsLighting = DoesShaderSupportsLighting(mat.shader, out userVertexLight);
                                        if (supportsLighting != null && supportsLighting.Value == false)
                                        {
                                            continue;
                                        }
                                        var par = new RendererParams(r, matPath);
                                        var key = par.ToString();
                                        if (!materialsWithRenderer.ContainsKey(key))
                                        {
                                            materialsWithRenderer.Add(key, par);
                                            Debug.Log(matPath);
                                        }
                                    }
                                }
                            }
                        }
                        ++prefabIndex;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            return materialsWithRenderer;
        }

        class DynamicRenderersSettings
        {
            internal String assetPathRoot;
            internal String wildcard;
            internal Regex regex = null;
            internal String modelType = null;
        }

        /// <summary>
        /// 记录一些在游戏中可以动态换材质、或者切换效果的渲染器参数
        /// </summary>
        class RendererParams
        {
            internal bool receiveShadows = false;
            internal ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            internal LightProbeUsage lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            internal ReflectionProbeUsage reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            internal MotionVectorGenerationMode motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            internal String materialPath = String.Empty;
            internal Material instancedMaterial = null;
            internal Shader instancedMaterialShader = null;
            internal String[] instancedMaterialKeywords = null;
            public RendererParams()
            {
            }
            public RendererParams(Renderer r, String materialPath)
            {
                receiveShadows = r.receiveShadows;
                shadowCastingMode = r.shadowCastingMode;
                lightProbeUsage = r.lightProbeUsage;
                reflectionProbeUsage = r.reflectionProbeUsage;
                motionVectorGenerationMode = r.motionVectorGenerationMode;
                this.materialPath = materialPath;
            }
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(materialPath).AppendLine();
                sb.Append(receiveShadows).AppendLine();
                sb.Append(shadowCastingMode).AppendLine();
                sb.Append(lightProbeUsage).AppendLine();
                sb.Append(reflectionProbeUsage).AppendLine();
                sb.Append(motionVectorGenerationMode).AppendLine();
                return sb.ToString();
            }
            public bool Apply(Renderer r)
            {
                var mat = instancedMaterial != null ? instancedMaterial : AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if ((object)instancedMaterial != null && instancedMaterial == null && instancedMaterialShader != null)
                {
                    mat = new Material(instancedMaterialShader);
                    instancedMaterial = mat;
                    if (instancedMaterialKeywords != null)
                    {
                        mat.shaderKeywords = instancedMaterialKeywords;
                    }
                }
                if (mat != null)
                {
                    r.sharedMaterial = mat;
                    r.receiveShadows = receiveShadows;
                    r.shadowCastingMode = shadowCastingMode;
                    r.lightProbeUsage = lightProbeUsage;
                    r.reflectionProbeUsage = reflectionProbeUsage;
                    r.motionVectorGenerationMode = motionVectorGenerationMode;
                    return true;
                }
                return false;
            }
        };

        /// <summary>
        /// 判断指定Shader是否支持光照
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="userVertexLight"></param>
        /// <returns></returns>
        public static bool? DoesShaderSupportsLighting(Shader shader, out bool userVertexLight)
        {
            userVertexLight = false;
            if (EditorUtils.IsEmptyShader(shader))
            {
                return null;
            }
            var shaderFullName = shader.name;
            var shaderShortName = shaderFullName;
            var split = shaderFullName.LastIndexOf('/');
            if (split >= 0)
            {
                shaderShortName = shaderFullName.Substring(split + 1);
            }
            // unity内置
            if (shaderFullName.StartsWith("Standard") ||
                shaderFullName.StartsWith("Mobile/VertexLit") ||
                shaderFullName.StartsWith("Mobile/Bumped") ||
                shaderFullName.StartsWith("Mobile/Diffuse") ||
                shaderFullName.StartsWith("Legacy Shaders/"))
            {
                return true;
            }
            if (shaderFullName.StartsWith("Particles/") ||
                shaderFullName.StartsWith("Skybox/") ||
                shaderFullName.StartsWith("Sprites/") ||
                shaderFullName.StartsWith("Mobile/Skybox") ||
                shaderFullName.StartsWith("Mobile/Particles/") ||
                shaderFullName.StartsWith("Unlit/") ||
                shaderFullName.StartsWith("UI/"))
            {
                return false;
            }
            // 自定义
            /*
            if ( ... ) {
                return false;
            }
            if ( ... ) {
                userVertexLight = true;
                return true;
            }
            */
            return null;
        }

        static void StableSort<T>(this IList<T> list, Comparison<T> comparison = null) where T : IComparable<T>
        {
            int count = list.Count;
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

        [MenuItem("Assets/Shader/Validate ShaderVariantCollection")]
        static void ValidateShaderVariantCollection()
        {
            var svc = Selection.activeObject as ShaderVariantCollection;
            if (svc != null)
            {
                var data = ShaderVariantCollectionHelper.ExtractData(svc);
                try
                {
                    int group = 0;
                    int groupCount = data.Count;
                    foreach (var sv in data)
                    {
                        var parentProgress = (float)group / groupCount;
                        var shader = sv.Key;
                        if (shader != null)
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("Validating ShaderVariants...", shader.name, parentProgress))
                            {
                                return;
                            }
                            var shaderPath = AssetDatabase.GetAssetPath(shader);
                            if (EditorUtils.IsUnityDefaultResource(shaderPath))
                            {
                            }
                            else if (!String.IsNullOrEmpty(shaderPath))
                            {
                                var shaderVariantsPath = System.IO.Path.ChangeExtension(shaderPath, ".shadervariants");
                                shaderVariantsPath = shaderVariantsPath.Insert(0, "Assets/SVC/ShaderVariants/");
                                var targetSVC = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderVariantsPath);
                                if (targetSVC == null)
                                {
                                    Debug.LogErrorFormat("ShaderVariantCollection is missing for shader: {0}", shader.name);
                                }
                                else
                                {
                                    int count = sv.Value.Count;
                                    int index = 0;
                                    foreach (var v in sv.Value)
                                    {
                                        try
                                        {
                                            var _v = new ShaderVariantCollection.ShaderVariant(v.shader, v.passType, v.keywords);
                                            if (!targetSVC.Contains(_v))
                                            {
                                                Debug.LogErrorFormat("ShaderVariant does not exist: {0}", v.ToString());
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            UDebug.LogError(e);
                                        }
                                        ++index;
                                    }
                                }
                            }
                        }
                        ++group;
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                UDebug.LogError("Please select a ShaderVariants asset first.");
            }
        }

        [MenuItem("Tools/Shader/Open ShaderVariants Collector", priority = 0)]
        static void Open()
        {
            NewCollectAllShaderVariants(withScenes: true, autoStart: false);
        }

        /// <summary>
        /// 计算场景渲染包围球
        /// </summary>
        static void FitAllRenderersBoundingSphereInView()
        {
            var camera = Camera.main;
            if (camera == null && Selection.activeGameObject != null)
            {
                if (PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) == PrefabAssetType.NotAPrefab)
                {
                    camera = Selection.activeGameObject.GetComponent<Camera>();
                }
            }
            if (camera == null)
            {
                return;
            }
            Debug.Assert(camera.orthographic == false);
            Bounds bounds;
            var renderers = new Renderer[0];
            var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            var offset = 0;
            for (int i = 0; i < roots.Length; ++i)
            {
                var _renderers = roots[i].GetComponentsInChildren<Renderer>();
                Array.Resize(ref renderers, renderers.Length + _renderers.Length);
                Array.Copy(_renderers, 0, renderers, offset, _renderers.Length);
                offset += _renderers.Length;
            }

            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; ++i)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
                var center = bounds.center;
                var radius = bounds.extents.magnitude;
                var cameraT = camera.transform;
                cameraT.LookAt(center, Vector3.up);
                var lookDir = cameraT.forward;
                if (!camera.orthographic)
                {
                    // The field of view of the camera in degrees.
                    // This is the vertical field of view
                    var angle = camera.fieldOfView * 0.5f;
                    var distance = radius / Mathf.Sin(angle * Mathf.Deg2Rad);
                    var nearPlane = distance - radius;
                    var farPlane = nearPlane + radius * 2;
                    camera.nearClipPlane = nearPlane;
                    camera.farClipPlane = farPlane;
                    cameraT.position = center - lookDir * distance;
                }
                else
                {
                    var distance = radius * 2;
                    cameraT.position = center - lookDir * distance;
                    camera.nearClipPlane = radius;
                    camera.farClipPlane = radius * 3;
                    camera.orthographicSize = radius;
                }
            }
        }

        /// <summary>
        /// 确保场景中有一个主相机，用来渲染整个场景，从而获取到当前激活的Shader变体
        /// </summary>
        /// <returns></returns>
        static Camera EnsureMainCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var go = new GameObject(TempCameraName);
                camera = go.AddComponent<Camera>();
                camera.cullingMask = -1;
                camera.gameObject.hideFlags = HideFlags.DontSave;
                camera.tag = "MainCamera";
            }
            return camera;
        }

        static void CollectAllBuildScenesShaderVariantsInScenes()
        {
            var assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type GameViewType = assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(GameViewType);
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            if (scenes.Length > 0)
            {
                var taskList = new EditorUtils.TaskActions();
                for (int i = 0; i < scenes.Length; ++i)
                {
                    var scene = scenes[i];
                    var path = scene.path;
                    if (path == LevelEditor.BlackholeScenePath || !File.Exists(path))
                    {
                        continue;
                    }
                    taskList.Add(
                        String.Format("Open Scene {0}", path),
                        () =>
                        {
                            EditorSceneManager.OpenScene(path);
                        }
                    );
                    taskList.Add(
                        String.Format("Render Scene {0}", path),
                        () =>
                        {
                            EnsureMainCamera();
                            FitAllRenderersBoundingSphereInView();
                            var gameview = EditorWindow.GetWindow(GameViewType);
                            if (gameview != null)
                            {
                                gameview.Repaint();
                            }
                        }
                    );
                    taskList.Add(String.Format("Skip Frame {0}", path), () => { });
                    taskList.Add(
                        String.Format("Close Scene {0}", path),
                        () =>
                        {
                            var curScene = EditorSceneManager.GetActiveScene();
                            EditorSceneManager.OpenScene(LevelEditor.BlackholeScenePath);
                        }
                    );
                }
                taskList.Add(
                    String.Format("Ending..."),
                    () =>
                    {
                        var shaderCount = ShaderUtils.GetCurrentShaderVariantCollectionShaderCount();
                        var shaderVariantCount = ShaderUtils.GetCurrentShaderVariantCollectionVariantCount();
                        Debug.LogFormat("Currently tracked: {0} shaders {1} total variants.",
                            shaderCount, shaderVariantCount);
                        var tempCamera = EnsureMainCamera();
                        if ((tempCamera.hideFlags & HideFlags.DontSave) != 0)
                        {
                            UDebug.Assert(tempCamera.name == TempCameraName);
                            GameObject.DestroyImmediate(tempCamera.gameObject);
                        }
                        SaveShaderVariants();
                    },
                    false
                );
                EditorUtils.DoEditorTask("Collect All EditorBuildSettings.Scenes ShaderVariants", taskList, true);
            }
        }

        [MenuItem("Tools/Shader/Clear ShaderVariant Validation Cache")]
        static void ClearShaderVariantValidationCache()
        {
            ShaderVariantValidationCache.sharedInstance.Clear();
        }

        [MenuItem("Tools/Shader/Save Current ShaderVariants")]
        static void SaveShaderVariantsGUI()
        {
            SaveShaderVariants(true, false);
        }

        [MenuItem("Tools/Shader/Save Current ShaderVariants As New")]
        static void SaveShaderVariantsAsNewGUI()
        {
            SaveShaderVariants(true);
        }

        [MenuItem("Tools/Shader/Reimport All Shaders Assets")]
        static void ReimportAllShaderAssets()
        {
            try
            {
                var all = ShaderUtil.GetAllShaderInfo();
                for (int i = 0; i < all.Length; ++i)
                {
                    UDebug.Assert(all[i].hasErrors == false);
                    var shader = Shader.Find(all[i].name);
                    if (shader != null)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Reimport Shader...", shader.name, (float)i / all.Length))
                        {
                            break;
                        }
                        var assetPath = AssetDatabase.GetAssetPath(shader);
                        if (!String.IsNullOrEmpty(assetPath) && !EditorUtils.IsUnityDefaultResource(assetPath))
                        {
                            var importer = ShaderImporter.GetAtPath(assetPath) as ShaderImporter;
                            UDebug.Assert(importer != null);
                            if (importer != null)
                            {
                                var before = AssetDatabase.GetDependencies(assetPath);
                                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.DontDownloadFromCacheServer);
                                var after = AssetDatabase.GetDependencies(assetPath);
                                if (!before.SequenceEqual(after))
                                {
                                    UDebug.LogErrorFormat("Reimport shader: {0} for error dependencies!", shader.name);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        static void _CollectShaderDependencies(String shaderAssetPath, HashSet<String> shaderPaths)
        {
            if (!String.IsNullOrEmpty(shaderAssetPath) && !EditorUtils.IsUnityDefaultResource(shaderAssetPath))
            {
                var deps = AssetDatabase.GetDependencies(shaderAssetPath);
                for (int j = 0; j < deps.Length; ++j)
                {
                    var dep = deps[j];
                    if (!String.IsNullOrEmpty(dep) && !EditorUtils.IsUnityDefaultResource(dep))
                    {
                        if (shaderPaths.Add(dep))
                        {
                            var svc = GetShaderVariantsAssetPath(dep);
                            if (!String.IsNullOrEmpty(svc))
                            {
                                shaderPaths.Add(svc);
                            }
                        }
                    }
                }
            }
        }

        [MenuItem("Tools/Shader/Sort Selected ShaderVariantsCollections")]
        static void SortSelectedShaderVariantsCollections()
        {
            var svcs = new HashSet<ShaderVariantCollection>();
            var files = EditorUtils.GetSelectedAssetPathList(typeof(ShaderVariantCollection), null);
            for (int i = 0; i < files.Count; ++i)
            {
                if (files[i].EndsWith(".shadervariants", StringComparison.OrdinalIgnoreCase))
                {
                    var svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(files[i]);
                    if (svc != null)
                    {
                        svcs.Add(svc);
                    }
                }
            }
            if (svcs.Count == 0)
            {
                UDebug.LogError("No ShaderVariantCollection assets selected.");
            }
            else
            {
                SortShaderVariantCollections(svcs.ToList());
            }
        }

        static void SortShaderVariantCollections(List<ShaderVariantCollection> svcs)
        {
            try
            {
                var i = 0;
                var count = svcs.Count;
                foreach (var svc in svcs)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Sorting Shader Variants", svc.name, (float)i / count))
                    {
                        break;
                    }
                    SortShaderVariantCollection(svc);
                    ++i;
                }
            }
            finally
            {
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
        }

        static void SaveShaderVariantCollections(Dictionary<String, SVCWithList> svcs)
        {
            try
            {
                var i = 0;
                var count = svcs.Count;
                foreach (var kv in svcs)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Sorting Shader Variants", kv.Value.name, (float)i / count))
                    {
                        break;
                    }
                    SaveShaderVariantCollection(kv.Key, kv.Value);
                    ++i;
                }
            }
            finally
            {
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
        }

        static void SortShaderVariantCollection(ShaderVariantCollection svc)
        {
            var data = ShaderVariantCollectionHelper.ExtractData(svc);
            var shaders = data.Keys.ToList();
            var shaderNames = new List<KeyValuePair<String, Shader>>();
            for (int i = 0; i < shaders.Count; ++i)
            {
                shaderNames.Add(new KeyValuePair<String, Shader>(shaders[i].name, shaders[i]));
            }
            shaderNames.Sort((l, r) => l.Key.CompareTo(r.Key));
            svc.Clear();
            for (int i = 0; i < shaderNames.Count; ++i)
            {
                var svList = data[shaderNames[i].Value];
                svList.Sort();
                for (int j = 0; j < svList.Count; ++j)
                {
                    var vc = new ShaderVariantCollection.ShaderVariant();
                    vc.shader = svList[j].shader;
                    vc.passType = svList[j].passType;
                    vc.keywords = svList[j].keywords ?? emptyStringArray;
                    svc.Add(vc);
                }
            }
            UnityEditor.EditorUtility.SetDirty(svc);
        }

        static bool SaveShaderVariantCollection(String assetPath, SVCWithList newSvc)
        {
            var curSvc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(assetPath);
            if (curSvc == null)
            {
                SortShaderVariantCollection(newSvc.svc);
                AssetDatabase.CreateAsset(newSvc.svc, assetPath);
                curSvc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(assetPath);
                UDebug.Assert(curSvc != null);
                Debug.LogFormat("Create new ShaderVariantCollection: {0}, shaderCount: {1}, variantCount = {2}",
                    assetPath, curSvc.shaderCount, curSvc.variantCount);
                UDebug.Assert(curSvc.variantCount == newSvc.variants.Count);
                return true;
            }
            else
            {
                var data = ShaderVariantCollectionHelper.ExtractData(curSvc);
                var curVariants = new List<ShaderVariantCollectionHelper.ShaderVariant>();
                foreach (var kv in data)
                {
                    if (kv.Value != null)
                    {
                        curVariants.AddRange(kv.Value);
                    }
                }
                var curCount = curVariants.Count;
                var newCount = newSvc.variants.Count;
                curVariants.Sort();
                newSvc.variants.Sort();
                if (curCount != newCount || !newSvc.variants.SequenceEqual(curVariants))
                {
                    curSvc.Clear();
                    Shader curShader = null;
                    var hasCode = true;
                    var variantCount = 0ul;
                    foreach (var v in newSvc.variants)
                    {
                        if (curShader != v.shader)
                        {
                            hasCode = ShaderUtils.HasCodeSnippets(v.shader);
                            if (hasCode)
                            {
                                variantCount = ShaderUtils.GetVariantCount(v.shader, false);
                            }
                            else
                            {
                                variantCount = 0;
                            }
                            curShader = v.shader;
                        }
                        if (variantCount > 10 * 1000000UL)
                        {
                            var sv = new ShaderVariantCollection.ShaderVariant();
                            sv.shader = v.shader;
                            sv.passType = v.passType;
                            sv.keywords = v.keywords ?? emptyStringArray;
                            if (!curSvc.Add(sv))
                            {
                                if (hasCode && !curSvc.Contains(sv))
                                {
                                    Debug.LogErrorFormat("Add ShaderVariant failed: {0}", v.ToString());
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var sv = new ShaderVariantCollection.ShaderVariant(v.shader, v.passType, v.keywords);
                                if (!curSvc.Add(sv))
                                {
                                    if (hasCode && !curSvc.Contains(sv))
                                    {
                                        Debug.LogErrorFormat("Add ShaderVariant failed: {0}", v.ToString());
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogErrorFormat("Add ShaderVariant is invalid: {0}", v.ToString());
                            }
                        }
                    }
                    UDebug.Assert(curSvc.variantCount == newSvc.variants.Count);
                    UnityEditor.EditorUtility.SetDirty(curSvc);
                    Debug.LogFormat("Update ShaderVariantCollection: {0}, shaderCount: {1}, variantCount = {2}",
                        assetPath, curSvc.shaderCount, curSvc.variantCount);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static String GetShaderVariantsAssetPath(Shader shader)
        {
            return GetShaderVariantsAssetPath(AssetDatabase.GetAssetPath(shader));
        }

        public static String GetShaderVariantsAssetPath(String shaderAssetPath)
        {
            if (String.IsNullOrEmpty(shaderAssetPath) || EditorUtils.IsUnityDefaultResource(shaderAssetPath))
            {
                return String.Empty;
            }
            var shaderVariantsPath = System.IO.Path.ChangeExtension(shaderAssetPath, ".shadervariants");
            shaderVariantsPath = shaderVariantsPath.Insert(0, "Assets/SVC/ShaderVariants/");
            return shaderVariantsPath;
        }

        static bool ShaderVariant_Equal(ref ShaderVariantCollection.ShaderVariant left, ref ShaderVariantCollection.ShaderVariant right)
        {
            if (left.shader != right.shader)
            {
                return false;
            }
            if (left.passType != right.passType)
            {
                return false;
            }
            if (object.ReferenceEquals(left.keywords, right.keywords))
            {
                return true;
            }
            var lcount = left.keywords != null ? left.keywords.Length : 0;
            var rcount = right.keywords != null ? right.keywords.Length : 0;
            if (lcount != rcount)
            {
                return false;
            }
            if (lcount > 0)
            {
                var llist = left.keywords.OrderBy(s => s);
                var rlist = right.keywords.OrderBy(s => s);
                return llist.SequenceEqual(rlist);
            }
            return false;
        }

        static bool ShaderVariant_Equal(ref ShaderVariantCollectionHelper.ShaderVariant left, ref ShaderVariantCollectionHelper.ShaderVariant right)
        {
            if (left.shader != right.shader)
            {
                return false;
            }
            if (left.passType != right.passType)
            {
                return false;
            }
            if (object.ReferenceEquals(left.keywords, right.keywords))
            {
                return true;
            }
            var lcount = left.keywords != null ? left.keywords.Length : 0;
            var rcount = right.keywords != null ? right.keywords.Length : 0;
            if (lcount != rcount)
            {
                return false;
            }
            if (lcount > 0)
            {
                var llist = left.sorted_keywords;
                var rlist = right.sorted_keywords;
                return llist.SequenceEqual(rlist);
            }
            return false;
        }

        internal class ShaderVariantValidationCache
        {

            internal struct SnippetData
            {
                public ShaderType shaderType;
                public PassType passType;
                public string passName;
            }

            internal class ShaderInfo
            {
                internal String hash;
                internal List<SnippetData> snippets;
            }

            internal class ValidateResults
            {
                internal String shaderHash = String.Empty;
                internal HashSet<String> validSet = new HashSet<String>();
                internal HashSet<String> invalidSet = new HashSet<String>();
            }

            Dictionary<String, ValidateResults> validCache = new Dictionary<String, ValidateResults>();
            Dictionary<String, ShaderInfo> shaderHashCache = new Dictionary<String, ShaderInfo>();

            internal ShaderInfo GetShaderInfo(Shader shader)
            {
                ShaderInfo info;
                var hash = HashShader(shader);
                if (shaderHashCache.TryGetValue(shader.name, out info))
                {
                    if (hash == info.hash)
                    {
                        return info;
                    }
                    UDebug.Assert(false);
                }
                return null;
            }

            internal static String HashShader(Shader shader, Dictionary<String, ShaderInfo> shaderHashCache)
            {
                ShaderInfo shaderInfo;
                if (shaderHashCache != null && shaderHashCache.TryGetValue(shader.name, out shaderInfo))
                {
                    UDebug.Assert(shaderInfo.hash != null);
                    return shaderInfo.hash;
                }
                String hash;
                var assetPath = AssetDatabase.GetAssetPath(shader);
                if (String.IsNullOrEmpty(assetPath))
                {
                    hash = String.Empty;
                }
                else
                {
                    if (EditorUtils.IsUnityDefaultResource(assetPath))
                    {
                        hash = EditorUtils.Md5String(assetPath + AssetDatabase.AssetPathToGUID(assetPath));
                    }
                    else
                    {
                        var deps = AssetDatabase.GetDependencies(assetPath).ToList();
                        deps.StableSort();
                        var sb = new StringBuilder();
                        deps.ForEach(d => sb.Append(EditorUtils.Md5Asset(d)).AppendLine());
                        hash = EditorUtils.Md5String(sb.ToString());
                    }
                }
                if (shaderHashCache != null && hash != null)
                {
                    shaderHashCache[shader.name] = new ShaderInfo { hash = hash };
                }
                UDebug.Assert(hash != null);
                return hash;
            }

            internal String HashShader(Shader shader, bool useCache = true)
            {
                return HashShader(shader, useCache ? shaderHashCache : null);
            }

            internal bool? Check(Shader shader, PassType passType, String[] keywords, out STuple<String, String, bool> cacheItem)
            {
                cacheItem = STuple.Create(String.Empty, String.Empty, false);
                UDebug.Assert(shader != null && keywords != null);
                var hash = HashShader(shader);
                if (String.IsNullOrEmpty(hash))
                {
                    UDebug.Assert(false);
                    return null;
                }
                var buff = new List<String>();
                Array.ForEach(keywords, k => buff.Add(k));
                buff.Sort();
                var value = String.Join(";", buff);
                var sb = new StringBuilder();
                sb.Append(shader.name).Append(';');
                sb.Append(hash).Append(';');
                sb.Append(passType).Append(';');
                var key = sb.ToString();
                ValidateResults vdict;
                cacheItem = STuple.Create(key, value, false);
                if (validCache.TryGetValue(key, out vdict) && vdict != null)
                {
                    if (vdict.validSet.Contains(value))
                    {
                        cacheItem = STuple.Create(key, value, true);
                        return true;
                    }
                    else if (vdict.invalidSet.Contains(value))
                    {
                        cacheItem = STuple.Create(key, value, false);
                        return false;
                    }
                }
                return null;
            }

            internal void Encache(String hash, STuple<String, String, bool> cacheItem)
            {
                try
                {
                    ValidateResults r;
                    if (!validCache.TryGetValue(cacheItem.Item1, out r))
                    {
                        r = new ValidateResults();
                        r.shaderHash = hash;
                        validCache.Add(cacheItem.Item1, r);
                    }
                    UDebug.Assert(r.shaderHash == hash);
                    if (cacheItem.Item2 != null)
                    {
                        if (cacheItem.Item3)
                        {
                            r.validSet.Add(cacheItem.Item2);
                        }
                        else
                        {
                            r.invalidSet.Add(cacheItem.Item2);
                        }
                    }
                }
                catch (Exception e)
                {
                    UDebug.LogException(e);
                }
            }

            internal bool AppendSnippet(Shader shader, SnippetData snippet)
            {
                if (String.IsNullOrEmpty(HashShader(shader)))
                {
                    return false;
                }
                ShaderInfo info;
                if (shaderHashCache.TryGetValue(shader.name, out info))
                {
                    info.snippets = info.snippets ?? new List<SnippetData>();
                    if (info.snippets.FindIndex(snippet, (e, t) => e.passName == t.passName && e.passType == t.passType && e.shaderType == t.shaderType) == -1)
                    {
                        info.snippets.Add(snippet);
                        return true;
                    }
                }
                return false;
            }

            internal void Clear()
            {
                validCache.Clear();
                shaderHashCache.Clear();
                try
                {
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                        Debug.LogFormat("Remove: {0}", FilePath);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                Debug.LogFormat("ShaderVariantValidationCache is clean.");
            }

            const String FilePath = "Assets/SVC/ToolOutput/ShaderVariantValidationCache.json";

            internal void Reload()
            {
                try
                {
                    Clear();
                    if (!File.Exists(FilePath))
                    {
                        return;
                    }
                    var removedHashes = new HashSet<String>();
                    var jo = JSONObject.Create(File.ReadAllText(FilePath));
                    var jShaderHashCache = jo.GetField("shaderHashCache");
                    if (jShaderHashCache != null && jShaderHashCache.type == JSONObject.Type.OBJECT)
                    {
                        if (jShaderHashCache.keys != null)
                        {
                            for (int i = 0; i < jShaderHashCache.keys.Count; ++i)
                            {
                                var shaderName = jShaderHashCache.keys[i];
                                var shader = Shader.Find(shaderName);
                                if (shader != null)
                                {
                                    UDebug.Assert(shaderName == shader.name);
                                    var jInfo = jShaderHashCache.list[i];
                                    if (jInfo != null && jInfo.type == JSONObject.Type.OBJECT)
                                    {
                                        String hash;
                                        jInfo.GetField(out hash, "hash", String.Empty);
                                        if (!String.IsNullOrEmpty(hash))
                                        {
                                            var newHash = HashShader(shader, false);
                                            if (!String.IsNullOrEmpty(hash) && newHash == hash)
                                            {
                                                var shaderInfo = new ShaderInfo();
                                                shaderInfo.hash = hash;
                                                var jSnippets = jInfo.GetField("snippets");
                                                if (jSnippets != null && jSnippets.type == JSONObject.Type.ARRAY && jSnippets.Count > 0)
                                                {
                                                    for (int j = 0; j < jSnippets.list.Count; ++j)
                                                    {
                                                        var o = jSnippets.list[j];
                                                        if (o != null && o.IsObject)
                                                        {
                                                            shaderInfo.snippets = shaderInfo.snippets ?? new List<SnippetData>();
                                                            string passName;
                                                            String _shaderType, _passType;
                                                            o.GetField(out passName, "passName", String.Empty);
                                                            o.GetField(out _shaderType, "shaderType", String.Empty);
                                                            o.GetField(out _passType, "passType", String.Empty);
                                                            var indexA = EnumInt32<ShaderType>.GetDefines().FindIndex(_shaderType, (e, t) => e.name == t);
                                                            var indexB = EnumInt32<PassType>.GetDefines().FindIndex(_passType, (e, t) => e.name == t);
                                                            if (indexA >= 0 && indexB >= 0)
                                                            {
                                                                try
                                                                {
                                                                    var snippet = new SnippetData();
                                                                    snippet.passName = passName;
                                                                    snippet.shaderType = (ShaderType)(EnumInt32<ShaderType>.GetDefines()[indexA].value);
                                                                    snippet.passType = (PassType)(EnumInt32<PassType>.GetDefines()[indexB].value);
                                                                    shaderInfo.snippets.Add(snippet);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    UDebug.LogException(e);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                shaderHashCache.Add(jShaderHashCache.keys[i], shaderInfo);
                                            }
                                            else
                                            {
                                                // shader资源hash变了，删除旧的记录
                                                removedHashes.Add(hash);
                                                Debug.LogWarningFormat("Skip expired shader info: {0}", shaderName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var jValidCache = jo.GetField("validCache");
                    if (jValidCache != null && jValidCache.type == JSONObject.Type.OBJECT)
                    {
                        for (int i = 0; i < jValidCache.keys.Count; ++i)
                        {
                            var key = jValidCache.keys[i];
                            var val = jValidCache.list[i];
                            if (val != null && val.IsObject)
                            {
                                String shaderHash;
                                val.GetField(out shaderHash, "shaderHash", String.Empty);
                                if (removedHashes.Contains(shaderHash))
                                {
                                    // 跳过已经无效的记录
                                    Debug.LogWarningFormat("Skip expired validation cache item: {0}", key);
                                    continue;
                                }
                                var rec = new ValidateResults();
                                rec.shaderHash = shaderHash;
                                validCache.Add(key, rec);
                                var jValidSet = val.GetField("validSet");
                                var jInvalidSet = val.GetField("invalidSet");
                                if (jValidSet != null && jValidSet.IsArray)
                                {
                                    for (int j = 0; j < jValidSet.list.Count; ++j)
                                    {
                                        if (jValidSet.list[j].IsString)
                                        {
                                            var s = jValidSet.list[j].str;
                                            if (s != null)
                                            {
                                                rec.validSet.Add(s);
                                            }
                                        }
                                    }
                                }
                                if (jInvalidSet != null && jInvalidSet.IsArray)
                                {
                                    for (int j = 0; j < jInvalidSet.list.Count; ++j)
                                    {
                                        if (jInvalidSet.list[j].IsString)
                                        {
                                            var s = jInvalidSet.list[j].str;
                                            if (s != null)
                                            {
                                                rec.invalidSet.Add(s);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    UDebug.LogException(e);
                }
            }

            internal void Save()
            {
                try
                {
                    var root = new JSONObject(JSONObject.Type.OBJECT);
                    if (shaderHashCache.Count > 0)
                    {
                        var jShaderHashCache = new JSONObject(JSONObject.Type.OBJECT);
                        foreach (var kv in shaderHashCache)
                        {
                            var jInfo = new JSONObject(JSONObject.Type.OBJECT);
                            jInfo.SetField("hash", kv.Value.hash);
                            if (kv.Value.snippets != null && kv.Value.snippets.Count > 0)
                            {
                                var jSnippets = new JSONObject(JSONObject.Type.ARRAY);
                                jInfo.SetField("snippets", jSnippets);
                                for (int i = 0; i < kv.Value.snippets.Count; ++i)
                                {
                                    var o = kv.Value.snippets[i];
                                    var jSnippet = new JSONObject(JSONObject.Type.OBJECT);
                                    jSnippet.SetField("passName", o.passName);
                                    jSnippet.SetField("shaderType", o.shaderType.ToString());
                                    jSnippet.SetField("passType", o.passType.ToString());
                                    jSnippets.Add(jSnippet);
                                }
                            }
                            jShaderHashCache.SetField(kv.Key, jInfo);
                        }
                        root.SetField("shaderHashCache", jShaderHashCache);
                    }
                    if (validCache.Count > 0)
                    {
                        var jValidCache = new JSONObject(JSONObject.Type.OBJECT);
                        root.SetField("validCache", jValidCache);
                        foreach (var kv in validCache)
                        {
                            var v = kv.Value;
                            var jValidateResults = new JSONObject(JSONObject.Type.OBJECT);
                            jValidateResults.SetField("shaderHash", v.shaderHash);
                            if (v.validSet.Count > 0)
                            {
                                var jValidSet = new JSONObject(JSONObject.Type.ARRAY);
                                jValidateResults.AddField("validSet", jValidSet);
                                var values = v.validSet.ToList();
                                values.StableSort();
                                values.ForEach(e => jValidSet.Add(e));
                            }
                            if (v.invalidSet.Count > 0)
                            {
                                var jInvalidSet = new JSONObject(JSONObject.Type.ARRAY);
                                jValidateResults.AddField("invalidSet", jInvalidSet);
                                var values = v.invalidSet.ToList();
                                values.StableSort();
                                values.ForEach(e => jInvalidSet.Add(e));
                            }
                            jValidCache.SetField(kv.Key, jValidateResults);
                        }
                    }
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileUtils.CreateDirectory(FilePath);
                    File.WriteAllText(FilePath, root.Print(true));
                }
                catch (Exception e)
                {
                    UDebug.LogException(e);
                }
            }

            internal void Remove(ShaderVariantCollectionHelper.ShaderVariant sv)
            {
            }

            static ShaderVariantValidationCache s_SVCache = new ShaderVariantValidationCache();
            internal static ShaderVariantValidationCache sharedInstance
            {
                get
                {
                    if (s_SVCache == null)
                    {
                        s_SVCache = new ShaderVariantValidationCache();
                    }
                    return s_SVCache;
                }
            }
        }

        static ShaderVariantCollection.ShaderVariant FastCreateShaderVariant(Shader shader, PassType passType, string[] keywords)
        {

            var tempKeywords = new List<string>(keywords);
            tempKeywords.Remove("FOG_LINEAR");
            tempKeywords.Remove("_DETAIL_MASKMAP_OUTLINE");
            tempKeywords.Remove("_DEPTH_NO_MSAA");
            tempKeywords.Remove("_PREPARE_NORMAL");
            tempKeywords.Remove("_MRT_PREPARE_NORMAL");
            keywords = tempKeywords.ToArray();

            Exception _e = null;
            var cacheItem = STuple.Create(String.Empty, String.Empty, false);
            var inCache = false;
            var c = ShaderVariantValidationCache.sharedInstance.Check(shader, passType, keywords, out cacheItem);
            inCache = c != null;
            if (c != null)
            {
                if (c.Value == false)
                {
                    throw new ArgumentException(String.Format("invalid keywords: '{0}', '{1}'", cacheItem.Item1, cacheItem.Item2));
                }
            }
            try
            {
                // Unity目前没有提供获取指定Pass的LightMode，真实PassType无法断定
                // 由于目前我们自定义和内置Shader在ShadowCaster这种特殊pass上都使用同一个名字
                // 所以我们可以从ShaderPassName来判断是否可能是ShadowCaster
                var validPassName = true;
                var shaderInfo = ShaderVariantValidationCache.sharedInstance.GetShaderInfo(shader);
                if (shaderInfo != null && shaderInfo.snippets != null)
                {
                    validPassName = false;
                    // 使用之前编译阶段获取到指定shader所有有效的pass类型
                    for (int s = 0; s < shaderInfo.snippets.Count; ++s)
                    {
                        var snippet = shaderInfo.snippets[s];
                        if (snippet.passType == passType)
                        {
                            var sd = ShaderUtil.GetShaderData(shader);
                            for (int i = 0; i < sd.SubshaderCount; ++i)
                            {
                                var subShader = sd.GetSubshader(i);
                                var passCount = subShader.PassCount;
                                for (int j = 0; j < passCount; ++j)
                                {
                                    if (subShader.GetPass(j).Name == snippet.passName)
                                    {
                                        validPassName = true;
                                        goto END_PASS_CHECK;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (passType == PassType.ShadowCaster)
                {
                    validPassName = false;
                    var sd = ShaderUtil.GetShaderData(shader);
                    for (int i = 0; i < sd.SubshaderCount; ++i)
                    {
                        var subShader = sd.GetSubshader(i);
                        var passCount = subShader.PassCount;
                        for (int j = 0; j < passCount; ++j)
                        {
                            var pass = subShader.GetPass(j);
                            if (pass.Name.Equals("ShadowCaster", StringComparison.OrdinalIgnoreCase) ||
                                pass.Name.Equals("Caster", StringComparison.OrdinalIgnoreCase))
                            {
                                validPassName = true;
                                goto END_PASS_CHECK;
                            }
                        }
                    }
                }
            END_PASS_CHECK:
                if (!validPassName)
                {
                    throw new ArgumentException("invalid passType for shader: {0}", shader.name);
                }
                if (inCache && c.Value)
                {
                    // 缓存中已经知道这个变体是合法的
                    var _fastSV = new ShaderVariantCollection.ShaderVariant();
                    _fastSV.shader = shader;
                    _fastSV.passType = passType;
                    _fastSV.keywords = keywords;
                    return _fastSV;
                }
                return new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);
            }
            catch (ArgumentException e)
            {
                _e = e;
            }
            finally
            {
                if (!inCache && !String.IsNullOrEmpty(cacheItem.Item1))
                {
                    cacheItem = STuple.Create(cacheItem.Item1, cacheItem.Item2, _e == null ? true : false);
                    var hash = ShaderVariantValidationCache.sharedInstance.HashShader(shader);
                    ShaderVariantValidationCache.sharedInstance.Encache(hash, cacheItem);
                }
            }
            throw new ArgumentException(_e != null ? _e.ToString() : "??");
        }

        struct SVCWithList
        {
            internal String name;
            internal ShaderVariantCollection svc;
            internal List<ShaderVariantCollectionHelper.ShaderVariant> variants;
        }

        static ShaderVariantCollection LoadOrCreateSVC(String shaderPath, Dictionary<String, SVCWithList> svcRecords, out KeyValuePair<String, SVCWithList> kv)
        {
            var shaderVariantsPath = System.IO.Path.ChangeExtension(shaderPath, ".shadervariants");
            shaderVariantsPath = shaderVariantsPath.Insert(0, "Assets/SVC/ShaderVariants/");
            if (FileUtils.CreateDirectory(shaderVariantsPath))
            {
                SVCWithList newSVC;
                ShaderVariantCollection va = null;
                if (!svcRecords.TryGetValue(shaderVariantsPath, out newSVC))
                {
                    newSVC = new SVCWithList();
                    va = new ShaderVariantCollection();
                    newSVC.variants = new List<ShaderVariantCollectionHelper.ShaderVariant>();
                    newSVC.svc = va;
                    newSVC.name = System.IO.Path.GetFileNameWithoutExtension(shaderVariantsPath);
                    svcRecords.Add(shaderVariantsPath, newSVC);
                    var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                    var shaderVariantsPatchPath = System.IO.Path.ChangeExtension(shaderPath, ".shadervariants");
                    shaderVariantsPatchPath = shaderVariantsPatchPath.Insert(0, "Assets/SVC/ShaderVariants/__patches/");
                    var patch_svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderVariantsPatchPath);
                    if (patch_svc != null && shader != null)
                    {
                        List<ShaderVariantCollectionHelper.ShaderVariant> variants;
                        var patch_variants = ShaderVariantCollectionHelper.ExtractData(patch_svc);
                        if (patch_variants.TryGetValue(shader, out variants))
                        {
                            foreach (var _sv in variants)
                            {
                                try
                                {
                                    var newV = new ShaderVariantCollection.ShaderVariant(_sv.shader, _sv.passType, _sv.keywords);
                                    if (!va.Contains(newV))
                                    {
                                        va.Add(newV);
                                        Debug.LogFormat("Patch shader variant: {0}, PassType = {1}, Keywords = '{2}'", _sv.shader.name, _sv.passType, String.Join(" ", _sv.keywords));
                                    }
                                }
                                catch (Exception e)
                                {
                                    // 创建变体失败，说明此变体不属于该Shader的这个PassType
                                    Debug.LogErrorFormat("Patch shader variant failed: {0}, PassType = {1}, Keywords = '{2}'", _sv.shader.name, _sv.passType, String.Join(" ", _sv.keywords));
                                    Debug.LogError(e.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    va = newSVC.svc;
                }
                UDebug.Assert(va != null);
                kv = new KeyValuePair<String, SVCWithList>(shaderVariantsPath, newSVC);
                return va;
            }
            kv = new KeyValuePair<string, SVCWithList>();
            return null;
        }

        static String _FormatCount(ulong count)
        {
            string result;
            if (count > 1000000000UL)
            {
                result = (count / 1000000000.0).ToString("f2") + "B";
            }
            else if (count > 1000000UL)
            {
                result = (count / 1000000.0).ToString("f2") + "M";
            }
            else if (count > 1000UL)
            {
                result = (count / 1000.0).ToString("f2") + "k";
            }
            else
            {
                result = count.ToString();
            }
            return result;
        }

        static bool _CheckExcludedShader(Shader shader, String shaderPath, ref HashSet<String> excludeShaders)
        {
            if (excludeShaders != null && excludeShaders.Contains(shaderPath))
            {
                return true;
            }
            if (shader == null)
            {
                shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
            }
            if (shader != null && ShaderUtils.HasCodeSnippets(shader))
            {
                var variantCount = ShaderUtils.GetVariantCount(shader, false);
                if (variantCount > 10 * 1000000UL)
                {
                    //if ( EditorUtils.DisplayCancelableProgressBarWithTimeout(
                    //    "Shader Variant Collector", String.Format( "[{0}]: shader variant count ({1}) is huge, continue?", shader.name, _FormatCount( variantCount ) ), 10000 ) ) {
                    //    excludeShaders = excludeShaders ?? new HashSet<String>();
                    //    excludeShaders.Add( shaderPath );
                    //    return false;       // 很大的也继续
                    //}
                    return false;
                }
            }
            return false;
        }

        static void SaveShaderVariants(bool keepTempShaderVariants = true, bool refreshAll = true,
            HashSet<String> excludeShaders = null, HashSet<String> updateOnlyShaders = null, bool useShaderVariantValidationCache = true)
        {
            if (useShaderVariantValidationCache)
            {
                ShaderVariantValidationCache.sharedInstance.Reload();
            }
            else
            {
                ShaderVariantValidationCache.sharedInstance.Clear();
            }
            ShaderUtils.ClearCache();
            var tempPath = "Assets/SVC/__temp_VariantCollection.shadervariants";
            try
            {
                if (refreshAll && File.Exists(tempPath))
                {
                    AssetDatabase.DeleteAsset(tempPath);
                }
                if (refreshAll || !File.Exists(tempPath))
                {
                    ShaderUtils.SaveCurrentShaderVariantCollection(tempPath);
                    AssetDatabase.Refresh();
                }
                var svcRecords = new Dictionary<String, SVCWithList>();
                var svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(tempPath);
                if (svc != null)
                {
                    // 当前shader变体信息缓存，忽略代码执行过程中对资源进行的任何修改
                    var shaderCombinations = new Dictionary<Shader, ShaderParsedCombinations>();
                    var shaderVariants = ShaderVariantCollectionHelper.ExtractData(svc);
                    var extraVariants = new ShaderVariantCollectionHelper.RawData();
                    var depShaders = new List<Shader>();
                    int svIndex = 0;
                    foreach (var sv in shaderVariants)
                    {
                        depShaders.Clear();
                        var shaderPath = AssetDatabase.GetAssetPath(sv.Key);
                        if (_CheckExcludedShader(sv.Key, shaderPath, ref excludeShaders))
                        {
                            continue;
                        }
                        if (updateOnlyShaders != null && updateOnlyShaders.Count > 0 && !updateOnlyShaders.Contains(shaderPath))
                        {
                            continue;
                        }
                        var deps = AssetDatabase.GetDependencies(shaderPath).ToList();
                        // 加载依赖项，以及自己的变体组合信息
                        for (int i = 0; i < deps.Count; ++i)
                        {
                            Shader depShader = null;
                            if (deps[i] == shaderPath)
                            {
                                depShader = sv.Key;
                            }
                            else
                            {
                                if (deps[i].EndsWith(".shader", StringComparison.OrdinalIgnoreCase))
                                {
                                    depShader = AssetDatabase.LoadAssetAtPath<Shader>(deps[i]);
                                    if (depShader != null)
                                    {
                                        depShaders.Add(depShader);
                                    }
                                    else
                                    {
                                        UDebug.LogErrorFormat("Load shader '{0}' failed.", deps[i]);
                                    }
                                }
                            }
                            if (depShader != null)
                            {
                                if (!shaderCombinations.ContainsKey(depShader))
                                {
                                    var info = ShaderUtils.ParseShaderCombinations(depShader, true, false);
                                    if (info != null)
                                    {
                                        shaderCombinations.Add(depShader, info);
                                    }
                                }
                            }
                        }
                        {
                            KeyValuePair<String, SVCWithList> kv;
                            var va = LoadOrCreateSVC(shaderPath, svcRecords, out kv);
                            if (va == null)
                            {
                                continue;
                            }
                            var svInfos = sv.Value;
                            svInfos.StableSort((l, r) => l.ToString().CompareTo(r.ToString()));
                            var mainProgress = (float)svIndex / shaderVariants.Count;
                            mainProgress = Mathf.FloorToInt(mainProgress * 10) / 10.0f;
                            for (int i = 0; i < svInfos.Count; ++i)
                            {
                                var _sv = svInfos[i];
                                var subProgress = (float)i / (svInfos.Count * 10);
                                var progress = mainProgress + subProgress;
                                if (EditorUtility.DisplayCancelableProgressBar(
                                    String.Format("SaveVariants[{0}]: {1}, {2}/{3}", _sv.shader.name, _sv.passType, i, svInfos.Count),
                                    String.Join("\n", _sv.keywords), progress))
                                {
                                    break;
                                }
                                try
                                {
                                    var realSV = FastCreateShaderVariant(_sv.shader, _sv.passType, _sv.keywords);
                                    if (va.Add(realSV))
                                    {
                                        kv.Value.variants.Add(_sv);
                                    }
                                }
                                catch (System.ArgumentException e)
                                {
                                    // 创建变体失败，说明此变体不属于该Shader的这个PassType
                                    Debug.LogWarningFormat("{0}, PassType = {1}, Keywords = '{2}'", _sv.shader.name, _sv.passType, String.Join(" ", _sv.keywords));
                                    Debug.LogWarning(e.ToString());
                                }
                                if (depShaders.Count > 0)
                                {
                                    // 检查此变体是否可能属于依赖项
                                    for (int j = 0; j < depShaders.Count; ++j)
                                    {
                                        ShaderParsedCombinations combs;
                                        var depShader = depShaders[j];
                                        if (shaderCombinations.TryGetValue(depShader, out combs))
                                        {
                                            var _keywords = _sv.keywords.ToList();
                                            // 删除不属于此Shader的关键字
                                            var removeCount = _keywords.RemoveAll(combs, (e, _combs) => !_combs.IsValidKeyword(e));
                                            var _newKeywords = removeCount > 0 ? _keywords.ToArray() : _sv.keywords;
                                            try
                                            {
                                                // 测试当前传入的变体是否合法，如果不合法，此构造函数会抛出参数错误的异常跳过后面操作
                                                FastCreateShaderVariant(depShader, _sv.passType, _newKeywords);
                                                // 保存到额外依赖的Shader中
                                                List<ShaderVariantCollectionHelper.ShaderVariant> _depShaderVariants;
                                                if (!extraVariants.TryGetValue(depShader, out _depShaderVariants))
                                                {
                                                    _depShaderVariants = new List<ShaderVariantCollectionHelper.ShaderVariant>();
                                                    extraVariants.Add(depShader, _depShaderVariants);
                                                }
                                                var exists = false;
                                                var newShaderVariant = new ShaderVariantCollectionHelper.ShaderVariant(
                                                        depShader, _sv.passType, _newKeywords);
                                                for (int n = 0; n < _depShaderVariants.Count; ++n)
                                                {
                                                    var v = _depShaderVariants[n];
                                                    if (ShaderVariant_Equal(ref newShaderVariant, ref v))
                                                    {
                                                        exists = true;
                                                        break;
                                                    }
                                                }
                                                if (!exists)
                                                {
                                                    _depShaderVariants.Add(newShaderVariant);
                                                }
                                            }
                                            catch (System.ArgumentException e)
                                            {
                                                Debug.LogWarningFormat("{0}, PassType = {1}, Keywords = '{2}'", depShader.name, _sv.passType, String.Join(" ", _newKeywords));
                                                Debug.LogWarning(e.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            EditorUtility.SetDirty(va);
                        }
                        ++svIndex;
                    }
                    svIndex = 0;
                    foreach (var extra in extraVariants)
                    {
                        var shaderPath = AssetDatabase.GetAssetPath(extra.Key);
                        if (_CheckExcludedShader(extra.Key, shaderPath, ref excludeShaders))
                        {
                            continue;
                        }
                        if (updateOnlyShaders != null && updateOnlyShaders.Count > 0 && !updateOnlyShaders.Contains(shaderPath))
                        {
                            continue;
                        }
                        {
                            KeyValuePair<String, SVCWithList> kv;
                            var va = LoadOrCreateSVC(shaderPath, svcRecords, out kv);
                            if (va == null)
                            {
                                continue;
                            }
                            var svInfos = extra.Value;
                            svInfos.StableSort((l, r) => l.ToString().CompareTo(r.ToString()));
                            var mainProgress = (float)svIndex / shaderVariants.Count;
                            mainProgress = Mathf.FloorToInt(mainProgress * 10) / 10.0f;
                            for (int i = 0; i < svInfos.Count; ++i)
                            {
                                var _sv = svInfos[i];
                                var subProgress = (float)i / (svInfos.Count * 10);
                                var progress = mainProgress + subProgress;
                                if (EditorUtility.DisplayCancelableProgressBar(
                                    String.Format("SaveExtraVariants[{0}]: {1}, {2}/{3}", _sv.shader.name, _sv.passType, i, svInfos.Count),
                                    String.Join("\n", _sv.keywords), progress))
                                {
                                    break;
                                }
                                try
                                {
                                    var realSV = FastCreateShaderVariant(_sv.shader, _sv.passType, _sv.keywords);
                                    if (va.Add(realSV))
                                    {
                                        kv.Value.variants.Add(_sv);
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    Debug.LogWarningFormat("{0}, PassType = {1}, Keywords = '{2}'", _sv.shader.name, _sv.passType, String.Join(" ", _sv.keywords));
                                    // 创建变体失败，说明此变体不属于该Shader的这个PassType
                                }
                            }
                        }
                        ++svIndex;
                    }
                    UpdateShaderCollection();
                }
                SaveShaderVariantCollections(svcRecords);
            }
            finally
            {
                ShaderVariantValidationCache.sharedInstance.Save();
                if (File.Exists(tempPath))
                {
                    if (!keepTempShaderVariants)
                    {
                        AssetDatabase.DeleteAsset(tempPath);
                    }
                }
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Tools/Shader/Dump All Dynamic Lighting Materials")]
        static void DumpAllDynamicLightingMaterials()
        {
            _CollectDynamicLightingMaterials();
        }

        [MenuItem("Tools/Shader/Collect All ShaderVariants")]
        public static void CollectAllShaderVariants()
        {
            NewCollectAllShaderVariants(withScenes: true, autoStart: true);
        }

        /// <summary>
        /// 搜集流程，供调试使用
        /// </summary>
        [MenuItem("Tools/Shader/Collect All ShaderVariants-DebugBypass")]
        public static void CollectAllShaderVariants_DebugBypass()
        {
            NewCollectAllShaderVariants(withScenes: true, autoStart: true, debugBypass: true);
        }

        /// <summary>
        /// 把搜集结果保存到Prefab中
        /// </summary>
        [MenuItem("Tools/Shader/Update Shader Collection")]
        static void UpdateShaderCollection()
        {
#if UNITY_EDITOR
            var collection = AssetDatabase.LoadAssetAtPath<GameObject>(ShaderCollectionPath);
            if (collection != null)
            {
                var isNew = false;
                if (UnityUtils.RequireComponent<ShaderCollection>(collection, out isNew).UpdateList() || isNew)
                {
                    EditorUtility.SetDirty(collection);
                    AssetDatabase.SaveAssets();
                    Debug.LogFormat("ShaderCollection is updated.");
                }
                else
                {
                    Debug.LogFormat("ShaderCollection is up-to-date.");
                }
            }
#endif
        }

        static bool _enableDebugger = false;
        static bool _breakDebugger = false;
        static EditorCoroutines.EditorCoroutine _currentTask = null;
        static EditorWindow _taskOwner = null;

        public static bool IsWorking()
        {
            return _taskOwner != null && _currentTask != null && _currentTask.finished == false;
        }

        public static void Stop()
        {
            try
            {
                if (_taskOwner != null && _currentTask != null && _currentTask.finished == false)
                {
                    _taskOwner.StopCoroutine(_currentTask.routine);
                }
            }
            catch (Exception e)
            {
                UDebug.LogException(e);
            }
            finally
            {
                _taskOwner = null;
                _currentTask = null;
            }
        }

        static void NewCollectAllShaderVariants(bool withScenes = true, bool autoStart = false, bool debugBypass = false, bool useShaderVariantValidationCache = true)
        {
            var window = EditorWindow.GetWindow(typeof(CustomMessageBox), true, "Collect All ShaderVariants") as CustomMessageBox;
            if (window != null)
            {
                var excludeShaders = new List<String>();
                excludeShaders.Add("Packages/com.unity.test-framework.performance/Editor/TestReportGraph/TestReportShader.shader");

                var updateOnlyShaders = new List<String>();
                var excludeShaders_pos = Vector2.zero;
                var updateOnlyShaders_pos = Vector2.zero;
                var includeScenes = withScenes;
                var _useShaderVariantValidationCache = useShaderVariantValidationCache;
                Shader curExcludeShader = null;
                Shader curUpdateOnlyShader = null;
                EditorUtility.ClearProgressBar();
                EditorCoroutines.EditorCoroutine it = null;
                window.Info = "Render Frame";
                window.minSize = new Vector2(400, 400);
                window.maxSize = new Vector2(600, 800);
                window.Show();
                window.OnClose = (int button, int returnValue) =>
                {
                    window.StopAllCoroutines();
                    EditorUtility.ClearProgressBar();
                };
                if (_enableDebugger)
                {
                    _breakDebugger = true;
                }
                else
                {
                    _breakDebugger = false;
                }
                window.OnGUIFunc = () =>
                {
                    includeScenes = EditorGUILayout.Toggle("Include Scenes", includeScenes);
                    GUI.enabled = true;
                    var __enableDebugger = EditorGUILayout.Toggle("Enable Debugger", _enableDebugger);
                    if (__enableDebugger != _enableDebugger)
                    {
                        _enableDebugger = __enableDebugger;
                        _breakDebugger = _enableDebugger;
                    }
                    _useShaderVariantValidationCache = EditorGUILayout.Toggle("Use SVCache", _useShaderVariantValidationCache);
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = _breakDebugger;
                    if (GUILayout.Button("Continue"))
                    {
                        _breakDebugger = false;
                    }
                    GUI.enabled = true;
                    if (it == null)
                    {
                        if (GUILayout.Button("Start"))
                        {
                            it = window.StartCoroutine(ShaderVariantsCollector(includeScenes, excludeShaders, updateOnlyShaders));
                            _currentTask = it;
                            _taskOwner = window;
                        }
                    }
                    else
                    {
                        GUI.enabled = it != null && it.finished;
                        if (GUILayout.Button("Close"))
                        {
                            window.Close();
                        }
                    }
                    if (GUILayout.Button("Save as new"))
                    {
                        var _excludeShaders = excludeShaders != null ? new HashSet<String>(excludeShaders) : new HashSet<String>();
                        var _updateOnlyShaders = updateOnlyShaders != null ? new HashSet<String>(updateOnlyShaders) : new HashSet<String>();
                        SaveShaderVariants(true,
                            excludeShaders: _excludeShaders,
                            updateOnlyShaders: _updateOnlyShaders,
                            useShaderVariantValidationCache: _useShaderVariantValidationCache);
                    }
                    if (GUILayout.Button("Save"))
                    {
                        var _excludeShaders = excludeShaders != null ? new HashSet<String>(excludeShaders) : new HashSet<String>();
                        var _updateOnlyShaders = updateOnlyShaders != null ? new HashSet<String>(updateOnlyShaders) : new HashSet<String>();
                        SaveShaderVariants(true, false,
                            excludeShaders: _excludeShaders,
                            updateOnlyShaders: _updateOnlyShaders,
                            useShaderVariantValidationCache: _useShaderVariantValidationCache);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Update Shaders:");
                        {
                            EditorGUILayout.BeginHorizontal();
                            curUpdateOnlyShader = EditorGUILayout.ObjectField(curUpdateOnlyShader, typeof(Shader), false) as Shader;
                            if (GUILayout.Button("+", GUILayout.Width(32)))
                            {
                                if (curUpdateOnlyShader != null)
                                {
                                    var path = AssetDatabase.GetAssetPath(curUpdateOnlyShader);
                                    if (!String.IsNullOrEmpty(path) && !EditorUtils.IsUnityDefaultResource(path))
                                    {
                                        var deps = AssetDatabase.GetDependencies(path);
                                        Array.ForEach(deps,
                                            p =>
                                            {
                                                if (!updateOnlyShaders.Contains(p))
                                                {
                                                    updateOnlyShaders.Add(p);
                                                }
                                            }
                                        );
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        updateOnlyShaders_pos = EditorGUILayout.BeginScrollView(updateOnlyShaders_pos);
                        for (int i = 0; i < updateOnlyShaders.Count; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            var shader = EditorGUILayout.TextField(updateOnlyShaders[i]);
                            if (GUILayout.Button("-", GUILayout.Width(32)))
                            {
                                updateOnlyShaders.RemoveAt(i);
                                --i;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    {
                        EditorGUILayout.LabelField("Exclude Shaders:");
                        {
                            EditorGUILayout.BeginHorizontal();
                            curExcludeShader = EditorGUILayout.ObjectField(curExcludeShader, typeof(Shader), false) as Shader;
                            if (GUILayout.Button("+", GUILayout.Width(32)))
                            {
                                if (curExcludeShader != null)
                                {
                                    var path = AssetDatabase.GetAssetPath(curExcludeShader);
                                    if (!String.IsNullOrEmpty(path) && !EditorUtils.IsUnityDefaultResource(path))
                                    {
                                        var deps = AssetDatabase.GetDependencies(path);
                                        Array.ForEach(deps,
                                            p =>
                                            {
                                                if (!excludeShaders.Contains(p))
                                                {
                                                    excludeShaders.Add(p);
                                                }
                                            }
                                        );
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        excludeShaders_pos = EditorGUILayout.BeginScrollView(excludeShaders_pos);
                        for (int i = 0; i < excludeShaders.Count; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            var shader = EditorGUILayout.TextField(excludeShaders[i]);
                            if (GUILayout.Button("-", GUILayout.Width(32)))
                            {
                                excludeShaders.RemoveAt(i);
                                --i;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    return 0;
                };
                if (autoStart)
                {
                    window.OnGUIFunc = null;
                    _currentTask = window.StartCoroutine(
                        ShaderVariantsCollector(
                            includeScenes, excludeShaders,
                            updateOnlyShaders, debugBypass, useShaderVariantValidationCache
                        )
                    );
                    _taskOwner = window;
                }
            }
        }

        static Transform _CreateProxyRenderers(out List<Renderer> renderAbles)
        {
            renderAbles = new List<Renderer>();
            var tempRoot = new GameObject("__temp_root").transform;
            tempRoot.hideFlags |= HideFlags.DontSave;
            // 创建用于材质渲染的几何体，渲染代理球直径1，间距为2
            var offset = new Vector3(-8, -8, -8);
            offset.y += 8 * 2 + 2;
            for (int k = 0; k < 8; ++k)
            {
                for (int j = 0; j < 8; ++j)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        var pos = new Vector3(i * 2, j * 2, k * 2);
                        pos += offset;
                        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        go.transform.parent = tempRoot;
                        go.transform.position = pos;
                        go.name = String.Format("temp_renderer_{0}_{1}_{2}", i, j, k);
                        go.hideFlags |= HideFlags.DontSave;
                        var renderer = go.GetComponent<Renderer>();
                        renderAbles.Add(renderer);

                        renderer.receiveShadows = false;
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    }
                }
            }
            var camera = EnsureMainCamera();
            if (camera.CompareTag("MainCamera") && camera.gameObject.hideFlags == HideFlags.DontSave)
            {
                camera.transform.parent = tempRoot;
            }
            FitAllRenderersBoundingSphereInView();
            return tempRoot;
        }

        /// <summary>
        /// 用编辑器协程实现的整个变体搜集过程
        /// </summary>
        /// <param name="includeScenes"></param>
        /// <param name="_excludeShaders"></param>
        /// <param name="_updateOnlyShaders"></param>
        /// <param name="debugBypass"></param>
        /// <param name="useShaderVariantValidationCache"></param>
        /// <returns></returns>
        static IEnumerator ShaderVariantsCollector(bool includeScenes, List<String> _excludeShaders, List<String> _updateOnlyShaders, bool debugBypass = false, bool useShaderVariantValidationCache = true)
        {
            var excludeShaders = _excludeShaders != null ? new HashSet<String>(_excludeShaders) : new HashSet<String>();
            var updateOnlyShaders = _updateOnlyShaders != null ? new HashSet<String>(_updateOnlyShaders) : new HashSet<String>();
            if (!debugBypass)
            {
                ReimportAllShaderAssets();
            }
            LevelEditor.RefreshBuildScenes();
            EditorUtils.ClearConsole();
            EditorUtils.ShowDebugConsole();
            var assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type GameViewType = assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(GameViewType);
            QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowCascades = 0;
            QualitySettings.softParticles = false;
            QualitySettings.softVegetation = false;
            ShaderUtils.ClearCurrentShaderVariantCollection();
            yield return null;
            List<RendererParams> dynamicMaterials;
            if (debugBypass)
            {
                dynamicMaterials = new List<RendererParams>();
            }
            else
            {
                dynamicMaterials = _CollectDynamicLightingMaterials().Values.ToList();
            }
            yield return null;
            if (!debugBypass)
            {
                try
                {
                    // TODO: 收集所有代码中可能会创建的各种材质效果变种
                    /*
                    GraphicalEffectEditorUtils.sharedInstance.CollectShaderVariantForSpecialEffect(
                        ( state, mat ) => {
                            var _mat = UnityEngine.Object.Instantiate<Material>( mat );
                            UnityEngine.Object.DontDestroyOnLoad( _mat );
                            var rp = new RendererParams {
                                receiveShadows = state.rendererState.receiveShadow,
                                shadowCastingMode = state.rendererState.castShadow,
                                lightProbeUsage = state.rendererState.lightProbeUsage,
                                reflectionProbeUsage = state.rendererState.reflectionProbeUsage,
                                motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion,
                                instancedMaterial = _mat,
                                instancedMaterialShader = _mat.shader,
                                instancedMaterialKeywords = _mat.shaderKeywords
                            };
                            dynamicMaterials.Add( rp );
                        }
                    );*/
                }
                catch (Exception e)
                {
                    UDebug.LogException(e);
                }
            }
            yield return null;
            // 整个项目所需要支持的各种光照烘焙模式，通过把项目中所有动态对象（如：角色，特效等）的材质放入这些场景中渲染一次，获取相应变体
            var realtimeScenePath = "Packages/com.knight.shadervariant/Runtime/LightEnvs/realtime.unity";
            var lightEnvPathList = new List<String>(
                new String[] {
                    realtimeScenePath,
                    "Packages/com.knight.shadervariant/Runtime/LightEnvs/subtractive.unity"
                }
            );
            EditorSceneManager.OpenScene(LevelEditor.BlackholeScenePath);

            // 专门收集高清模型，展示模型在各个光照环境下的变体
            for (int envIndex = 0; envIndex < lightEnvPathList.Count; ++envIndex)
            {
                var lightEnvPath = lightEnvPathList[envIndex];
                EditorSceneManager.OpenScene(lightEnvPath);
                if (EditorSceneManager.GetActiveScene().path != lightEnvPath)
                {
                    continue;
                }
                var lightEnv = UnityEngine.Object.FindFirstObjectByType<LightEnv>();
                if (lightEnv == null)
                {
                    continue;
                }
                List<Renderer> renderAbles;
                _CreateProxyRenderers(out renderAbles);
                for (int i = 0; i < renderAbles.Count; ++i)
                {
                    renderAbles[i].gameObject.SetActive(false);
                }
                var shadowDistance = QualitySettings.shadowDistance;
                QualitySettings.shadowDistance = 1000;
                while (lightEnv.SwitchStatus())
                {
                    yield return null;
                    var _breakState = _breakDebugger;
                    while (_enableDebugger && _breakDebugger)
                    {
                        yield return null;
                    }
                    var _dynamicMaterials = new Queue<RendererParams>(dynamicMaterials);
                    var batchIndex = 0;
                    while (_dynamicMaterials.Count > 0)
                    {
                        var cur = _dynamicMaterials.Dequeue();
                        var renderer = renderAbles[batchIndex];
                        if (cur.Apply(renderer))
                        {
                            renderer.gameObject.SetActive(true);
                            batchIndex++;
                        }
                        if (batchIndex >= renderAbles.Count)
                        {
                            batchIndex = 0;
                            yield return null;
                            if (gameView != null)
                            {
                                gameView.Repaint();
                            }
                        }
                    }
                    if (batchIndex > 0)
                    {
                        yield return null;
                        for (int s = batchIndex; s < renderAbles.Count; ++s)
                        {
                            renderAbles[s].gameObject.SetActive(false);
                        }
                        if (gameView != null)
                        {
                            gameView.Repaint();
                        }
                    }
                    if (_enableDebugger)
                    {
                        _breakDebugger = _breakState;
                    }
                    else
                    {
                        _breakDebugger = false;
                    }
                    var shaderCount = ShaderUtils.GetCurrentShaderVariantCollectionShaderCount();
                    var shaderVariantCount = ShaderUtils.GetCurrentShaderVariantCollectionVariantCount();
                    Debug.LogFormat("Currently tracked: {0} shaders {1} total variants.",
                        shaderCount, shaderVariantCount);
                }
                QualitySettings.shadowDistance = shadowDistance;
                yield return new WaitForSeconds(1);
            }

            yield return null;
            dynamicMaterials.ForEach(
                rp =>
                {
                    if (rp.instancedMaterial != null)
                    {
                        UnityEngine.Object.DestroyImmediate(rp.instancedMaterial);
                        rp.instancedMaterial = null;
                    }
                }
            );
            EditorSceneManager.OpenScene(LevelEditor.BlackholeScenePath);

            // 收集整个工程下的所有材质，在动态环境下的变体
            EditorSceneManager.OpenScene(realtimeScenePath);
            if (!debugBypass)
            {

                var allMaterials = CollectAllMaterialAssetsForGame();

                List<Renderer> renderAbles;
                _CreateProxyRenderers(out renderAbles);
                for (int i = 0; i < renderAbles.Count; ++i)
                {
                    renderAbles[i].gameObject.SetActive(false);
                }
                var lightEnv = UnityEngine.Object.FindFirstObjectByType<LightEnv>();
                if (lightEnv != null)
                {
                    var shadowDistance = QualitySettings.shadowDistance;
                    QualitySettings.shadowDistance = 1000;
                    while (lightEnv.SwitchStatus())
                    {
                        yield return null;
                        var _breakState = _breakDebugger;
                        while (_enableDebugger && _breakDebugger)
                        {
                            yield return null;
                        }
                        var _dynamicMaterials = new Queue<String>(allMaterials);
                        var batchIndex = 0;
                        while (_dynamicMaterials.Count > 0)
                        {
                            var cur = _dynamicMaterials.Dequeue();
                            var mat = AssetDatabase.LoadAssetAtPath<Material>(cur);
                            if (mat == null)
                            {
                                continue;
                            }
                            var renderer = renderAbles[batchIndex];

                            renderer.sharedMaterial = mat;
                            renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                            renderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                            renderer.shadowCastingMode = ShadowCastingMode.On;
                            renderer.receiveShadows = true;

                            renderer.gameObject.SetActive(true);
                            batchIndex++;

                            if (batchIndex >= renderAbles.Count)
                            {
                                batchIndex = 0;
                                yield return null;
                                if (gameView != null)
                                {
                                    gameView.Repaint();
                                }
                            }
                        }
                        if (batchIndex > 0)
                        {
                            yield return null;
                            for (int s = batchIndex; s < renderAbles.Count; ++s)
                            {
                                renderAbles[s].gameObject.SetActive(false);
                            }
                            if (gameView != null)
                            {
                                gameView.Repaint();
                            }
                        }
                        if (_enableDebugger)
                        {
                            _breakDebugger = _breakState;
                        }
                        else
                        {
                            _breakDebugger = false;
                        }
                        var shaderCount = ShaderUtils.GetCurrentShaderVariantCollectionShaderCount();
                        var shaderVariantCount = ShaderUtils.GetCurrentShaderVariantCollectionVariantCount();
                        Debug.LogFormat("Currently tracked: {0} shaders {1} total variants.",
                            shaderCount, shaderVariantCount);
                    }
                    QualitySettings.shadowDistance = shadowDistance;
                    yield return new WaitForSeconds(1);
                }
            }

            EditorSceneManager.OpenScene(LevelEditor.BlackholeScenePath);
            if (includeScenes)
            {
                var rSceneGUIDs = AssetDatabase.FindAssets("t:Scene", new string[]
                {
                    "Assets/GameAssets/Scene"
                });
                var rBuildScenePaths = new List<string>();
                for (int i = 0; i < rSceneGUIDs.Length; i++)
                {
                    var rSceneAssetPath = AssetDatabase.GUIDToAssetPath(rSceneGUIDs[i]);
                    rBuildScenePaths.Add(rSceneAssetPath);
                }
                if (rBuildScenePaths.Count > 0)
                {
                    var taskList = new EditorUtils.TaskActions();
                    for (int i = 0; i < rBuildScenePaths.Count; ++i)
                    {
                        var path = rBuildScenePaths[i];
                        if (path == LevelEditor.BlackholeScenePath || !File.Exists(path))
                        {
                            continue;
                        }
                        EditorSceneManager.OpenScene(path);
                        yield return null;
                        List<GameObject> extraDynamicPrefabs = null;
                        /*
                        var extraDynamicAssetPathLists = UnityEngine.Object.FindObjectsOfType<GOUtility.GOU_AssetReferencePathList>();
                        if ( extraDynamicAssetPathLists.Length > 0 ) {
                            var tag = typeof( ShaderVariantCollectionExporter ).Name;
                            for ( int j = 0; j < extraDynamicAssetPathLists.Length; ++j ) {
                                var pathList = extraDynamicAssetPathLists[ j ];
                                if ( pathList != null &&
                                    String.Equals( pathList.m_tag, tag, StringComparison.OrdinalIgnoreCase ) &&
                                    pathList.m_pathList != null && pathList.m_pathList.Count > 0 ) {
                                    for ( int k = 0; k < pathList.m_pathList.Count; ++k ) {
                                        var pathref = pathList.m_pathList[ k ];
                                        if ( pathref.loaderType == GOUtility.LoaderType.Prefab && !String.IsNullOrEmpty( pathref.fullPath ) ) {
                                            var assetPath = pathref.fullPath;
                                            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>( assetPath );
                                            if ( prefab != null ) {
                                                extraDynamicPrefabs = extraDynamicPrefabs ?? new List<GameObject>();
                                                prefab = GameObject.Instantiate<GameObject>( prefab );
                                                extraDynamicPrefabs.Add( prefab );
                                                using ( UDebug.ChangeColor( ColorIndex.Yellow ) ) {
                                                    Debug.LogFormat( "Load extra prefab: {0}", assetPath );
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }*/
                        /*
                        var sceneObjectCaches = UnityEngine.Object.FindObjectsOfType<SceneObjectCache>();
                        if ( sceneObjectCaches.Length > 0 ) {
                            var sb = StringUtils.newStringBuilder;
                            if ( sceneObjectCaches.Length > 1 ) {
                                for ( int j = 0; j < sceneObjectCaches.Length; ++j ) {
                                    var o = sceneObjectCaches[ j ];
                                    sb.Append( UnityUtils.GetHierarchyPath( o.transform ) );
                                    sb.AppendLine();
                                }
                                var msg = StringUtils.LazyFormatedString.Create(
                                    "Multiple({0}) SceneObjectCaches have been found, please remove the redundants:\n{1}",
                                    sceneObjectCaches.Length, sb.ToString()
                                );
                                UDebug.Ensure( sceneObjectCaches.Length == 1, msg );
                            }
                            try {
                                var terrainRenderers = sceneObjectCaches[ 0 ].terrainRenderers;
                                var shadowReceivers = sceneObjectCaches[ 0 ].shadowReceivers;
                                for ( int j = 0; j < shadowReceivers.Length; ++j ) {
                                    var go = SceneObjectCache.CreateClonedRenderer( shadowReceivers[ j ] );
                                    if ( go != null ) {
                                        extraDynamicPrefabs.Add( go );
                                    }
                                }
                                for ( int j = 0; j < terrainRenderers.Length; ++j ) {
                                    var go = SceneObjectCache.CreateClonedRenderer( terrainRenderers[ j ] );
                                    if ( go != null ) {
                                        extraDynamicPrefabs.Add( go );
                                    }
                                }
                            } catch ( Exception e ) {
                                UDebug.LogException( e );
                            }
                        }*/
                        yield return null;
                        EnsureMainCamera();
                        FitAllRenderersBoundingSphereInView();
                        if (gameView != null)
                        {
                            gameView.Repaint();
                        }
                        yield return null;
                        var shaderCount = ShaderUtils.GetCurrentShaderVariantCollectionShaderCount();
                        var shaderVariantCount = ShaderUtils.GetCurrentShaderVariantCollectionVariantCount();
                        Debug.LogFormat("Currently tracked: {0} shaders {1} total variants.",
                            shaderCount, shaderVariantCount);
                        var tempCamera = EnsureMainCamera();
                        if ((tempCamera.hideFlags & HideFlags.DontSave) != 0)
                        {
                            UDebug.Assert(tempCamera.name == TempCameraName);
                            GameObject.DestroyImmediate(tempCamera.gameObject);
                        }
                        if (extraDynamicPrefabs != null)
                        {
                            extraDynamicPrefabs.ForEach(o => UnityEngine.Object.DestroyImmediate(o));
                        }
                    }
                }
                EditorSceneManager.OpenScene(LevelEditor.BlackholeScenePath);
            }
            if (!debugBypass)
            {
                SaveShaderVariants(
                    excludeShaders: excludeShaders,
                    updateOnlyShaders: updateOnlyShaders,
                    useShaderVariantValidationCache: useShaderVariantValidationCache);
            }
        }
    }
}
//EOF