using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEditor;

namespace Game.Editor
{
    public class GameEditorLocalization : ILocalization
    {
        private MultiLanguageType mCurLanguageType;
        private LocalizationConfig mLanguageConfig;

        public GameEditorLocalization()
        {
            // 获取默认值
            this.mCurLanguageType = LocalizationManagerEditor.LocalizationConfig.DefaultLanguageType;
        }

        public MultiLanguageType CurLanguageType
        {
            get => this.mCurLanguageType;
            set => this.mCurLanguageType = value;
        }

        public MultiLanguageType DefaultLanguageType
        {
            get => this.mLanguageConfig.DefaultLanguageType;
        }

        public string DefaultFontName
        {
            get => this.mLanguageConfig.DefaultFontName;
        }

        public string GetLanguage(string rLanuageID)
        {
            if (!LocalizationManagerEditor.LanguageConfigs.TryGetValue(rLanuageID, out var rLanguageConfig))
            {
                return rLanuageID;
            }
            var rLanguageContent = rLanuageID;
            switch (this.mCurLanguageType)
            {
                case MultiLanguageType.ChineseSimplified:
                    rLanguageContent = rLanguageConfig.ChineseSimplified;
                    break;
                case MultiLanguageType.English:
                    rLanguageContent = rLanguageConfig.English;
                    break;
                case MultiLanguageType.Japanese:
                    rLanguageContent = rLanguageConfig.Japanese;
                    break;
                case MultiLanguageType.Indonesian:
                    rLanguageContent = rLanguageConfig.Indonesian;
                    break;
                case MultiLanguageType.Korean:
                    rLanguageContent = rLanguageConfig.Korean;
                    break;
                case MultiLanguageType.Malay:
                    rLanguageContent = rLanguageConfig.Malay;
                    break;
                case MultiLanguageType.Thai:
                    rLanguageContent = rLanguageConfig.Thai;
                    break;
                case MultiLanguageType.Arabic:
                    rLanguageContent = rLanguageConfig.Arabic;
                    break;
                case MultiLanguageType.ChineseTraditional:
                    rLanguageContent = rLanguageConfig.ChineseTraditional;
                    break;
                default:
                    rLanguageContent = rLanguageConfig.ChineseSimplified;
                    break;
            }
            return rLanguageContent;
        }

#pragma warning disable 1998
        public async UniTask LoadLanguageConfig(string rConfigABPath, string rConfigName)
        {
             this.mLanguageConfig = AssetDatabase.LoadAssetAtPath<LocalizationConfig>(rConfigABPath);
        }
#pragma warning restore 1998
    }
}
