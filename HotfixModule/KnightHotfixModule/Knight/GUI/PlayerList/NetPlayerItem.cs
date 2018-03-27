//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Framework.WindUI;
using UnityEngine.UI;
using Core;
using Framework.Hotfix;
using UnityEngine.EventSystems;

namespace Game.Knight
{
    public class NetPlayerItem
    {
        public GameObject                       GameObject;
        public TextFormat                       ActorProfession;
        public TextFormat                       ActorLevel;
        public Text                             ActorName;
        public Toggle                           SelectedToggle;

        public PlayerListView                   Parent;

        private ActorNet                        mNetActor;
        private ActorCreater.ActorCreateRequest mActorCreateRequest;

        public NetPlayerItem(HotfixMBContainer rMBContainer)
        {
            this.GameObject      = rMBContainer.gameObject;   
            this.ActorProfession = rMBContainer.Objects[0].Object as TextFormat;
            this.ActorLevel      = rMBContainer.Objects[1].Object as TextFormat;
            this.ActorName       = rMBContainer.Objects[2].Object as Text;
            this.SelectedToggle  = rMBContainer.Objects[3].Object as Toggle;

            HotfixEventManager.Instance.Binding(this.SelectedToggle, Framework.HEventTriggerType.Select, (rTarget) => { OnValueChanged(); });
        }

        public void Destroy()
        {
            HotfixEventManager.Instance.UnBinding(this.SelectedToggle, Framework.HEventTriggerType.Select, (rTarget) => { OnValueChanged(); });
        }

        public void Set(ActorNet rNetActor)
        {
            this.mNetActor = rNetActor;
            ActorProfessional rProfessional = GameConfig.Instance.GetActorProfessional(rNetActor.ProfessionalID);
            this.ActorProfession.Set(rProfessional.Name);
            this.ActorLevel.Set(rNetActor.Level);
            this.ActorName.text = rNetActor.ActorName;
        }

        public GameObject GetActorGo()
        {
            if (mActorCreateRequest == null) return null;
            return mActorCreateRequest.ActorGo;
        }

        public void SetSelected(bool isSelected)
        {
            this.SelectedToggle.isOn = isSelected;
        }

        public void OnValueChanged()
        {
            if (this.SelectedToggle.isOn && this.Parent.SelectedPlayerItem != this)
            {
                StopLoad();
                Account.Instance.ActiveActor = this.mNetActor;
                this.Parent.SelectedPlayerItem = this;
                ActorProfessional rProfessional = GameConfig.Instance.GetActorProfessional(this.mNetActor.ProfessionalID);
                mActorCreateRequest = ActorCreater.CreateActor(this.mNetActor.ActorID, rProfessional.HeroID, ActorLoadCompleted);
            }
            else if (!this.SelectedToggle.isOn)
            {
                ActorCreater.DestoryActor(mActorCreateRequest.Hero.ID);
                UtilTool.SafeDestroy(mActorCreateRequest.ActorGo);
                mActorCreateRequest.Stop();
            }
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

