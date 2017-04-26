//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.WindUI;
using UnityEngine.UI;
using Framework.Hotfix;
using WindHotfix.GUI;

namespace KnightHotfixModule.Knight
{
    public class CreatePlayerView : THotfixViewController<CreatePlayerView>
    {
        public ToggleGroup      ProfessionSelected;
        public InputField       PlayerName;
        public Text             ProfessionalDesc;
        public CreatePlayerItem CurrentSelectedItem;
        
        public override void OnInitialize()
        {
            // 转换变量
            this.ProfessionSelected  = this.Objects[0].Object as ToggleGroup;
            this.PlayerName          = this.Objects[1].Object as InputField;
            this.ProfessionalDesc    = this.Objects[2].Object as Text;
            this.CurrentSelectedItem = (this.Objects[3].Object as HotfixMBContainer).MBHotfixObject as CreatePlayerItem;
            
            // 注册事件
            this.AddEventListener(this.Objects[4].Object, OnPlayerCreateBtn_Clicked);
            this.AddEventListener(this.Objects[5].Object, OnBackBtn_Clicked);
        }
        
        public override void OnOpening()
        {
            this.CurrentSelectedItem.StartLoad();
            this.mIsOpened = true;
        }

        public override void OnClosing()
        {
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
            KnightHotfixModule.Knight.CreatePlayer.Instance.Create(this.PlayerName.text, this.CurrentSelectedItem.ProfessionalID);
        }

        private void OnBackBtn_Clicked(UnityEngine.Object rTarget)
        {
            UIManager.Instance.Open("KNPlayerList", View.State.dispatch);
        }
    }
}
