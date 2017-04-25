//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine.UI;
using Core;

namespace Game.Knight
{
    public class PlayerListView : View
    {
        public NetPlayerItem    NetPlayerItemTemplate;
        public GridLayoutGroup  PlayerListContainer;

        public NetPlayerItem    SelectedPlayerItem;

        protected override void InitializeViewController()
        {
           // this.viewController = new PlayerListViewController(this);
        }

        public void OnCreatePlayerBtn_Clicked()
        {
            UIManager.Instance.Open("KNCreatePlayer", State.dispatch);
        }

        public void OnStartGameBtn_Clicked()
        {
            GameLoading.Instance.StartLoading(2.0f, "开始进入游戏世界...");
            UIManager.Instance.CloseView(this.GUID);
            Globals.Instance.LoadLevel("World");
        }
        
        public void RefreshActorList()
        {
            List<NetActor> rActors = Account.Instance.NetActors;
            if (rActors != null)
            {
                for (int i = 0; i < rActors.Count; i++)
                {
                    GameObject rPlayerItemObj = GameObject.Instantiate(this.NetPlayerItemTemplate.gameObject);
                    var rNetPlayerItem = rPlayerItemObj.ReceiveComponent<NetPlayerItem>();
                    rNetPlayerItem.Parent = this;
                    rNetPlayerItem.Set(rActors[i]);

                    rNetPlayerItem.SetSelected(i == 0);
                    if (i == 0) this.SelectedPlayerItem.OnValueChanged();

                    rPlayerItemObj.SetActive(true);
                    rPlayerItemObj.transform.SetParent(this.PlayerListContainer.transform, false);
                }
            }
        }
    }

    public class PlayerListViewController : ViewController
    {
        public PlayerListViewController(PlayerListView rView)
            : base()
        {
        }

        public override void OnOpening()
        {
            //this.mView.RefreshActorList();

            //this.mView.IsOpened = true;
        }

        public override void OnClosing()
        {
            //this.mView.IsClosed = true;

            //if (this.mView.SelectedPlayerItem != null)
            //    this.mView.SelectedPlayerItem.StopLoad();
        }
    }
}
