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
        [DataBinding]
        public  string                  Name            { get; set; }
    }

    [DataBinding]
    public class FrameViewModel : ViewModel
    {
        [DataBinding]
        public string                   PlayerName      { get; set; }
        [DataBinding]
        public string                   CoinCount       { get; set; }
        [DataBinding]
        public List<MainFrameTabItem>   MainFrameTab    { get; set; }
    }
}
