//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine.UI;
using Core;
using UnityEngine;
using WindHotfix.Core;
using Framework.WindUI;
using UnityEngine.AssetBundles;
using Framework.Hotfix;
using UnityEngine.EventSystems;

namespace Game.Knight
{
    public class CreatePlayerItem
    {
        public Toggle                           SelectedPlayer;
        public CreatePlayerView                 Parent;
        public int                              ProfessionalID;

        private ActorCreater.ActorCreateRequest mActorCreateRequest;

        public void Initialize(CreatePlayerView rParent, HotfixMBContainer rMBContainer, int nProfessionalID)
        {
            // 这里的调用次序的问题 必须要等待下一帧才能得到ViewController的值
            this.Parent = rParent;
            this.SelectedPlayer = rMBContainer.Objects[0].Object as Toggle;

            HotfixEventManager.Instance.Binding(this.SelectedPlayer, (EventTriggerType)100, (rTarget) => { OnToggleSelectedValueChanged(); });

            // 获取ProfessionalID
            this.ProfessionalID = nProfessionalID;
        }

        public void Destroy()
        {
            HotfixEventManager.Instance.UnBinding(this.SelectedPlayer, (EventTriggerType)100, (rTarget) => { OnToggleSelectedValueChanged(); });
        }
        
        public void OnToggleSelectedValueChanged()
        {
            if (this.SelectedPlayer.isOn && this.Parent != null && this.Parent.CurrentSelectedItem != this)
            {
                StopLoad();
                StartLoad();
                this.Parent.CurrentSelectedItem = this;
            }
            else if (!this.SelectedPlayer.isOn)
            {
                StopLoad();
            }
        }

        public void StartLoad()
        {
            ActorProfessional rProfessional = GameConfig.Instance.GetActorProfessional(this.ProfessionalID);
            this.Parent.ProfessionalDesc.text = rProfessional.Desc;
            mActorCreateRequest = ActorCreater.CreateActor(-1, rProfessional.HeroID, ActorLoadCompleted);
        }

        public void StopLoad()
        {
            if (mActorCreateRequest != null)
            {
                if (mActorCreateRequest.ActorGo != null)
                {
                    ActorCreater.DestoryActor(mActorCreateRequest.Hero.ID);
                    UtilTool.SafeDestroy(mActorCreateRequest.ActorGo);
                }
                mActorCreateRequest.Stop();
            }
        }

        private void ActorLoadCompleted(GameObject rActorGo)
        {
            var rActorPos = rActorGo.transform.position;
            RaycastHit rHitInfo;
            if (Physics.Raycast(rActorPos + Vector3.up * 5.0f, Vector3.down, out rHitInfo, 20, 1 << LayerMask.NameToLayer("Road")))
            {
                rActorPos = new Vector3(rActorPos.x, rHitInfo.point.y, rActorPos.z);
            }
            rActorGo.transform.position = rActorPos;
        }
    }
}
