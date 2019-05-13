using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Knight.Core;
using UnityEngine.UI;
using UnityEngine;
using Knight.Framework;

namespace Game
{
    public class FrameView : ViewController
    {
        private int                 mCurIndex;
        private int                 mPrevIndex;

        private View                mListTestView;

        protected override async Task OnInitialize()
        {
            await base.OnInitialize();

            this.mCurIndex  = 0;
            this.mPrevIndex = 0;

            var rMainFrame = ViewModelManager.Instance.ReceiveViewModel<FrameViewModel>();
            rMainFrame.PlayerName = Account.Instance.PlayerName;
            rMainFrame.CoinCount  = LogicUtilTool.ToCountString(Account.Instance.CoinCount);

            rMainFrame.MainFrameTab = new List<MainFrameTabItem>()
            {
                new MainFrameTabItem(){ Name = "战 斗"  },
                new MainFrameTabItem(){ Name = "基 地"  },
                new MainFrameTabItem(){ Name = "商 城"  },
            };
        }

        protected override async Task OnOpen()
        {
            await base.OnOpen();
            // 打开故事大厅
            this.mListTestView = await FrameManager.Instance.OpenPageUIAsync("KNListTest", View.State.PageSwitch);
        }

        [DataBinding]
        public void OnMainFrameTabChanged(EventArg rEventArg)
        {
            var nCurIndex = rEventArg.Get<int>(0);
            if (nCurIndex == this.mCurIndex) return;

            this.mPrevIndex = this.mCurIndex;
            this.mCurIndex = nCurIndex;

            if (this.mCurIndex == 0)
            {
                // 打开故事大厅
                this.mListTestView.Show();
            }
            else
            {
                this.mListTestView.Hide();
            }
        }
    }
}
