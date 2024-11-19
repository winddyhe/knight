using Knight.Core;
using UnityEditor;

namespace Knight.Framework.UI.Editor
{
    [CustomEditor(typeof(MultiTextMeshProConfig))]
    public class MultiTextMeshProConfigInspector : UnityEditor.Editor
    {
        private MultiTextMeshProConfig mConfig;

        private void OnEnable()
        {
            this.mConfig = this.target as MultiTextMeshProConfig;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var rLanguageContent = LocalizationTool.Instance.GetLanguage(this.mConfig.MultiLanguageID);
            if (!string.IsNullOrEmpty(rLanguageContent))
            {
                EditorGUILayout.LabelField("LanguageContent: ", rLanguageContent);
                this.mConfig.Text = rLanguageContent;
            }
        }
    }
}
