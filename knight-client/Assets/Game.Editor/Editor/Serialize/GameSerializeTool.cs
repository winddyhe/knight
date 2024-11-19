using UnityEditor;

namespace Game.Editor
{
    public class GameSerializeTool
    {
        [MenuItem("Tools/Serializer/GameConfig Json To Binary")]
        public static void SerializeGameConfigJsonToBinary()
        {
            GameConfig.Instance.LoadFromJsonInEditor("Assets/GameAssets/Config/GameConfig/Json");
            GameConfig.Instance.SaveToBinaryFiles("Assets/GameAssets/Config/GameConfig/Binary");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}