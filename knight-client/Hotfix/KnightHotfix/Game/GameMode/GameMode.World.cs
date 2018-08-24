using Knight.Hotfix.Core;
using System;

namespace Game
{
    public partial class GameMode_World : GameMode
    {
        protected override void OnBuildStages()
        {
            this.AddGameStage(0, new StageTask_LoadAssets(this));
            this.AddGameStage(2, new StageTask_InitData(this));
        }
    }
}
