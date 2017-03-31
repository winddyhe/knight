using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.WindUI;
using UnityEngine;
using WindHotfix.Core;
using UnityEngine.UI;

namespace KnightHotfixModule.Test.UI
{
    public class UILoginTest : ViewController
    {
        private HotfixEventHandler  mEventHandler;

        private InputField mAccountInput;
        private InputField mPasswordInput;

        public override void Initialize(List<UnityEngine.Object> rObjs)
        {
            base.Initialize(rObjs);
            Debug.LogError("UILoginTest.Initialize..." + this.mObjects.Count);

            mAccountInput = this.mObjects[0] as InputField;
            mPasswordInput = this.mObjects[1] as InputField;

            mEventHandler = new HotfixEventHandler(this.mObjects);
            mEventHandler.AddEventListener(this.mObjects[2], OnButton_Clicked);
        }

        public override void OnOpening()
        {
            base.OnOpening();
            Debug.LogError("OnOpening: " + this.mIsOpened);
        }

        public override void OnClosing()
        {
            base.OnClosing();
            Debug.LogError("OnClosing: " + this.mIsClosed);

            mEventHandler.RemoveAll();
            mEventHandler = null;
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mEventHandler == null) return;
            mEventHandler.Handle(rTarget);
        }

        private void OnButton_Clicked(UnityEngine.Object rObj)
        {
            Debug.LogError("Button Clicked..." + this.mAccountInput.text + ", " + this.mPasswordInput.text);
        }
    }
}
