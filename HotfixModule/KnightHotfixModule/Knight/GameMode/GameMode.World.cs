using System;
using WindHotfix.GameStage;
using WindHotfix.Game;

namespace Game.Knight
{
    public partial class GameMode_World : GameMode
    {
        public StageConfig  StageConfig;

        public EntityPlayer MainPlayer;
        public EntityCamera MainCamera;
        
        protected override void OnBuildStages()
        {
            this.AddGameStage(0, new StageTask_LoadAssets(this));
            this.AddGameStage(1, new StageTask_CreateECS(this));
            this.AddGameStage(2, new StageTask_InitData(this));
        }

        protected override void OnUpdate()
        {
            // ECS模块的更新
            ECSManager.Instance.Update();
        }

        protected override void OnDestroy()
        {
            // ECS模块的销毁
            ECSManager.Instance.Destroy();
        }
    }
}
