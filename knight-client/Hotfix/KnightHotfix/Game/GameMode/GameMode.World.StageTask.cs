using Knight.Framework;
using Knight.Hotfix.Core;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Core;
using UnityFx.Async;

namespace Game
{
    public partial class GameMode_World
    {
        /// <summary>
        /// 加载场景资源的StageTask
        /// </summary>
        public class StageTask_LoadAssets : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_LoadAssets(GameMode_World rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "LoadAssets";
                return true;
            }

            protected override async Task OnRun_Async()
            {
                await base.OnRun_Async();
            }
        }

        /// <summary>
        /// 初始化数据， 加载界面
        /// </summary>
        public class StageTask_InitData : StageTask
        {
            public GameMode_World GameMode;

            public StageTask_InitData(GameMode_World rGameMode)
            {
                this.GameMode = rGameMode;
            }

            protected override bool OnInit()
            {
                this.name = "InitData";
                return true;
            }

            protected override async Task OnRun_Async()
            {
                //打开Login界面
                await ViewManager.Instance.OpenAsync("KNFrame", View.State.Fixing);
                await WaitAsync.WaitForSeconds(1.0f);

                //隐藏进度条
                GameLoading.Instance.Hide();

                Debug.Log("GameStage -- Init data complete.");
            }
        }
    }
}
