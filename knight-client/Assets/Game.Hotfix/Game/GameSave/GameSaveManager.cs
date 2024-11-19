using Knight.Core;
using Knight.Framework.GameSave;

namespace Game
{
    public class GameSaveManager : GameSaveManagerBase
    {
        public static GameSaveManager Instance => Singleton<GameSaveManager>.Instance;

        [GameSaveFile("__GameSave__Account__")]
        public GameSaveAccount Account;
    }
}
