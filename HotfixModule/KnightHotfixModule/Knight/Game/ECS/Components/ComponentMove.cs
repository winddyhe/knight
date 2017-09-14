using UnityEngine;
using System.Collections;
using WindHotfix.Game;

namespace Game.Knight
{
    public class ComponentMove : GameComponent
    {
        /// <summary>
        /// 移动用的
        /// </summary>
        public Vector3      MoveSpeed;
        public Vector3      Position;
        public Vector3      Forward;
        public float        TurnSpeed;
        
        /// <summary>
        /// 地面监测用的
        /// </summary>
        public Vector3      GroundNormal;
        public float        GroundedY;
        /// <summary>
        /// 转向用的
        /// </summary>
        public float        TurnAmount;
        public float        ForwardAmount;

        public float        GroundCheckDistance = 5f;
        public float        MovingTurnSpeed     = 360;
        public float        StationaryTurnSpeed = 270;
        public float        JumpPower           = 12.0f;
        public float        GravityMultiplier   = 3.0f;
    }
}