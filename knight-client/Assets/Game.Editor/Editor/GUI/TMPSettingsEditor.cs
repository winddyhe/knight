using Knight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Game.Editor
{
    public class TMPSettingsEditor
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ScriptsReloaded()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            TMP_Settings.LoadSettingFunc = () =>
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/GameAssets/GUI/TMP/Fonts/TMPFont/TMPSettings.asset");
            };
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange rPlayModeStateChange)
        {
            if (rPlayModeStateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                TMP_Settings.LoadSettingFunc = () =>
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/GameAssets/GUI/TMP/Fonts/TMPFont/TMPSettings.asset");
                };
            }
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }
}
