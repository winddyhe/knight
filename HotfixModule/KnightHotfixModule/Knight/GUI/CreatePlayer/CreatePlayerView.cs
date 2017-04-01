using System;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine.UI;
using Game.Knight;
using WindHotfix.Core;
using UnityEngine;

namespace KnightHotfixModule.Knight.GUI
{
    public class CreatePlayerView : ViewController
    {
        public ToggleGroup          ProfessionSelected;
        public InputField           PlayerName;
        public Text                 ProfessionalDesc;
        public CreatePlayerItem     CurrentSelectedItem;

        private HotfixEventHandler  mEventHandler;

        public override void Initialize(List<UnityEngine.Object> rObjs)
        {
            base.Initialize(rObjs);
            mEventHandler = new HotfixEventHandler(this.mObjects);

            // 转换变量
            this.ProfessionSelected  = this.mObjects[0] as ToggleGroup;
            this.PlayerName          = this.mObjects[1] as InputField;
            this.ProfessionalDesc    = this.mObjects[2] as Text;
            this.CurrentSelectedItem = this.mObjects[3] as CreatePlayerItem;

            // 注册事件
            mEventHandler.AddEventListener(this.mObjects[4], OnPlayerCreateBtn_Clicked);
            mEventHandler.AddEventListener(this.mObjects[5], OnBackBtn_Clicked);
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mEventHandler == null) return;
            mEventHandler.Handle(rTarget);
        }

        public override void OnOpening()
        {
            base.OnOpening();
            this.CurrentSelectedItem.StartLoad();
            this.mIsOpened = true;
        }

        public override void OnClosing()
        {
            base.OnClosing();

            mEventHandler.RemoveAll();
            mEventHandler = null;

            this.CurrentSelectedItem.StopLoad();
            this.mIsClosed = true;
        }

        private void OnPlayerCreateBtn_Clicked(UnityEngine.Object rTarget)
        {
            if (string.IsNullOrEmpty(this.PlayerName.text))
            {
                Toast.Instance.Show("角色名不能为空！");
                return;
            }
            KnightHotfixModule.Knight.GameFlow.CreatePlayer.Instance.Create(this.PlayerName.text, this.CurrentSelectedItem.ProfessionalID);
        }

        private void OnBackBtn_Clicked(UnityEngine.Object rTarget)
        {
            UIManager.Instance.Open("KNPlayerList", View.State.dispatch);
        }
    }
}
