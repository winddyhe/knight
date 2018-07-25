using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

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
        }
        
        protected override void OnUpdate()
        {
        }

        [DataBinding]
        private void OnBtnButton_Clicked()
        {
            Debug.LogError("OnBtnButton_Clicked...");
        }
    }
}
