//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.WindUI;
using UnityEngine.UI;
using Framework.Hotfix;
using WindHotfix.GUI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using WindHotfix.Core;
using Framework;

namespace Game.Knight
{
    public class CreatePlayerView : TViewController<CreatePlayerView>
    {
        [HotfixBinding("ProfessionSelected")]
        public ToggleGroup              ProfessionSelected;
        [HotfixBinding("PlayerNameInputField")]
        public InputField               PlayerName;
        [HotfixBinding("ProfessionText")]
        public Text                     ProfessionalDesc;
        [HotfixBinding("Knight1")]
        public HotfixMBContainer        KnightItem1;
        [HotfixBinding("Knight2")]
        public HotfixMBContainer        KnightItem2;
        [HotfixBinding("Knight3")]
        public HotfixMBContainer        KnightItem3;

        public CreatePlayerItem         CurSelectedItem;
        public List<CreatePlayerItem>   CreatePlayerItems;

        public override void OnInitialize()
        {
            // 初始化Items
            this.InitItems();
        }
        
        private void InitItems()
        {
            this.CreatePlayerItems = new List<CreatePlayerItem>();

            CreatePlayerItem rItem = new CreatePlayerItem();
            rItem.Initialize(this, KnightItem1, 911);
            this.CreatePlayerItems.Add(rItem);

            rItem = new CreatePlayerItem();
            rItem.Initialize(this, KnightItem2, 911);
            this.CreatePlayerItems.Add(rItem);

            rItem = new CreatePlayerItem();
            rItem.Initialize(this, KnightItem3, 911);
            this.CreatePlayerItems.Add(rItem);
        }

        public override void OnOpening()
        {
            this.CurSelectedItem = this.CreatePlayerItems[0];
            this.CurSelectedItem.StartLoad();
            this.mIsOpened = true;
        }

        public override void OnClosing()
        {
            this.CurSelectedItem.StopLoad();
            this.mIsClosed = true;

            for (int i = 0; i < this.CreatePlayerItems.Count; i++)
            {
                this.CreatePlayerItems[i].Destroy();
            }
        }

        [HotfixBindingEvent("PlayerCreateBtn", HEventTriggerType.PointerClick)]
        private void OnPlayerCreateBtn_Clicked(UnityEngine.Object rTarget)
        {
            if (string.IsNullOrEmpty(this.PlayerName.text))
            {
                Toast.Instance.Show("角色名不能为空！");
                return;
            }
            Game.Knight.CreatePlayer.Instance.Create(this.PlayerName.text, this.CurSelectedItem.ProfessionalID);
        }

        [HotfixBindingEvent("BackBtn", HEventTriggerType.PointerClick)]
        private void OnBackBtn_Clicked(UnityEngine.Object rTarget)
        {
            ViewManager.Instance.Open("KNPlayerList", View.State.dispatch);
        }
    }
}
