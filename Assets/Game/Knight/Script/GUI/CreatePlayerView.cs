//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;
using UnityEngine.UI;

namespace Game.Knight
{
    public class CreatePlayerView : View
    {
        #region unity_bind

        public ToggleGroup          ProfessionSelected;
        public InputField           PlayerName;
        public Text                 ProfessionalDesc;

        public CreatePlayerItem     CurrentSelectedItem;

        #endregion

        protected override void InitializeViewController()
        {
            //this.viewController = new CreatePlayerViewController(this);
        }

        public void OnPlayerCreateBtn_Clicked()
        {
            if (string.IsNullOrEmpty(this.PlayerName.text))
            {
                Toast.Instance.Show("角色名不能为空！");
                return;
            }
            CreatePlayer.Instance.Create(this.PlayerName.text, this.CurrentSelectedItem.ProfessionalID);
        }

        public void OnBackBtn_Clicked()
        {
            UIManager.Instance.Open("KNPlayerList", View.State.dispatch);
        }
    }

    public class CreatePlayerViewController : ViewController
    {
        public CreatePlayerViewController(CreatePlayerView rView) 
            : base()
        {
        }

        public override void OnOpening()
        {
            //this.mView.CurrentSelectedItem.StartLoad();
            //this.mView.IsOpened = true;
        }

        public override void OnClosing()
        {
            //this.mView.CurrentSelectedItem.StopLoad();
            //this.mView.IsClosed = true;
        }
    }
}
