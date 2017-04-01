using System.Collections.Generic;
using Framework.WindUI;
using WindHotfix.Core;
using UnityEngine.UI;
using KnightHotfixModule.Knight.GameFlow;

namespace KnightHotfixModule.Knight.GUI
{
    public class LoginView : ViewController
    {
        private HotfixEventHandler  mEventHandler; 

        private InputField          mAccountInput;
        private InputField          mPasswordInput;

        public override void Initialize(List<UnityEngine.Object> rObjs)
        {
            base.Initialize(rObjs);

            mAccountInput = this.mObjects[0] as InputField;
            mPasswordInput = this.mObjects[1] as InputField;

            mEventHandler = new HotfixEventHandler(this.mObjects);
            mEventHandler.AddEventListener(this.mObjects[2], OnButton_Clicked);
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mEventHandler == null) return;
            mEventHandler.Handle(rTarget);
        }

        public override void OnOpening()
        {
            base.OnOpening();
        }

        public override void OnClosing()
        {
            base.OnClosing();

            mEventHandler.RemoveAll();
            mEventHandler = null;
        }

        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            if (string.IsNullOrEmpty(this.mAccountInput.text))
            {
                Toast.Instance.Show("用户名不能为空。");
                return;
            }
            if (string.IsNullOrEmpty(this.mPasswordInput.text))
            {
                Toast.Instance.Show("密码不能为空。");
                return;
            }
            Login.Instance.LoginGateServer(this.mAccountInput.text, this.mPasswordInput.text);
        }
    }
}
