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
        private string      mAccountName    = "Test111";
        private string      mPassword       = "xxxxxxx";

        [DataBinding]
        public  string      AccountName
        {
            get { return mAccountName;  }
            set
            {
                mAccountName = value;
                this.PropChanged("AccountName");
            }
        }

        [DataBinding]
        public  string      Password
        {
            get { return mPassword;     }
            set
            {
                mPassword = value;
                this.PropChanged("Password");
            }
        }
    }
}
