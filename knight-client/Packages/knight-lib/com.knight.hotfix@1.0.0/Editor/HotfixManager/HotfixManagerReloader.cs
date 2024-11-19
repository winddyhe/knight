using Knight.Core;
using UnityEngine;

namespace Knight.Framework.Hotfix
{
    public class HotfixManagerReloader
    {
        [UnityEditor.Callbacks.DidReloadScripts(1)]
        public static void ScriptsReloaded()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (!Application.isPlaying)
            {
                ViewModelInjectInHotfixDLL();
                GameSaveInjectInHotfixDLL();
                var rDLLNames = new string[] { "Game.Config", "Game.Hotfix" };
                HotfixManager.Instance.Initialize(rDLLNames);
                HotfixManager.Instance.LoadDLL();
            }
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange rPlayModeStateChange)
        {
            if (rPlayModeStateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                ViewModelInjectInHotfixDLL();
                GameSaveInjectInHotfixDLL();
                var rDLLNames = new string[] { "Game.Config", "Game.Hotfix" };
                HotfixManager.Instance.Initialize(rDLLNames);
                HotfixManager.Instance.LoadDLL();
            }
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private static void ViewModelInjectInHotfixDLL()
        {
            var rUIFrameworkAssembly = System.Reflection.Assembly.Load("Knight.UI.Editor");
            if (rUIFrameworkAssembly == null)
            {
                Debug.LogError("Cannot find dll: Knight.UI.Editor.");
                return;
            }
            var rViewModelInjectEditorType = rUIFrameworkAssembly.GetType("Knight.Framework.UI.Editor.ViewModelInjectEditor");
            if (rViewModelInjectEditorType == null)
            {
                Debug.LogError("Cannot find class: Knight.Framework.UI.Editor.ViewModelInjectEditor.");
                return;
            }
            rViewModelInjectEditorType.InvokeMember("Inject", ReflectTool.flags_method_static, null, null, new object[0]);
        }
        
        private static void GameSaveInjectInHotfixDLL()
        {
            var rGameSaveAssembly = System.Reflection.Assembly.Load("Knight.GameSave.Editor");
            if (rGameSaveAssembly == null)
            {
                Debug.LogError("Cannot find dll: Knight.GameSave.Editor.");
                return;
            }
            var rGameSaveInjectEditorType = rGameSaveAssembly.GetType("Knight.Framework.GameSave.Editor.GameSaveDataInjectEditor");
            if (rGameSaveInjectEditorType == null)
            {
                Debug.LogError("Cannot find class: Knight.Framework.GameSave.Editor.GameSaveDataInjectEditor.");
                return;
            }
            rGameSaveInjectEditorType.InvokeMember("Inject", ReflectTool.flags_method_static, null, null, new object[0]);
        }
    }
}
