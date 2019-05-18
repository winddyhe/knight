using System;
using System.Collections.Generic;
using Knight.Hotfix.Core;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    [ViewModelInitialize]
    public class Account : ViewModel
    {
        public static Account Instance
        {
            get { return ViewModelManager.Instance.ReceiveViewModel<Account>(); }
        }

        [DataBinding]
        public string           PlayerName      { get; set; }
        [DataBinding]
        public int              CoinCount       { get; set; }
    }
}
