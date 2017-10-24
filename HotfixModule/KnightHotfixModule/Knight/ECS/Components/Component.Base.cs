using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;
using WindHotfix.Core;

namespace Game.Knight
{
    public class ComponentAnimator : GameComponent
    {
        public bool                 IsMove;
        public bool                 IsRun;
    }

    public class ComponentCollider : GameComponent
    {
        public float                Radius;
        public float                Height;
    }

    public class ComponentMove : GameComponent
    {
        /// <summary>
        /// 移动用的
        /// </summary>
        public Vector3              MoveSpeed;
        public float                TurnSpeed;
        /// <summary>
        /// 速度系数
        /// </summary>
        public float                SpeedRate           = 1.0f;
        
        /// <summary>
        /// 地面监测用的
        /// </summary>
        public Vector3              GroundNormal        = Vector3.up;
        public float                GroundedY           = 0.0f;
        /// <summary>
        /// 转向用的
        /// </summary>
        public float                TurnAmount          = 0.0f;
        public float                ForwardAmount       = 0.0f;

        public float                GroundCheckDistance = 5f;
        public float                MovingTurnSpeed     = 360;
        public float                StationaryTurnSpeed = 270;
        public float                JumpPower           = 12.0f;
        public float                GravityMultiplier   = 3.0f;
    }

    public class ComponentTransform : GameComponent
    {
        public Vector3              Position            = Vector3.zero;
        public Vector3              Scale               = Vector3.one;
        public Vector3              Forward             = Vector3.forward;

        public Vector3              Up                  = Vector3.up;
        public Vector3              TempForword         = new Vector3(1, 0, 1);
    }

    public class ComponentInput : GameComponent
    {
        public float                HorizontalInput     = 0.0f;
        public float                VerticalInput       = 0.0f;

        public bool                 IsRunInput          = false;

        public Vector3              TempForword         = new Vector3(1, 0, 1);
    }

    public class ComponentHero : GameComponent
    {
        public ActorHero            Hero;
    }

    public class ComponentAvatar : GameComponent
    {
        public ActorAvatar          Avatar;
    }
    
    public class ComponentProfessional : GameComponent
    {
        public ActorProfessional    Professional;
    }

    public class ComponentNet : GameComponent
    {
        public ActorNet             NetActor;
    }
}
