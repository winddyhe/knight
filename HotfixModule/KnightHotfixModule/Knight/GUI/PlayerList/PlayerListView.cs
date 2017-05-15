//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine.UI;
using Core;
using WindHotfix.GUI;
using Framework.Hotfix;
using WindHotfix.Core;

namespace Game.Knight
{
    public class PlayerListView : THotfixViewController<PlayerListView>
    {
        public NetPlayerItem    NetPlayerItemTemplate;
        public GridLayoutGroup  PlayerListContainer;

        public Button           StartGameBtn;
        public Button           CreatePlayerBtn;

        public NetPlayerItem    SelectedPlayerItem;


        public override void OnInitialize()
        {
            this.PlayerListContainer    = this.Objects[1].Object as GridLayoutGroup;
            this.StartGameBtn           = this.Objects[2].Object as Button;
            this.CreatePlayerBtn        = this.Objects[3].Object as Button;
            this.SelectedPlayerItem     = null;

            this.AddEventListener(this.StartGameBtn, OnStartGameBtn_Clicked);
            this.AddEventListener(this.CreatePlayerBtn, OnCreatePlayerBtn_Clicked);
        }

        public override void OnOpening()
        {
            this.NetPlayerItemTemplate = (this.Objects[0].Object as HotfixMBContainer).MBHotfixObject as NetPlayerItem;
            this.RefreshActorList();
        }

        public override void OnClosing()
        {
            if (this.SelectedPlayerItem != null)
                this.SelectedPlayerItem.StopLoad();
        }

        public void OnStartGameBtn_Clicked(Object rTarget)
        {
            GameLoading.Instance.StartLoading(2.0f, "开始进入游戏世界...");
            UIManager.Instance.CloseView(this.GUID);
            Globals.Instance.LoadLevel("World");
        }
        
        public void OnCreatePlayerBtn_Clicked(Object rTarget)
        {
            UIManager.Instance.Open("KNCreatePlayer", View.State.dispatch);
        }

        public void RefreshActorList()
        {
            List<NetActor> rActors = Account.Instance.NetActors;
            if (rActors != null)
            {
                for (int i = 0; i < rActors.Count; i++)
                {
                    GameObject rPlayerItemObj = GameObject.Instantiate(this.NetPlayerItemTemplate.GameObject);
                    rPlayerItemObj.SetActive(true);
                    var rMBContainer = rPlayerItemObj.GetComponent<HotfixMBContainer>();
                    var rNetPlayerItem = rMBContainer.MBHotfixObject as NetPlayerItem;

                    rNetPlayerItem.Parent = this;
                    rNetPlayerItem.Set(rActors[i]);

                    rNetPlayerItem.SetSelected(i == 0);
                    if (i == 0) this.SelectedPlayerItem.OnValueChanged();
                    
                    rPlayerItemObj.transform.SetParent(this.PlayerListContainer.transform, false);
                }
            }
        }
    }
}
