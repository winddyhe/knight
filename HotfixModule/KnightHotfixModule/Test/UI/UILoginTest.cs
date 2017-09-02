//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using UnityEngine.UI;
using WindHotfix.GUI;

namespace KnightHotfixModule.Test.UI
{
    public class UILoginTest : TViewController<UILoginTest>
    {
        private InputField mAccountInput;
        private InputField mPasswordInput;

        public override void OnInitialize()
        {
            Debug.LogError("UILoginTest.Initialize..." + this.Objects.Count);

            mAccountInput = this.Objects[0].Object as InputField;
            mPasswordInput = this.Objects[1].Object as InputField;
            
            this.EventBinding(this.Objects[2].Object, UnityEngine.EventSystems.EventTriggerType.PointerClick, OnButton_Clicked);
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
