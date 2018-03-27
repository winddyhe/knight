//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using UnityEngine.UI;
using WindHotfix.GUI;
using WindHotfix.Core;
using UnityEngine.EventSystems;
using Framework;

namespace KnightHotfixModule.Test.UI
{
    public class UILoginTest : TViewController<UILoginTest>
    {
        [HotfixBinding("AccountInput")]
        public InputField       AccountInput;
        [HotfixBinding("PasswordInput")]
        public InputField       PasswordInput;

        public override void OnInitialize()
        {
            Debug.LogError("UILoginTest.Initialize..." + this.Objects.Count);
        }

        public override void OnOpening()
        {
            Debug.LogError("OnOpening: " + this.mIsOpened);
        }

        public override void OnClosing()
        {
            Debug.LogError("OnClosing: " + this.mIsClosed);
        }

        [HotfixBindingEvent("LoginBtn", HEventTriggerType.PointerClick)]
        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError("Button Clicked..." + this.AccountInput.text + ", " + this.PasswordInput.text);
        }
    }
}
