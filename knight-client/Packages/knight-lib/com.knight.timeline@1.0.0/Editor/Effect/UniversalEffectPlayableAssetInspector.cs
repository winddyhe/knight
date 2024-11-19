using Knight.Framework.Timeline;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Knight.Framework.Timeline.Editor
{

    [CustomEditor(typeof(UniversalEffectPlayableAsset)), CanEditMultipleObjects]
    public class UniversalEffectPlayableAssetInspector : UnityEditor.Editor
    {
        static class Styles
        {
            static string s_DisabledBecauseOfSelfControlTooltip = "Must be disabled when the Source Game Object references the same PlayableDirector component that is being controlled";
            public static readonly GUIContent activationContent = EditorGUIUtility.TrTextContent("Control Activation", "When checked the clip will control the active state of the source game object");
            public static readonly GUIContent activationDisabledContent = GUIUtility.TextContent("Control Activation|" + s_DisabledBecauseOfSelfControlTooltip);
            public static readonly GUIContent prefabContent = EditorGUIUtility.TrTextContent("Prefab", "A prefab to instantiate as a child object of the source game object");
            public static readonly GUIContent advancedContent = EditorGUIUtility.TrTextContent("Advanced");
            public static readonly GUIContent updateParticleSystemsContent = EditorGUIUtility.TrTextContent("Control Particle Systems", "Synchronize the time between the clip and any particle systems on the game object");
            public static readonly GUIContent updatePlayableDirectorContent = EditorGUIUtility.TrTextContent("Control Playable Directors", "Synchronize the time between the clip and any playable directors on the game object");
            public static readonly GUIContent updatePlayableDirectorDisabledContent = GUIUtility.TextContent("Control Playable Directors|" + s_DisabledBecauseOfSelfControlTooltip);
            public static readonly GUIContent updateITimeControlContent = EditorGUIUtility.TrTextContent("Control ITimeControl", "Synchronize the time between the clip and any Script that implements the ITimeControl interface on the game object");
            public static readonly GUIContent updateHierarchy = EditorGUIUtility.TrTextContent("Control Children", "Search child game objects for particle systems and playable directors");
            public static readonly GUIContent randomSeedContent = EditorGUIUtility.TrTextContent("Random Seed", "A random seed to provide the particle systems for consistent previews. This will only be used on particle systems where AutoRandomSeed is on.");
            public static readonly GUIContent postPlayableContent = EditorGUIUtility.TrTextContent("Post Playback", "The active state to the leave the game object when the timeline is finished. \n\nRevert will leave the game object in the state it was prior to the timeline being run");
            #region 扩展参数
            public static readonly GUIContent layerContent = EditorGUIUtility.TrTextContent("Layer");
            public static readonly GUIContent transSyncDataContent = EditorGUIUtility.TrTextContent("Transform Sync Data");
            #endregion
        }

        public static readonly string[] mEffectViewSpecialTypes = new string[] { "None", "DifferentEnemy", "OnlySelf", "OnlyFriendTeamWithoutSelf" };
        public static readonly string[] mEffectTypes = new string[] { "Self", "World" };

        SerializedProperty m_SourceObject;
        SerializedProperty m_PrefabObject;
        SerializedProperty m_SyncData_IsNeedMirror;
        SerializedProperty m_SyncData_IsFollowPosition;
        SerializedProperty m_SyncData_EffectViewSpecialType;
        SerializedProperty m_SyncData_EffectType;
        SerializedProperty m_SyncData_IsIgnoreOffsetY;
        SerializedProperty m_SyncData_IsPositionLocal;
        SerializedProperty m_SyncData_PositionOffset;
        SerializedProperty m_SyncData_IsFollowRotation;
        SerializedProperty m_SyncData_RotateOffset;
        SerializedProperty m_SyncData_IsFollowScale;
        SerializedProperty m_SyncData_ScaleOffset;
        SerializedProperty m_SyncData_IsFollowInUpdate;
        SerializedProperty m_SyncData_IsFollowRotationInit;
        SerializedProperty m_SyncData_IsFollowScaleInit;

        SerializedProperty m_UpdateParticle;
        SerializedProperty m_UpdateDirector;
        SerializedProperty m_UpdateITimeControl;
        SerializedProperty m_SearchHierarchy;
        SerializedProperty m_UseActivation;
        SerializedProperty m_PostPlayback;
        SerializedProperty m_RandomSeed;

        #region 扩展参数
        SerializedProperty m_Layer;
        SerializedProperty m_TransSyncData;
        #endregion

        bool m_CycleReference;

        GUIContent m_SourceObjectLabel = new GUIContent();

        // the director that the selection was made with. Normally this matches the active director in timeline,
        //  but persists if the active timeline changes (case 962516)
        private PlayableDirector contextDirector
        {
            get
            {
                if (serializedObject == null)
                    return null;
                return serializedObject.context as PlayableDirector;
            }
        }

        public virtual void OnEnable()
        {
            if (target == null) // case 946080
                return;

            m_SourceObject = serializedObject.FindProperty("sourceGameObject");
            m_PrefabObject = serializedObject.FindProperty("prefabGameObject");
            m_UpdateParticle = serializedObject.FindProperty("updateParticle");
            m_UpdateDirector = serializedObject.FindProperty("updateDirector");
            m_UpdateITimeControl = serializedObject.FindProperty("updateITimeControl");
            m_SearchHierarchy = serializedObject.FindProperty("searchHierarchy");
            m_UseActivation = serializedObject.FindProperty("active");
            m_PostPlayback = serializedObject.FindProperty("postPlayback");
            m_RandomSeed = serializedObject.FindProperty("particleRandomSeed");

            #region 扩展参数
            m_Layer = serializedObject.FindProperty("layer");
            m_TransSyncData = serializedObject.FindProperty("transSyncData");
            m_SyncData_IsNeedMirror = m_TransSyncData.FindPropertyRelative("IsNeedMirror");
            m_SyncData_IsFollowPosition = m_TransSyncData.FindPropertyRelative("IsFollowPosition");
            m_SyncData_IsIgnoreOffsetY = m_TransSyncData.FindPropertyRelative("IsIgnoreOffsetY");
            m_SyncData_IsPositionLocal = m_TransSyncData.FindPropertyRelative("IsPositionLocal");
            m_SyncData_EffectViewSpecialType = m_TransSyncData.FindPropertyRelative("EffectViewSpecialType");
            m_SyncData_EffectType = m_TransSyncData.FindPropertyRelative("EffectType");
            m_SyncData_PositionOffset = m_TransSyncData.FindPropertyRelative("PositionOffset");
            m_SyncData_IsFollowRotation = m_TransSyncData.FindPropertyRelative("IsFollowRotation");
            m_SyncData_RotateOffset = m_TransSyncData.FindPropertyRelative("RotateOffset");
            m_SyncData_IsFollowScale = m_TransSyncData.FindPropertyRelative("IsFollowScale");
            m_SyncData_ScaleOffset = m_TransSyncData.FindPropertyRelative("ScaleOffset");
            m_SyncData_IsFollowInUpdate = m_TransSyncData.FindPropertyRelative("IsFollowInUpdate");
            m_SyncData_IsFollowRotationInit = m_TransSyncData.FindPropertyRelative("IsFollowRotationInit");
            m_SyncData_IsFollowScaleInit = m_TransSyncData.FindPropertyRelative("IsFollowScaleInit");
            #endregion
            CheckForCyclicReference();
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            serializedObject.Update();

            m_SourceObjectLabel.text = m_SourceObject.displayName;

            if (m_PrefabObject.objectReferenceValue != null)
                m_SourceObjectLabel.text = "Parent Object";

            bool selfControlled = false;

            EditorGUI.BeginChangeCheck();

            using (new GUIMixedValueScope(m_SourceObject.hasMultipleDifferentValues))
                EditorGUILayout.PropertyField(m_SourceObject, m_SourceObjectLabel);

            var sourceGameObject = m_SourceObject.exposedReferenceValue as GameObject;
            //selfControlled = m_PrefabObject.objectReferenceValue == null && TimelineWindow.instance != null && TimelineWindow.instance.state != null &&
            //    contextDirector != null && sourceGameObject == contextDirector.gameObject;
            selfControlled = m_PrefabObject.objectReferenceValue == null && TimelineWindowUtility.CheckStateNotNull() &&
                contextDirector != null && sourceGameObject == contextDirector.gameObject;

            if (EditorGUI.EndChangeCheck())
            {
                CheckForCyclicReference();
                if (!selfControlled)
                    DisablePlayOnAwake(sourceGameObject);
            }

            if (selfControlled)
            {
                EditorGUILayout.HelpBox("The assigned GameObject references the same PlayableDirector component being controlled.", MessageType.Warning);
            }
            else if (m_CycleReference)
            {
                EditorGUILayout.HelpBox("The assigned GameObject contains a PlayableDirector component that results in a circular reference.", MessageType.Warning);
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_PrefabObject, Styles.prefabContent);
            EditorGUI.indentLevel--;

            using (new EditorGUI.DisabledScope(selfControlled))
            {
                EditorGUILayout.PropertyField(m_UseActivation, selfControlled ? Styles.activationDisabledContent : Styles.activationContent);
                if (m_UseActivation.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_PostPlayback, Styles.postPlayableContent);
                    EditorGUI.indentLevel--;
                }
            }

            m_SourceObject.isExpanded = EditorGUILayout.Foldout(m_SourceObject.isExpanded, Styles.advancedContent);

            if (m_SourceObject.isExpanded)
            {
                EditorGUI.indentLevel++;

                using (new EditorGUI.DisabledScope(selfControlled && !m_SearchHierarchy.boolValue))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_UpdateDirector, selfControlled ? Styles.updatePlayableDirectorDisabledContent : Styles.updatePlayableDirectorContent);
                    if (EditorGUI.EndChangeCheck())
                    {
                        CheckForCyclicReference();
                    }
                }

                EditorGUILayout.PropertyField(m_UpdateParticle, Styles.updateParticleSystemsContent);
                if (m_UpdateParticle.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_RandomSeed, Styles.randomSeedContent);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(m_UpdateITimeControl, Styles.updateITimeControlContent);

                EditorGUILayout.PropertyField(m_SearchHierarchy, Styles.updateHierarchy);

                EditorGUI.indentLevel--;
            }

            #region 扩展参数
            m_Layer.intValue = EditorGUILayout.LayerField(Styles.layerContent, m_Layer.intValue);
            m_TransSyncData.isExpanded = EditorGUILayout.Foldout(m_TransSyncData.isExpanded, Styles.transSyncDataContent);
            if (m_TransSyncData.isExpanded)
            {
                EditorGUILayout.PropertyField(m_SyncData_IsNeedMirror);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowPosition);

                m_SyncData_EffectViewSpecialType.intValue = EditorGUILayout.Popup("EffectViewSpecialType", m_SyncData_EffectViewSpecialType.intValue, mEffectViewSpecialTypes);
                m_SyncData_EffectType.intValue = EditorGUILayout.Popup("ActorEffectType", m_SyncData_EffectType.intValue, mEffectTypes);

                EditorGUILayout.PropertyField(m_SyncData_IsIgnoreOffsetY);
                EditorGUILayout.PropertyField(m_SyncData_IsPositionLocal);
                EditorGUILayout.PropertyField(m_SyncData_PositionOffset);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowRotation);
                EditorGUILayout.PropertyField(m_SyncData_RotateOffset);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowScale);
                EditorGUILayout.PropertyField(m_SyncData_ScaleOffset);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowInUpdate);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowRotationInit);
                EditorGUILayout.PropertyField(m_SyncData_IsFollowScaleInit);
            }
            #endregion
            serializedObject.ApplyModifiedProperties();
        }

        //
        // Fix for a workflow issue where scene objects with directors have play on awake by default enabled.
        //  This causes confusion when the director is played within another director, so we disable it on assignment
        //  to avoid the issue, but not force the issue on the user
        public void DisablePlayOnAwake(GameObject sourceObject)
        {
            if (sourceObject != null && m_UpdateDirector.boolValue)
            {
                if (m_SearchHierarchy.boolValue)
                {
                    var directors = sourceObject.GetComponentsInChildren<PlayableDirector>();
                    foreach (var d in directors)
                    {
                        DisablePlayOnAwake(d);
                    }
                }
                else
                {
                    DisablePlayOnAwake(sourceObject.GetComponent<PlayableDirector>());
                }
            }
        }

        public void DisablePlayOnAwake(PlayableDirector director)
        {
            if (director == null)
                return;
            var obj = new SerializedObject(director);
            var prop = obj.FindProperty("m_InitialState");
            prop.enumValueIndex = (int)PlayState.Paused;
            obj.ApplyModifiedProperties();
        }

        void CheckForCyclicReference()
        {
            serializedObject.ApplyModifiedProperties();
            m_CycleReference = false;

            PlayableDirector director = contextDirector;
            if (contextDirector == null)
                return;

            foreach (var asset in targets.OfType<ControlPlayableAsset>())
            {
                if (ControlPlayableUtility.DetectCycle(asset, director))
                {
                    m_CycleReference = true;
                    return;
                }
            }
        }
    }
}
