using System;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine.UI;
using Game.Knight;
using WindHotfix.Core;
using UnityEngine;
using Framework.Hotfix;

namespace KnightHotfixModule.Knight.GUI
{
    public class CreatePlayerView : ViewController
    {
        public ToggleGroup              ProfessionSelected;
        public InputField               PlayerName;
        public Text                     ProfessionalDesc;
        public CreatePlayerItem         CurrentSelectedItem;

        private HotfixEventHandler  mEventHandler;
        
        public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            base.Initialize(rObjs, rBaseDatas);
            mEventHandler = new HotfixEventHandler();

            // 转换变量
            this.ProfessionSelected  = this.mObjects[0].Object as ToggleGroup;
            this.PlayerName          = this.mObjects[1].Object as InputField;
            this.ProfessionalDesc    = this.mObjects[2].Object as Text;
            this.CurrentSelectedItem = (this.mObjects[3].Object as MonoBehaviourContainer).ProxyHotfixObject as CreatePlayerItem;

            Debug.LogError(this.mObjects[3].Object);

            // 注册事件
            mEventHandler.AddEventListener(this.mObjects[4].Object, OnPlayerCreateBtn_Clicked);
            mEventHandler.AddEventListener(this.mObjects[5].Object, OnBackBtn_Clicked);
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
