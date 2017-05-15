//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.Collections;
using WindHotfix.Core;

namespace Game.Knight
{
    public partial class Actor
    {
        public class ActorCreateRequest
        {
            public CoroutineHandler Handler;
            public Actor            Actor;

            public void Stop()
            {
                CoroutineManager.Instance.Stop(this.Handler);
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

            ActorCreateRequest rRequest = new ActorCreateRequest() { Actor = rActor };
            rRequest.Handler = CoroutineManager.Instance.StartHandler(CreateActor_Async(rActor, rLoadCompleted));
            return rRequest;
        }

        private static IEnumerator CreateActor_Async(Actor rActor, System.Action<Actor> rLoadCompleted)
        {
            var rAvatarRequest = AvatarAssetLoader.Instance.Load(rActor.Avatar.ABPath, rActor.Avatar.AssetName);
            yield return rAvatarRequest;

            ExhibitActor rExhibitActor = new ExhibitActor();
            rExhibitActor.Actor = rActor;
            rExhibitActor.ActorGo = rAvatarRequest.AvatarGo;
            rExhibitActor.Actor.ExhibitActor = rExhibitActor;

            // 设置ActorGo的大小
            var rHero = rActor.Hero;
            rExhibitActor.ActorGo.transform.localScale = new Vector3(rHero.Scale, rHero.Scale, rHero.Scale);

            // 添加角色控制器
            var rActorController = rActor.ActorGo.ReceiveHotfixComponent<ActorController>();
            rActorController.Actor = rActor;

            // 添加角色的技能管理器
            var rActorGamePlayMgr = rActor.ActorGo.ReceiveHotfixComponent<ActorGamePlayManager>();
            rActorGamePlayMgr.Initialize(rActor);

            UtilTool.SetLayer(rExhibitActor.ActorGo, "Actor", true);
            
            UtilTool.SafeExecute(rLoadCompleted, rExhibitActor.Actor);
        }
    }
}
