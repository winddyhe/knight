using UnityEngine;
using UnityEngine.UI;
using WindHotfix.GUI;

namespace KnightHotfixModule.Test.UI
{
    public class UILoginTest : THotfixViewController<UILoginTest>
    {
        private InputField mAccountInput;
        private InputField mPasswordInput;

        public override void OnInitialize()
        {
            Debug.LogError("UILoginTest.Initialize..." + this.Objects.Count);

            mAccountInput = this.Objects[0].Object as InputField;
            mPasswordInput = this.Objects[1].Object as InputField;
            
            this.AddEventListener(this.Objects[2].Object, OnButton_Clicked);
        }

        public override void OnOpening()
        {
            Debug.LogError("OnOpening: " + this.mIsOpened);
        }

        public override void OnClosing()
        {
            Debug.LogError("OnClosing: " + this.mIsClosed);
        }
        
        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError("Button Clicked..." + this.mAccountInput.text + ", " + this.mPasswordInput.text);
        }
    }
}
