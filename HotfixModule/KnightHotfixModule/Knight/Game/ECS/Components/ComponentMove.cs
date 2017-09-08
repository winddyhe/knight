using UnityEngine;
using System.Collections;
using WindHotfix.Game;

namespace Game.Knight
{
    public class ComponentMove : GameComponent
    {
        public Transform    Trans;
        public Vector3      MoveSpeed;


        public Vector3      mGroundNormal;
        public float        mGroundedY;
        public float        mTurnAmount;
        public float        mForwardAmount;

        public float        groundCheckDistance = 5f;
        public float        movingTurnSpeed = 360;
        public float        stationaryTurnSpeed = 270;
        public float        jumpPower = 12.0f;
        public float        gravityMultiplier = 3.0f;
    }
}