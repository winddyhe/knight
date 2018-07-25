using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game
{
    public class LoginView : ViewController
    {
        [HotfixBinding("Login")]
        public LoginViewModel   ViewModel;
        [HotfixBinding("Login1")]
        public LoginViewModel   ViewModel1;

        protected override void OnOpening()
        {
            //this.ViewModel1.Password = "xxxxxxxxxxxxxxxxxxxxxxxxxxx";
        }

        //int i = 0;
        protected override void OnUpdate()
        {
            //i++;
            //this.ViewModel.AccountName = i.ToString();
        }
    }
}
