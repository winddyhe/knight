using Cysharp.Threading.Tasks;
using Knight.Core;
using Knight.Framework.Hotfix;
using Knight.Framework.UI;

namespace Game
{
    public class HotfixMainLogic : IHotfixMainLogic
    {
        public async UniTask Initialize()
        {
            // 初始化本地化工具
            LocalizationTool.Instance.Initialize(new GameLocalization());
            await LocalizationTool.Instance.LoadLanguageConfig("game/gui/config.ab", "LocalizationConfig");

            // 预加载Shader
            await ShaderManager.Instance.Initialize("game/urp/shaders.ab");

            // 加载TextMeshPro的字体设置
            await TMPFontManager.Instance.Initialize();

            // 加载配置
            await GameConfig.Instance.Initialize();

            // 初始化UI模块
            var rViewConfig = new ViewConfig();
            rViewConfig.Initialize();
            ViewManager.Instance.Initialize(rViewConfig);

            // 加载登录模块的UI资源
            await ViewManager.Instance.Open("Login", "UILogin", ViewLayer.Fixed, 0);
        }

        public void Destroy()
        {
        }

        public void LateUpdate(float fDeltaTime)
        {
        }

        public void Update(float fDeltaTime)
        {
        }
    }
}
