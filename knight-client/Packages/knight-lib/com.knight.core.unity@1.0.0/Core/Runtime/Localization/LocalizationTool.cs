using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core
{
    public enum MultiLanguageType
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [InspectorName("简体中文")]
        ChineseSimplified = 1,
        /// <summary>
        /// 英文
        /// </summary>
        [InspectorName("英文")]
        English = 2,
        /// <summary>
        /// 日文
        /// </summary>
        [InspectorName("日文")]
        Japanese = 3,
        /// <summary>
        /// 印尼语
        /// </summary>
        [InspectorName("印尼语")]
        Indonesian = 4,
        /// <summary>
        /// 韩语
        /// </summary>
        [InspectorName("韩语")]
        Korean = 5,
        /// <summary>
        /// 马来语
        /// </summary>
        [InspectorName("马来语")]
        Malay = 6,
        /// <summary>
        /// 泰语
        /// </summary>
        [InspectorName("泰语")]
        Thai = 7,
        /// <summary>
        /// 阿拉伯语
        /// </summary>
        [InspectorName("阿拉伯语")]
        Arabic = 8,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [InspectorName("繁体中文")]
        ChineseTraditional = 9,
    }

    public class LocalizationTool : TSingleton<LocalizationTool>
    {
        public Dictionary<MultiLanguageType, string> SuffixDict = new Dictionary<MultiLanguageType, string>()
        {
            { MultiLanguageType.ChineseSimplified, "#lan_zh-CN" },
            { MultiLanguageType.English, "#lan_en" },
            { MultiLanguageType.Japanese, "#lan_ja" },
            { MultiLanguageType.Indonesian, "#lan_id" },
            { MultiLanguageType.Korean, "#lan_ko" },
            { MultiLanguageType.Malay, "#lan_ms" },
            { MultiLanguageType.Thai, "#lan_th" },
            { MultiLanguageType.Arabic, "#lan_ar" },
            { MultiLanguageType.ChineseTraditional, "#lan_zh-TW" },
        };
        public Dictionary<MultiLanguageType, string> SuffixLowerDict = new Dictionary<MultiLanguageType, string>()
        {
            { MultiLanguageType.ChineseSimplified, "#lan_zh-cn" },
            { MultiLanguageType.English, "#lan_en" },
            { MultiLanguageType.Japanese, "#lan_ja" },
            { MultiLanguageType.Indonesian, "#lan_id" },
            { MultiLanguageType.Korean, "#lan_ko" },
            { MultiLanguageType.Malay, "#lan_ms" },
            { MultiLanguageType.Thai, "#lan_th" },
            { MultiLanguageType.Arabic, "#lan_ar" },
            { MultiLanguageType.ChineseTraditional, "#lan_zh-tw" },
        };

        private ILocalization mLocalization;

        private LocalizationTool()
        {
        }

        public void Initialize(ILocalization rLocalization)
        {
            this.mLocalization = rLocalization;
        }

        public MultiLanguageType CurrentLanguage
        {
            get
            {
                return this.mLocalization != null ? this.mLocalization.CurLanguageType : MultiLanguageType.ChineseSimplified;
            }
            set
            {
                if (this.mLocalization != null)
                {
                    this.mLocalization.CurLanguageType = value;
                }
            }
        }
        public MultiLanguageType DefaultLanguage => this.mLocalization != null ? this.mLocalization.DefaultLanguageType : MultiLanguageType.ChineseSimplified;
        public string DefaultFontName => this.mLocalization != null ? this.mLocalization.DefaultFontName : "Arial";

        public string GetLanguage(string rLanguageID)
        {
            return this.mLocalization?.GetLanguage(rLanguageID);
        }

        public async UniTask LoadLanguageConfig(string rConfigABPath, string rConfigName)
        {
            if (this.mLocalization != null)
            {
                await this.mLocalization.LoadLanguageConfig(rConfigABPath, rConfigName);
            }
        }

        public MultiLanguageType SystemLanguageToMultiLanguageType(SystemLanguage rSystemLanguage)
        {
            switch (rSystemLanguage)
            {
                case SystemLanguage.Chinese:
                    return MultiLanguageType.ChineseSimplified;
                case SystemLanguage.English:
                    return MultiLanguageType.English;
                case SystemLanguage.Japanese:
                    return MultiLanguageType.Japanese;
                case SystemLanguage.Indonesian:
                    return MultiLanguageType.Indonesian;
                case SystemLanguage.Korean:
                    return MultiLanguageType.Korean;
                case SystemLanguage.Thai:
                    return MultiLanguageType.Thai;
                case SystemLanguage.Arabic:
                    return MultiLanguageType.Arabic;
                case SystemLanguage.ChineseSimplified:
                    return MultiLanguageType.ChineseSimplified;
                case SystemLanguage.ChineseTraditional:
                    return MultiLanguageType.ChineseTraditional;
                default:
                    return MultiLanguageType.ChineseSimplified;
            }
        }

        public string GetLocalizationSuffix(string rLocalizationName)
        {
            return rLocalizationName + this.SuffixDict[LocalizationTool.Instance.CurrentLanguage];
        }

        public string GetLocalizationSuffixLower(string rLocalizationName)
        {
            return rLocalizationName + this.SuffixLowerDict[LocalizationTool.Instance.CurrentLanguage];
        }
    }
}

