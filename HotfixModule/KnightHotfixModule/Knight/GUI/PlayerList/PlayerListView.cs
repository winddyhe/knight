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
using WindHotfix.Core;

namespace Game.Knight
{
    public class PlayerListView : TViewController<PlayerListView>
    {
        [HotfixBinding("NetPlayerItem")]
        public HotfixMBContainer    NetPlayerItem;
        [HotfixBinding("Content")]
        public GridLayoutGroup      PlayerListContainer;

        public NetPlayerItem        NetPlayerItemTemplate;

        public NetPlayerItem        SelectedPlayerItem;
        public List<NetPlayerItem>  PlayerItems;

        public override void OnInitialize()
        {
            this.PlayerListContainer    = this.Objects[1].Object as GridLayoutGroup;
            this.SelectedPlayerItem     = null;
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

            for (int i = 0; i < this.PlayerItems.Count; i++)
            {
                this.PlayerItems[i].Destroy();
            }
        }

        [HotfixBindingEvent("StartGameBtn", HEventTriggerType.PointerClick)]
        public void OnStartGameBtn_Clicked(Object rTarget)
        {
            // 卸载当前场景
            CreatePlayer.Instance.UnloadScene();

            GameLoading.Instance.StartLoading(2.0f, "开始进入游戏世界...");
            ViewManager.Instance.CloseView(this.GUID);
            GameFlowLevelManager.Instance.LoadLevel("World");
        }

        [HotfixBindingEvent("CreatePlayerBtn", HEventTriggerType.PointerClick)]
        public void OnCreatePlayerBtn_Clicked(Object rTarget)
        {
            ViewManager.Instance.Open("KNCreatePlayer", View.State.dispatch);
        }

        public void InitActorList()
        {
            this.PlayerItems = new List<NetPlayerItem>();
            List<ActorNet> rActors = Account.Instance.NetActors;

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
