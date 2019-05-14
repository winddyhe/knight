using Knight.Core;
using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class ListDataItem : ViewModel
    {
        [DataBinding]
        public string Value1 { get; set; }
        [DataBinding]
        public string Value2 { get; set; }
    }

    [DataBinding]
    public class ListTestViewModel : ViewModel
    {
        [DataBinding]
        public  ObservableList<ListDataItem>    ItemDatas { get; set; }
    }
}
