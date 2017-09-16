using System;
using System.Collections;
using System.Collections.Generic;
using WindHotfix.Game;
using UnityEngine;
using Framework;

namespace Game.Knight
{
    public class EntityPlayer : GameEntity
    {
        public ComponentNet             CompNet;
        public ComponentProfessional    CompPrefessional;
        public ComponentAvatar          CompAvatar;
        public ComponentHero            CompHero;

        public ComponentAnimator        CompAnimator;
        public ComponentCollider        CompCollider;
        public ComponentMove            CompMove;
        public ComponentTransform       CompTrans;
        public ComponentInput           CompInput;

        public ComponentUnityAnimator   CompUnityAnimator;
        public ComponentUnityGo         CompUnitGo;

        public IEnumerator Create(ActorNet rNetActor, Vector3 rBornPos)
        {
            // 创建Component net
            this.CompNet = this.AddComponent<ComponentNet>();
            this.CompNet.NetActor = rNetActor;

            // 创建Component Professional
            ActorProfessional rProfessional = GameConfig.Instance.GetActorProfessional(rNetActor.ProfessionalID);
            if (rProfessional == null)
            {
                Debug.LogErrorFormat("Cannot find professional ID: {0}", rNetActor.ProfessionalID);
                yield break;
            }
            this.CompPrefessional = this.AddComponent<ComponentProfessional>();
            this.CompPrefessional.Professional = rProfessional;

            // 创建Component Hero
            ActorHero rHero = GameConfig.Instance.GetHero(rProfessional.HeroID);
            if (rHero == null)
            {
                Debug.LogErrorFormat("Cannot find hero ID: {0}", rProfessional.HeroID);
                yield break;
            }
            this.CompHero = this.AddComponent<ComponentHero>();
            this.CompHero.Hero = rHero;

            // 创建Component Avatar
            ActorAvatar rAvatar = GameConfig.Instance.GetAvatar(rHero.AvatarID);
            if (rAvatar == null)
            {
                Debug.LogErrorFormat("Cannot find avatar ID: {0}", rHero.AvatarID);
                yield break;
            }
            this.CompAvatar = this.AddComponent<ComponentAvatar>();
            this.CompAvatar.Avatar = rAvatar;

            // 根据Avatar加载角色
            var rAvatarRequest = AvatarAssetLoader.Instance.Load(this.CompAvatar.Avatar.ABPath, this.CompAvatar.Avatar.AssetName);
            yield return rAvatarRequest;
            if (rAvatarRequest.AvatarGo == null)
            {
                Debug.LogError("Avatar load failed..");
                yield break;
            }

            // 创建Component GameObject
            this.CompUnitGo = this.AddComponent<ComponentUnityGo>();
            this.CompUnitGo.GameObject = rAvatarRequest.AvatarGo;

            // 创建Component Unity Animator
            this.CompUnityAnimator = this.AddComponent<ComponentUnityAnimator>();
            this.CompUnityAnimator.Animator = rAvatarRequest.AvatarGo.GetComponent<Animator>();

            // 创建其他的Component
            this.CompAnimator = this.AddComponent<ComponentAnimator>();

            this.CompCollider = this.AddComponent<ComponentCollider>();
            this.CompCollider.Radius = rHero.Radius;
            this.CompCollider.Height = rHero.Height;

            this.CompMove = this.AddComponent<ComponentMove>();

            this.CompTrans = this.AddComponent<ComponentTransform>();
            this.CompTrans.Position = rBornPos;
            this.CompTrans.Forward = Vector3.forward;
            this.CompTrans.Scale = Vector3.one * rHero.Scale;

            this.CompInput = this.AddComponent<ComponentInput>();
        }
    }
}
