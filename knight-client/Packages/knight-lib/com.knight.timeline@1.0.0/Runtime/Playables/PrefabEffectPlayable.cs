using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UObject = UnityEngine.Object;

namespace Knight.Framework.Timeline
{
    /// <summary>
    /// Playable that controls and instantiates a Prefab.
    /// </summary>
    public class PrefabEffectPlayable : PlayableBehaviour
    {
        GameObject m_Instance;
        GameObject m_PrefabGameObject;

#if UNITY_EDITOR
        private bool m_IsActiveCached;
#endif

        public static IControlPlayableAllocator sAllocator = new DefaultPrefabAllocator();

        /// <summary>
        /// Creates a Playable with a PrefabControlPlayable behaviour attached
        /// </summary>
        /// <param name="graph">The PlayableGraph to inject the Playable into.</param>
        /// <param name="prefabGameObject">The prefab to instantiate from</param>
        /// <param name="parentTransform">Transform to parent instance to. Can be null.</param>
        /// <returns>Returns a Playabe with PrefabControlPlayable behaviour attached.</returns>
        public static ScriptPlayable<PrefabEffectPlayable> Create(PlayableGraph graph, GameObject prefabGameObject, Transform parentTransform, TransformSyncData syncData, int layer)
        {
            if (prefabGameObject == null)
                return ScriptPlayable<PrefabEffectPlayable>.Null;

            var handle = ScriptPlayable<PrefabEffectPlayable>.Create(graph);
            handle.GetBehaviour().Initialize(graph, prefabGameObject, parentTransform, syncData, layer);
            return handle;
        }

        /// <summary>
        /// The instance of the prefab created by this behaviour
        /// </summary>
        public GameObject prefabInstance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// Initializes the behaviour with a prefab and parent transform
        /// </summary>
        /// <param name="prefabGameObject">The prefab to instantiate from</param>
        /// <param name="parentTransform">Transform to parent instance to. Can be null.</param>
        /// <returns>The created instance</returns>
        public GameObject Initialize(PlayableGraph graph, GameObject prefabGameObject, Transform parentTransform, TransformSyncData syncData, int layer)
        {
            if (prefabGameObject == null)
                throw new ArgumentNullException("Prefab cannot be null");

            if (m_Instance != null)
            {
                Debug.LogWarningFormat("Prefab Control Playable ({0}) has already been initialized with a Prefab ({1}).", prefabGameObject.name, m_Instance.name);
            }
            else
            {
                if (syncData == null)
                {
                    m_Instance = sAllocator.Alloc(graph, prefabGameObject, parentTransform, false);
                    m_Instance.SetActive(false);
                }
                else
                {
                    if (syncData.IsLinkTransformAsChild)
                    {
                        m_Instance = sAllocator.Alloc(graph, prefabGameObject, parentTransform);
                    }
                    else
                    {
                        m_Instance = sAllocator.Alloc(graph, prefabGameObject);
                    }
                    m_Instance.SetActive(false);

                    // 初始化
                    if (parentTransform != null)
                    {
                        m_Instance.transform.position = parentTransform.position + syncData.PositionOffset;
                        m_Instance.transform.eulerAngles = parentTransform.eulerAngles + syncData.RotateOffset;
                        m_Instance.transform.localScale = parentTransform.localScale + syncData.ScaleOffset;
                    }
                    // 添加同步脚本
                    var rTransSync = m_Instance.GetComponent<TransformSync>();
                    if (!rTransSync)
                    {
                        rTransSync = m_Instance.AddComponent<TransformSync>();
                    }

                    rTransSync.Target = parentTransform;
                    rTransSync.SyncData = new TransformSyncData();
                    rTransSync.SyncData.IsFollowPosition = syncData.IsFollowPosition;
                    rTransSync.SyncData.IsIgnoreOffsetY = syncData.IsIgnoreOffsetY;
                    rTransSync.SyncData.IsPositionLocal = syncData.IsPositionLocal;
                    rTransSync.SyncData.PositionOffset = syncData.PositionOffset;
                    rTransSync.SyncData.IsFollowRotation = syncData.IsFollowRotation;
                    rTransSync.SyncData.RotateOffset = syncData.RotateOffset;
                    rTransSync.SyncData.IsFollowScale = syncData.IsFollowScale;
                    rTransSync.SyncData.ScaleOffset = syncData.ScaleOffset;
                    rTransSync.SyncData.IsFollowInUpdate = syncData.IsFollowInUpdate;
                    rTransSync.SyncData.IsLinkTransformAsChild = syncData.IsLinkTransformAsChild;
                    rTransSync.SyncData.IsFollowScaleInit = syncData.IsFollowScaleInit;
                    rTransSync.SyncData.IsFollowRotationInit = syncData.IsFollowRotationInit;
                    rTransSync.SetInitialTransform();
                }

                m_PrefabGameObject = prefabGameObject;
                //m_Instance.name = prefabGameObject.name + " [Timeline]";
                //SetHideFlagsRecursive(m_Instance);
                SetLayerRecursive(m_Instance, layer);
            }
            return m_Instance;
        }

        /// <summary>
        /// This function is called when the Playable that owns the PlayableBehaviour is destroyed.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_Instance)
            {
                sAllocator.Free(playable.GetGraph(), m_PrefabGameObject, m_Instance);
            }

#if UNITY_EDITOR
            UnityEditor.PrefabUtility.prefabInstanceUpdated -= OnPrefabUpdated;
#endif
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to Playables.PlayState.Playing.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_Instance == null)
                return;

            var rTransSync = m_Instance.GetComponent<TransformSync>();
            if (rTransSync)
            {
                rTransSync.SetInitialTransform();
            }
            var rTrailRenderers = m_Instance.GetComponentsInChildren<TrailRenderer>(true);
            for (int i = 0; i < rTrailRenderers.Length; i++)
            {
                if (rTrailRenderers[i])
                    rTrailRenderers[i].Clear();
            }
            m_Instance.SetActive(true);

#if UNITY_EDITOR
            m_IsActiveCached = true;
#endif
        }

        /// <summary>
        /// This function is called when the Playable play state is changed to PlayState.Paused.
        /// </summary>
        /// <param name="playable">The playable this behaviour is attached to.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // OnBehaviourPause can be called if the graph is stopped for a variety of reasons
            //  the effectivePlayState will test if the pause is due to the clip being out of bounds
            if (m_Instance != null && info.effectivePlayState == PlayState.Paused)
            {
                m_Instance.SetActive(false);
#if UNITY_EDITOR
                m_IsActiveCached = false;
#endif
            }
        }

#if UNITY_EDITOR
        void OnPrefabUpdated(GameObject go)
        {
            if (go == m_Instance)
            {
                SetHideFlagsRecursive(go);
                go.SetActive(m_IsActiveCached);
            }
        }

#endif

        static void SetHideFlagsRecursive(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            gameObject.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            if (!Application.isPlaying)
                gameObject.hideFlags |= HideFlags.HideInHierarchy;
            foreach (Transform child in gameObject.transform)
            {
                SetHideFlagsRecursive(child.gameObject);
            }
        }

        static void SetLayerRecursive(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }

    public interface IControlPlayableAllocator
    {
        GameObject Alloc(PlayableGraph graph, GameObject prefab, Transform parent = null, bool worldPositionStays = true);
        void Free(PlayableGraph graph, GameObject prefab, GameObject instance);
        List<GameObject> GetEffectObjects(PlayableGraph graph);
        void FreeAll();
    }

    class DefaultPrefabAllocator : IControlPlayableAllocator
    {
        public GameObject Alloc(PlayableGraph graph, GameObject prefab, Transform parent = null, bool worldPositionStays = true)
        {
            return UObject.Instantiate(prefab, parent, worldPositionStays);
        }

        public void Free(PlayableGraph graph, GameObject prefab, GameObject instance)
        {
            if (!Application.isPlaying)
            {
                UObject.DestroyImmediate(instance);
            }
            else
            {
                if (!instance.activeSelf)
                    UObject.Destroy(instance);
            }
        }

        public List<GameObject> GetEffectObjects(PlayableGraph graph)
        {
            return new List<GameObject>();
        }

        public void FreeAll()
        {
        }

    }
}
