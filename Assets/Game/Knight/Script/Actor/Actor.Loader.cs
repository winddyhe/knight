//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.Collections;

namespace Game.Knight
{
    public partial class Actor
    {
        public class ActorCreateRequest : BaseCoroutineRequest<ActorCreateRequest>
        {
            public Actor         actor;
            public Action<Actor> loadCompleted;

            public ActorCreateRequest(Actor rActor, System.Action<Actor> rLoadCompleted)
            {
                this.actor = rActor;
                this.loadCompleted = rLoadCompleted;
            }
        }

        public static ActorCreateRequest CreateActor(NetActor rNetActor, System.Action<Actor> rLoadCompleted = null)
        {
            if (rNetActor == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }
            return CreateActor(rNetActor.ActorID, rNetActor.Professional.HeroID, rLoadCompleted);
        }

        public static ActorCreateRequest CreateActor(long rActorID, int rHeroID, System.Action<Actor> rLoadCompleted = null)
        {
            Hero rHero = GameConfig.Instance.GetHero(rHeroID);
            if (rHero == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }

            Avatar rAvatar = GameConfig.Instance.GetAvatar(rHero.AvatarID);
            if (rAvatar == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }
            
            Actor rActor = new Actor();
            rActor.Avatar = rAvatar;
            rActor.Hero = rHero;
            
            var rCreateRequest = new ActorCreateRequest(rActor, rLoadCompleted);
            rCreateRequest.Start(CreateActor_Async(rCreateRequest));

            return rCreateRequest;
        }

        private static IEnumerator CreateActor_Async(ActorCreateRequest rCreateRequest)
        {
            var rAvatarRequest = AvatarAssetLoader.Instance.Load(rCreateRequest.actor.Avatar);
            yield return rAvatarRequest.Coroutine;

            ExhibitActor rExhibitActor = new ExhibitActor();
            rExhibitActor.Actor = rCreateRequest.actor;
            rExhibitActor.ActorGo = rAvatarRequest.avatarGo;
            rExhibitActor.Actor.ExhibitActor = rExhibitActor;

            // 设置ActorGo的大小
            var rHero = rCreateRequest.actor.Hero;
            rExhibitActor.ActorGo.transform.localScale = new Vector3(rHero.Scale, rHero.Scale, rHero.Scale);

            // 添加角色控制器
            var rActorController = rCreateRequest.actor.ActorGo.ReceiveComponent<ActorController>();
            rActorController.Actor = rCreateRequest.actor;

            // 添加角色的技能管理器
            var rActorGamePlayMgr = rCreateRequest.actor.ActorGo.ReceiveComponent<ActorGamePlayManager>();
            rActorGamePlayMgr.Initialize(rCreateRequest.actor);

            UtilTool.SetLayer(rExhibitActor.ActorGo, "Actor", true);
            
            UtilTool.SafeExecute(rCreateRequest.loadCompleted, rExhibitActor.Actor);
        }
    }
}
