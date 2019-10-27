using NaughtyAttributes.Editor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ButtonAssist), true)]
    public class ButtonAssistInspector : InspectorEditor
    {
        private string          mUISoundPath = "Assets/Game/GameAsset/Sound/UI";
        private ButtonAssist    mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            mTarget = this.target as ButtonAssist;
        }

        public override void OnInspectorGUI()
        {
            var rSoundAssets = new List<string>();
            var rGUIDS = AssetDatabase.FindAssets("t:AudioClip", new string[] { this.mUISoundPath });
            for (int i = 0; i < rGUIDS.Length; i++)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDS[i]);
                rSoundAssets.Add(Path.GetFileNameWithoutExtension(rAssetPath));
            }
            this.mTarget.UIAudioClips = rSoundAssets.ToArray();

            base.OnInspectorGUI();
        }
    }
}
