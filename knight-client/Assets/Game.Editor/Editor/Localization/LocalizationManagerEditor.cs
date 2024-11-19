using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Knight.Core;
using Newtonsoft.Json;
using System.IO;

namespace Game.Editor
{
    public class LocalizationManagerEditor
    {
        public static Dictionary<string, LanguageConfig> LanguageConfigs = new Dictionary<string, LanguageConfig>();
        public static LocalizationConfig LocalizationConfig;

        [UnityEditor.Callbacks.DidReloadScripts(3)]
        public static void ScriptsReloaded()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (!Application.isPlaying)
            {
                LoadLanguageConfigs();
            }
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange rPlayModeStateChange)
        {
            if (rPlayModeStateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                LoadLanguageConfigs();
            }
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        public static void LoadLanguageConfigs()
        {
            var rConfigPath = "Assets/GameAssets/GUI/Config/LocalizationConfig.asset";
            LocalizationConfig = AssetDatabase.LoadAssetAtPath<LocalizationConfig>(rConfigPath);

            var rJsonPath = "Assets/GameAssets/Config/GameConfig/Json/Language.json";
            var rJsonText = File.ReadAllText(rJsonPath);
            LanguageConfigs = JsonConvert.DeserializeObject<Dictionary<string, LanguageConfig>>(rJsonText);
            LocalizationTool.Instance.Initialize(new GameEditorLocalization());
        }
    }
}
