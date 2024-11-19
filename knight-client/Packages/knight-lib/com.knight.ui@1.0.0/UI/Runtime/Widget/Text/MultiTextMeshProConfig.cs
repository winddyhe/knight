using Knight.Core;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Knight.Framework.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MultiTextMeshProConfig : MonoBehaviour
    {
        public bool IsUseMultiLanguage;
        [SerializeField]
        [ShowIf("IsUseMultiLanguage")]
        private string mMultiLanguageID;

        private TextMeshProUGUI mTextMeshPro;

        public string MultiLanguageID
        {
            get { return this.mMultiLanguageID; }
            set { this.mMultiLanguageID = value; }
        }

        public string Text
        {
            get { return this.mTextMeshPro.text; }
            set { this.mTextMeshPro.text = value; }
        }

        private void Awake()
        {
            this.mTextMeshPro = this.gameObject.ReceiveComponent<TextMeshProUGUI>();
        }
    }
}
