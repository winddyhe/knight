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
        [HotfixBinding("MainFrame")]
        public  FrameViewModel      MainFrame;

        private int                 mCurIndex;
        private int                 mPrevIndex;

        protected override async Task OnInitialize()
        {
            await base.OnInitialize();

            this.mCurIndex  = 0;
            this.mPrevIndex = 0;

            this.MainFrame.PlayerName = Account.Instance.PlayerName;
            this.MainFrame.CoinCount  = LogicUtilTool.ToCountString(Account.Instance.CoinCount);

            this.MainFrame.MainFrameTab = new List<MainFrameTabItem>()
            {
                new MainFrameTabItem(){ Name = "战 斗"  },
                new MainFrameTabItem(){ Name = "基 地"  },
                new MainFrameTabItem(){ Name = "商 城"  },
            };
        }

        protected override void OnOpened()
        {
            // 打开故事大厅
            FrameManager.Instance.OpenPageUI("KNListTest");
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
                FrameManager.Instance.OpenPageUI("KNListTest");
            }
        }
    }
}
