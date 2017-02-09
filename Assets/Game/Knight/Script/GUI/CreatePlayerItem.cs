//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Knight
{
    public class CreatePlayerItem : MonoBehaviour
    {
        public Toggle           SelectedPlayer;
        public CreatePlayerView Parent;
        public int              ProfessionalID;

        private Actor.ActorCreateRequest    mActorCreateRequest;

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
