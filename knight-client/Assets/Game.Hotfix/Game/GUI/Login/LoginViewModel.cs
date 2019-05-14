using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Knight.Core;

namespace Game
{
    [DataBinding]
    public class LoginViewModel : ViewModel
    {
        [DataBinding]
        public  string      AccountName { get; set; }
        [DataBinding]
        public  string      Password    { get; set; }
    }
}
