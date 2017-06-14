//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Framework.WindUI;
using UnityEngine.UI;
using Core;
using WindHotfix.Core;
using UnityEngine.AssetBundles;

namespace Game.Knight
{
    public class NetPlayerItem : THotfixMB<NetPlayerItem>
    {
        public TextFormat                   ActorProfession;
        public TextFormat                   ActorLevel;
        public Text                         ActorName;
        public Toggle                       SelectedToggle;

        public PlayerListView               Parent;

        private NetActor                    mNetActor;
        private Actor.ActorCreateRequest    mActorCreateRequest;

        public override void OnInitialize()
        {
            this.ActorProfession = this.Objects[0].Object as TextFormat;
            this.ActorLevel      = this.Objects[1].Object as TextFormat;
            this.ActorName       = this.Objects[2].Object as Text;
            this.SelectedToggle  = this.Objects[3].Object as Toggle;

            this.AddEventListener(this.SelectedToggle, (rTarget) => { OnValueChanged(); });
        }

        public void Set(NetActor rNetActor)
        {
            this.mNetActor = rNetActor;
            this.ActorProfession.Set(rNetActor.Professional.Name);
            this.ActorLevel.Set(rNetActor.Level);
            this.ActorName.text = rNetActor.ActorName;
        }

        public Actor GetActor()
        {
            if (mActorCreateRequest == null) return null;
            return mActorCreateRequest.Actor;
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
                mActorCreateRequest = Actor.CreateActor(this.mNetActor.ActorID, this.mNetActor.Professional.HeroID, ActorLoadCompleted);
            }
            else if (!this.SelectedToggle.isOn)
            {
                Actor.DestoryActor(mActorCreateRequest.Actor.Hero.ID);
                UtilTool.SafeDestroy(mActorCreateRequest.Actor.ExhibitActor.ActorGo);
                mActorCreateRequest.Stop();
            }
        }

        public void StopLoad()
        {
            if (mActorCreateRequest != null)
            {
                if (mActorCreateRequest.Actor != null && 
                    mActorCreateRequest.Actor.ExhibitActor != null && 
                    mActorCreateRequest.Actor.ExhibitActor.ActorGo != null)
                {
                    Actor.DestoryActor(mActorCreateRequest.Actor.Hero.ID);
                    UtilTool.SafeDestroy(mActorCreateRequest.Actor.ExhibitActor.ActorGo);
                }
                mActorCreateRequest.Stop();
            }
        }

        private void ActorLoadCompleted(Actor rActor)
        {
            var rActorPos = rActor.ActorGo.transform.position;
            RaycastHit rHitInfo;
            if (Physics.Raycast(rActorPos + Vector3.up * 5.0f, Vector3.down, out rHitInfo, 20, 1 << LayerMask.NameToLayer("Road")))
            {
                rActorPos = new Vector3(rActorPos.x, rHitInfo.point.y, rActorPos.z);
            }
            rActor.ActorGo.transform.position = rActorPos;
        }
    }
}

