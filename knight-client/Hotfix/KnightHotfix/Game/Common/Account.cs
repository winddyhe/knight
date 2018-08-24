using System;
using System.Collections.Generic;
using Knight.Hotfix.Core;

namespace Game
{
    public class Account : THotfixSingleton<Account>
    {
        public string           PlayerName      { get; set; }
        public int              CoinCount       { get; set; }
        
        private Account()
        {
        }

        public void Initialize()
        {
            this.PlayerName = "Winddy";
            this.CoinCount = 399009;
        }
    }
}
