using UnityEngine;

namespace Knight.Core.Editor
{
    public class TypeResolveManagerEditor
    {
        [UnityEditor.Callbacks.DidReloadScripts(2)]
        public static void ScriptsReloaded()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (!Application.isPlaying)
            {
                TypeResolveManager.Instance.Initialize();
                TypeResolveManager.Instance.AddAssembly("Game");
                TypeResolveManager.Instance.AddAssembly("Game.Hotfix", true);
                TypeResolveManager.Instance.AddAssembly("Game.Config", true);
                TypeResolveManager.Instance.AddAssembly("Game.Editor");
                TypeResolveManager.Instance.AddAssembly("Knight.Assetbundle.Editor");
                TypeResolveManager.Instance.AddAssembly("Test");
            }
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange rPlayModeStateChange)
        {
            if (rPlayModeStateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                TypeResolveManager.Instance.Initialize();
                TypeResolveManager.Instance.AddAssembly("Game");
                TypeResolveManager.Instance.AddAssembly("Game.Hotfix", true);
                TypeResolveManager.Instance.AddAssembly("Game.Config", true);
                TypeResolveManager.Instance.AddAssembly("Game.Editor");
                TypeResolveManager.Instance.AddAssembly("Knight.Assetbundle.Editor");
                TypeResolveManager.Instance.AddAssembly("Test");
            }
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }
}
