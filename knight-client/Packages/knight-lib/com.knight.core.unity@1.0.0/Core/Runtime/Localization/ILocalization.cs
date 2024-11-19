
using Cysharp.Threading.Tasks;

namespace Knight.Core
{
    public interface ILocalization
    {
        MultiLanguageType CurLanguageType { get; set; }
        MultiLanguageType DefaultLanguageType { get; }
        string DefaultFontName { get; }
        string GetLanguage(string rLanuageID);
        UniTask LoadLanguageConfig(string rConfigABPath, string rConfigName);
    }
}