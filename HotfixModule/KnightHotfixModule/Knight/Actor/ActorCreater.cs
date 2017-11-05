//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Core;
using WindHotfix.Core;
using Framework;
using System.Threading.Tasks;

namespace Game.Knight
{
    public static class ActorCreater
    {
        public class ActorCreateRequest
        {
            public Task             Handler;
            public ActorAvatar      Avatar;
            public ActorHero        Hero;
            public GameObject       ActorGo;

            public void Stop()
            {
                //CoroutineManager.Instance.Stop(this.Handler);
                this.Handler.Dispose();
            }
        }

        public static ActorCreateRequest CreateActor(ActorNet rNetActor, System.Action<GameObject> rLoadCompleted = null)
        {
            if (rNetActor == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }
            ActorProfessional rProfessional = GameConfig.Instance.GetActorProfessional(rNetActor.ProfessionalID);
            if (rProfessional == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }
            return CreateActor(rNetActor.ActorID, rProfessional.HeroID, rLoadCompleted);
        }

        public static ActorCreateRequest CreateActor(long rActorID, int rHeroID, System.Action<GameObject> rLoadCompleted = null)
        {
            var rHero = GameConfig.Instance.GetHero(rHeroID);
            if (rHero == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }

            var rAvatar = GameConfig.Instance.GetAvatar(rHero.AvatarID);
            if (rAvatar == null)
            {
                UtilTool.SafeExecute(rLoadCompleted, null);
                return null;
            }

            var rRequest = new ActorCreateRequest() { Avatar = rAvatar, Hero = rHero };
            rRequest.Handler = CreateActor_Async(rRequest, rLoadCompleted);
            return rRequest;
        }

        public static void DestoryActor(int rHeroID, bool bIsDelayUnload = true)
        {
            var rHero = GameConfig.Instance.GetHero(rHeroID);
            if (rHero == null) return;

            var rAvatar = GameConfig.Instance.GetAvatar(rHero.AvatarID);
            if (rAvatar == null) return;

            AvatarAssetLoader.Instance.UnloadAsset(rAvatar.ABPath, bIsDelayUnload);
        }

        private static async Task CreateActor_Async(ActorCreateRequest rActorCreateRequest, System.Action<GameObject> rLoadCompleted)
        {
            var rAvatarRequest = await AvatarAssetLoader.Instance.Load(rActorCreateRequest.Avatar.ABPath, rActorCreateRequest.Avatar.AssetName);
            rActorCreateRequest.ActorGo = rAvatarRequest.AvatarGo;
            
            // 设置ActorGo的大小
            rAvatarRequest.AvatarGo.transform.localScale = Vector3.one * rActorCreateRequest.Hero.Scale;

            UtilTool.SetLayer(rAvatarRequest.AvatarGo, "Actor", true);
            UtilTool.SafeExecute(rLoadCompleted, rAvatarRequest.AvatarGo);
        }
    }
}


