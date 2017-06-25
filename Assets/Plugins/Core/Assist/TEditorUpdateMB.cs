using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    [ExecuteInEditMode]
    public class TEditorUpdateMB<T> : MonoBehaviour where T : MonoBehaviour
    {
        void Awake()
        {
            this.AwakeCustom();
#if UNITY_EDITOR
            EditorApplication.update += OnEditorUpdate;
#endif
        }

        void OnDestroy()
        {
            this.DestroyCustom();
#if UNITY_EDITOR
            EditorApplication.update -= OnEditorUpdate;
#endif
        }
        
        void Update()
        {
            if (!Application.isPlaying) return;
            UpdateCustom();
        }

        void OnEditorUpdate()
        {
            if (Application.isPlaying) return;
            UpdateCustom();
        }

        protected virtual void UpdateCustom()
        {
        }

        protected virtual void AwakeCustom()
        {
        }

        protected virtual void DestroyCustom()
        {
        }
    }
}
