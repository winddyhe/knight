using WindHotfix.GameStage;

namespace Game.Knight
{
    public partial class GameMode_World : GameMode
    {
        public StageConfig  StageConfig;

        public EntityPlayer MainPlayer1;
        public EntityCamera MainCamera;
        
        protected override void OnBuildStages()
        {
            this.AddGameStage(0, new StageTask_LoadAssets(this));
            this.AddGameStage(1, new StageTask_CreateECS(this));
            this.AddGameStage(2, new StageTask_InitData(this));
        }
    }
}
