using UnityEditor;
using System.IO;

namespace Game.Editor
{
    public class KnightCustomAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessAsset()
        {
            if (!this.assetPath.Contains("Assets/GameAssets/Config/GameConfig/Json"))
            {
                return;
            }
            GameConfig.Instance.SaveToBinaryFileByJsonFile(
                "Assets/GameAssets/Config/GameConfig/Json", "Assets/GameAssets/Config/GameConfig/Binary", Path.GetFileNameWithoutExtension(this.assetPath));
        }
    }
}
