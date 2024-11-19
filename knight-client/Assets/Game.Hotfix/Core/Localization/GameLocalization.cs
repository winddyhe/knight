using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEngine;

namespace Game
{
    public class GameLocalization : ILocalization
    {
        private MultiLanguageType mCurLanguage;
        private MultiLanguageType mSystemLanguage;
        private LocalizationConfig mLanguageConfig;

        public GameLocalization() 
        {
            // 获取系统语言
            this.mSystemLanguage = LocalizationTool.Instance.SystemLanguageToMultiLanguageType(Application.systemLanguage);
        }

        public MultiLanguageType CurLanguageType
        {
            get => this.mCurLanguage;
            set => this.mCurLanguage = value;
        }

        public MultiLanguageType DefaultLanguageType
        {
            get => this.mLanguageConfig.DefaultLanguageType;
            set => this.mLanguageConfig.DefaultLanguageType = value;
        }

        public string DefaultFontName
        {
            get => this.mLanguageConfig.DefaultFontName;
            set => this.mLanguageConfig.DefaultFontName = value;
        }

        public string GetLanguage(string rLanuageID)
        {
            var rSystemLanguage = this.mCurLanguage;
            return GameConfig.Instance.GetLanguage(rSystemLanguage, rLanuageID);
        }

        public async UniTask LoadLanguageConfig(string rConfigABPath, string rConfigName)
        {
            var rLanguageConfigRequest = AssetLoader.Instance.LoadAssetAsync<LocalizationConfig>(rConfigABPath, rConfigName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            await rLanguageConfigRequest.Task();
            if (rLanguageConfigRequest.Asset == null)
            {
                Debug.LogErrorFormat("Load Language Config Failed, ABPath: {0}, ConfigName: {1}", rConfigABPath, rConfigName);
                return;
            }
            this.mLanguageConfig = rLanguageConfigRequest.Asset;
            // 初始化将默认语言设置为游戏语言
            this.mCurLanguage = this.mLanguageConfig.DefaultLanguageType;
        }
    }
}