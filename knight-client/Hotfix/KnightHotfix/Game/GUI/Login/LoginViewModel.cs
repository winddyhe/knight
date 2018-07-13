using Knight.Hotfix.DataBinding;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class LoginViewModel : ViewModel
    {
        private string      mAccountName;
        private string      mPassword;

        public  string      AccountName
        {
            get { return mAccountName;  }
            set { mAccountName = value; }
        }

        public  string      Password
        {
            get { return mPassword;     }
            set { mPassword = value;    }
        }
    }
}
