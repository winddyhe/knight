//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using WindHotfix.Core;

namespace Game.Knight
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public class ActorController : THotfixMB<ActorController>
    {
        /// <summary>
        /// 角色
        /// </summary>
        public Actor            Actor;
        /// 刚体
        /// </summary>
        public Rigidbody        Rigidbody;
        /// <summary>
        /// 碰撞体
        /// </summary>
        public CapsuleCollider  Collider;
        /// <summary>
        /// 动作管理器
        /// </summary>
        public ActionManager    ActionMgr;
        
        private Vector3         mGroundNormal;
        private float           mGroundedY;
        private float           mTurnAmount;
        /// <summary>
        /// 旋转的速度
        /// </summary>
        private float           mForwardAmount;
        
        public float            groundCheckDistance = 5f;
        public float            movingTurnSpeed = 360;
        public float            stationaryTurnSpeed = 270;
        public float            jumpPower = 12.0f;
        [Range(1f, 4f)]
        public float            gravityMultiplier = 3.0f;


        public override void Start()
        {
            var rHero = this.Actor.Hero;
            this.Collider = this.Actor.ActorGo.ReceiveComponent<CapsuleCollider>();
            this.Collider.radius = rHero.Radius;
            this.Collider.height = rHero.Height;
            this.Collider.center = new Vector3(0, rHero.Height * 0.5f, 0);
            this.Rigidbody = this.Actor.ActorGo.ReceiveComponent<Rigidbody>();
            this.Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this.Rigidbody.isKinematic = true;
            this.Rigidbody.useGravity = false;
            //this.ActionMgr = this.Actor.ActorGo.ReceiveHotfixComponent<ActionManager>();
        }

        public void Move(Vector3 rMove, bool bIsRun)
        {
            if (this.Rigidbody == null) return;

            if (rMove.magnitude > 1.0f) rMove.Normalize();

            rMove = this.GameObject.transform.InverseTransformDirection(rMove);
            CheckGroundStatus();
            rMove = Vector3.ProjectOnPlane(rMove, mGroundNormal);
            mTurnAmount = Mathf.Atan2(rMove.x, rMove.z);
            mForwardAmount = rMove.z;

            // 和墙体的碰撞检测
            rMove = Move_RayForword(rMove);

            bool bIsMove = !rMove.Equals(Vector3.zero);
            
            this.GameObject.transform.Translate(rMove * 0.075f * (bIsRun ? 2 : 1));
            this.GameObject.transform.position = new Vector3(this.GameObject.transform.position.x, mGroundedY, this.GameObject.transform.position.z);

            // 应用角色的行走动画
            ApplyAnimation(bIsMove, bIsRun);
        }

        private void CheckGroundStatus()
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = this.GameObject.transform.position + Vector3.up * 1.0f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + Vector3.down * groundCheckDistance);
#endif
            if (Physics.Raycast(rActorPos, Vector3.down, out rHitInfo, groundCheckDistance, 1 << LayerMask.NameToLayer("Road")))
            {
                mGroundNormal = rHitInfo.normal;
                mGroundedY = rHitInfo.point.y;
            }
            else
            {
                mGroundNormal = Vector3.up;
                mGroundedY = 0;
            }

            ApplyExtraRatation();
        }

        private void ApplyExtraRatation() 
        {
            float fTurnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, mForwardAmount);
            this.GameObject.transform.Rotate(0, mTurnAmount * fTurnSpeed * Time.deltaTime, 0);
        }

        private void ApplyAnimation(bool bIsMove, bool bIsRun)
        {
            if (bIsRun)
            {
                this.ActionMgr.PlayRun(bIsMove);
                this.ActionMgr.PlayMove(bIsMove);
            }
            else
            {
                this.ActionMgr.PlayMove(bIsMove);
                this.ActionMgr.PlayRun(false);
            }
        }

        private Vector3 Move_RayForword(Vector3 rMove)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = this.GameObject.transform.position + Vector3.up * 0.2f;
            float rActorRadius = this.Actor.Hero.Radius + 0.2f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + rMove, Color.red);
            Debug.DrawLine(rActorPos, rActorPos + this.GameObject.transform.forward * rActorRadius, Color.green);
#endif
            if (Physics.Raycast(rActorPos, this.GameObject.transform.forward, out rHitInfo, rActorRadius, 1 << LayerMask.NameToLayer("Wall")))
            {
                rMove = Vector3.ProjectOnPlane(rMove, rHitInfo.normal);
                int k = Vector3.Dot(this.GameObject.transform.forward, rMove) >= 0 ? 1 : -1;
                rMove = this.GameObject.transform.InverseTransformDirection(rMove) * k;
#if UNITY_EDITOR
                Debug.DrawLine(rActorPos, rActorPos + rMove, Color.yellow);
#endif
            }
            return rMove;
        }
    }
}