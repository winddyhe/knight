using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class World : THotfixSingleton<World>
    {
        private GameMode_World mGameMode;
        
        private World()
        {
        }

        public async Task Initialize()
        {
            mGameMode = new GameMode_World();
            GameMode.GetCurrentMode = (() =>
            {
                return mGameMode;
            });

            // 开始游戏
            GameStageManager.Instance.InitGame();
            GameStageManager.Instance.StageIntialize();
            await GameStageManager.Instance.StageRunning();
        }
    }
}
