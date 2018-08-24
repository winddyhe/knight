using Knight.Core;
using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class MainFrameTabItem : ViewModel
    {
        private string                  mName;

        [DataBinding]
        public  string  Name
        {
            get { return mName;     }
            set { mName = value;    }
        }
    }

    [DataBinding]
    public class FrameViewModel : ViewModel
    {
        private string                  mPlayerName;
        private string                  mCoinCount;

        private List<MainFrameTabItem>  mMainFrameTab;

        [DataBinding]
        public  string  PlayerName
        {
            get { return mPlayerName;               }
            set
            {
                mPlayerName = value;
                this.PropChanged("PlayerName");
            }
        }
        [DataBinding]
        public string   CoinCount
        {
            get { return mCoinCount;                }
            set
            {
                mCoinCount = value;
                this.PropChanged("CoinCount");
            }
        }
        [DataBinding]
        public List<MainFrameTabItem>   MainFrameTab
        {
            get { return mMainFrameTab;             }
            set
            {
                mMainFrameTab = value;
                this.PropChanged("MainFrameTab");
            }
        }
    }
}
