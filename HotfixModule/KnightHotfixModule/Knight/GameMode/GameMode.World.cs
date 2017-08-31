using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindHotfix.GameStage;

namespace Game.Knight
{
    public partial class GameMode_World : GameMode
    {
        public StageConfig  StageConfig;
        public Actor        MainPlayer;
        public List<Actor>  PlayerList;

        protected void Initialize()
        {
            this.PlayerList = new List<Actor>();
        }

        protected override void OnBuildStages()
        {
            // 构建GameStages
            this.gsm.gameStages = new Dict<int, GameStage>();

            this.Initialize();

            // 加载资源的GameStage
            GameStage rStageLoadAssets = new GameStage();
            rStageLoadAssets.index = 0;
            rStageLoadAssets.taskList = new List<StageTask>();
            rStageLoadAssets.taskList.Add(new StageTask_LoadAssets(this));
            rStageLoadAssets.taskList.Add(new StageTask_CreatePlayer(this));
            this.gsm.gameStages.Add(rStageLoadAssets.index, rStageLoadAssets);

            // 初始化游戏数据
            GameStage rStageInitData = new GameStage();
            rStageInitData.index = 1;
            rStageInitData.taskList = new List<StageTask>();
            rStageInitData.taskList.Add(new StageTask_InitData(this));
            this.gsm.gameStages.Add(rStageInitData.index, rStageInitData);
        }
    }
}
