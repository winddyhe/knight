using System;
using System.Collections.Generic;
using NaughtyAttributes;
using static UnityEngine.UI.Button;
using Knight.Core;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonAssist : MonoBehaviour
    {
        public  bool                IsInValid;
        [Dropdown("UIAudioClips")]
        public  string              AudioClipName = "click";
        [Dropdown("UIAudioClips")]
        public  string              AudioDisableClipName = "click_invalid";

        public  Button              Button;
        public  ButtonClickedEvent  onClick = new ButtonClickedEvent();

        [HideInInspector]
        public  string[]            UIAudioClips = new string[] { };

        private void Awake()
        {
            if (this.Button == null)
                this.Button = this.GetComponent<Button>();
            this.Button.onClick.AddListener(this.OnClickedHandler);
        }

        private void OnDestroy()
        {
            this.Button?.onClick.RemoveAllListeners();
        }

        private void OnClickedHandler()
        {
            if (this.IsInValid)
            {
                SoundPlayer.Instance?.PlayMulti(this.AudioDisableClipName);
                return;
            }
            else
            {
                SoundPlayer.Instance?.PlayMulti(this.AudioClipName);
            }
            this.onClick?.Invoke();
        }
    }
}
