//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Game.Knight;
using UnityEngine.UI;
using Core;
using UnityEngine;
using WindHotfix.Core;
using Framework.WindUI;

namespace KnightHotfixModule.Knight
{
    public class CreatePlayerItem : THotfixMB<CreatePlayerItem>
    {
        public Toggle                       SelectedPlayer;
        public CreatePlayerView             Parent;
        public int                          ProfessionalID;

        private Actor.ActorCreateRequest    mActorCreateRequest;

        public override void OnInitialize()
        {
            this.SelectedPlayer = this.Objects[0].Object as Toggle;
            this.Parent = (this.Objects[1].Object as View).ViewController as CreatePlayerView;
        }

        public void OnToggleSelectedValueChanged()
        {
            if (this.SelectedPlayer.isOn && this.Parent.CurrentSelectedItem != this)
            {
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
            mActorCreateRequest = Actor.CreateActor(-1, rProfessional.HeroID, ActorLoadCompleted);
        }

        public void StopLoad()
        {
            if (mActorCreateRequest != null)
            {
                UtilTool.SafeDestroy(mActorCreateRequest.actor.ExhibitActor.ActorGo);
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
