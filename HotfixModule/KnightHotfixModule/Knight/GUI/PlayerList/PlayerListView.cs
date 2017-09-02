//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine.UI;
using WindHotfix.GUI;
using Framework.Hotfix;
using Framework;
using UnityEngine.EventSystems;

namespace Game.Knight
{
    public class PlayerListView : TUIViewController<PlayerListView>
    {
        public NetPlayerItem    NetPlayerItemTemplate;
        public GridLayoutGroup  PlayerListContainer;

        public Button           StartGameBtn;
        public Button           CreatePlayerBtn;

        public NetPlayerItem    SelectedPlayerItem;
        public List<NetPlayerItem> PlayerItems;


        public override void OnInitialize()
        {
            this.PlayerListContainer    = this.Objects[1].Object as GridLayoutGroup;
            this.StartGameBtn           = this.Objects[2].Object as Button;
            this.CreatePlayerBtn        = this.Objects[3].Object as Button;
            this.SelectedPlayerItem     = null;

            this.EventBinding(this.StartGameBtn, EventTriggerType.PointerClick, OnStartGameBtn_Clicked);
            this.EventBinding(this.CreatePlayerBtn, EventTriggerType.PointerClick, OnCreatePlayerBtn_Clicked);
        }

        public override void OnOpening()
        {
            this.NetPlayerItemTemplate = new NetPlayerItem(this.Objects[0].Object as HotfixMBContainer);
            this.InitActorList();
        }

        public override void OnClosing()
        {
            if (this.SelectedPlayerItem != null)
                this.SelectedPlayerItem.StopLoad();

            this.EventUnBinding(this.StartGameBtn, EventTriggerType.PointerClick, OnStartGameBtn_Clicked);
            this.EventUnBinding(this.CreatePlayerBtn, EventTriggerType.PointerClick, OnCreatePlayerBtn_Clicked);

            for (int i = 0; i < this.PlayerItems.Count; i++)
            {
                this.PlayerItems[i].Destroy();
            }
        }

        public void OnStartGameBtn_Clicked(Object rTarget)
        {
            // 卸载当前场景
            CreatePlayer.Instance.UnloadScene();

            GameLoading.Instance.StartLoading(2.0f, "开始进入游戏世界...");
            UIViewManager.Instance.CloseView(this.GUID);
            GameFlowLevelManager.Instance.LoadLevel("World");
        }
        
        public void OnCreatePlayerBtn_Clicked(Object rTarget)
        {
            UIViewManager.Instance.Open("KNCreatePlayer", UIView.State.dispatch);
        }

        public void InitActorList()
        {
            this.PlayerItems = new List<NetPlayerItem>();
            List<NetActor> rActors = Account.Instance.NetActors;

            if (rActors != null)
            {
                for (int i = 0; i < rActors.Count; i++)
                {
                    GameObject rPlayerItemObj = GameObject.Instantiate(this.NetPlayerItemTemplate.GameObject);
                    rPlayerItemObj.SetActive(true);
                    var rMBContainer = rPlayerItemObj.GetComponent<HotfixMBContainer>();
                    var rNetPlayerItem = new NetPlayerItem(rMBContainer);

                    rNetPlayerItem.Parent = this;
                    rNetPlayerItem.Set(rActors[i]);

                    rNetPlayerItem.SetSelected(i == 0);
                    if (i == 0)
                    {
                        rNetPlayerItem.OnValueChanged();
                    }
                    
                    rPlayerItemObj.transform.SetParent(this.PlayerListContainer.transform, false);

                    this.PlayerItems.Add(rNetPlayerItem);
                }
            }
        }
    }
}
