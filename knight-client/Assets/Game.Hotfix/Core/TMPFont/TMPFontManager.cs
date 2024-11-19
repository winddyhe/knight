using Cysharp.Threading.Tasks;
using Knight.Core;
using TMPro;

namespace Game
{
    public class TMPFontManager : TSingleton<TMPFontManager>
    {
        private string mTMPFontABPath = "game/gui/tmp/fonts/tmpfont.ab";
        private string mTMPLanguageFontABPath = "game/gui/tmp/fonts/{0}.ab";

        private TMP_Settings mTMPSettings;

        private AssetLoaderRequest<TMP_FontAsset> mCurrentTMPFontAssetRequest;
        private AssetLoaderRequest<TMP_FontAsset> mLanguageTMPFontAssetRequest;

        private TMP_FontAsset mCurrentTMPFontAsset;
        private TMP_FontAsset mLanguageTMPFontAsset;

        private TMPFontManager()
        {
        }

        public async UniTask Initialize()
        {
            // Load TMP Settings
            var rTMPSettingsLoadRequest = AssetLoader.Instance.LoadAssetAsync<TMP_Settings>(this.mTMPFontABPath, "TMPSettings", AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            await rTMPSettingsLoadRequest.Task();
            if (rTMPSettingsLoadRequest.Asset != null)
            {
                this.mTMPSettings = rTMPSettingsLoadRequest.Asset;
            }
            UnityEngine.Debug.Log("TMP Settings Load Success: " + (this.mTMPSettings != null));

            // 字体设置初始化
            TMP_Settings.LoadSettingFunc = this.LoadTMPSettings;

            // 加载字体资源
            await this.InitializeFontAsset();
        }

        private async UniTask InitializeFontAsset()
        {
            // 加载当前默认字体
            var rDefualtFontName = LocalizationTool.Instance.DefaultFontName;
            this.mCurrentTMPFontAssetRequest = AssetLoader.Instance.LoadAssetAsync<TMP_FontAsset>(this.mTMPFontABPath, rDefualtFontName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            await this.mCurrentTMPFontAssetRequest.Task();
            if (this.mCurrentTMPFontAssetRequest.Asset == null)
            {
                return;
            }
            this.mCurrentTMPFontAsset = this.mCurrentTMPFontAssetRequest.Asset;

            // 加载语言字体
            await this.SwitchLanguage();
        }

        public async UniTask SwitchLanguage()
        {
            // 卸载语言字体
            if (this.mLanguageTMPFontAssetRequest != null)
            {
                AssetLoader.Instance.Unload(this.mLanguageTMPFontAssetRequest);
                this.mLanguageTMPFontAsset = null;
            }

            // 加载语言字体
            var rLanguageFontABPath = string.Format(this.mTMPLanguageFontABPath, LocalizationTool.Instance.GetLocalizationSuffixLower("TMPFont"));
            var rLanguageFontAssetName = LocalizationTool.Instance.GetLocalizationSuffix(this.mCurrentTMPFontAsset.name);
            this.mLanguageTMPFontAssetRequest = AssetLoader.Instance.LoadAssetAsync<TMP_FontAsset>(rLanguageFontABPath, rLanguageFontAssetName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            await this.mLanguageTMPFontAssetRequest.Task();
            if (this.mLanguageTMPFontAssetRequest.Asset == null)
            {
                return;
            }

            // 设置语言字体
            this.mCurrentTMPFontAsset.fallbackFontAssetTable.Clear();
            this.mCurrentTMPFontAsset.fallbackFontAssetTable.Add(this.mLanguageTMPFontAsset);
            this.mCurrentTMPFontAsset.ReadFontAssetDefinition();
        }

        private TMP_Settings LoadTMPSettings()
        {
            return this.mTMPSettings;
        }
    }
}
