using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemMove : GameSystem
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            if (!this.IsActive) return;

            ECSManager.Instance.ForeachEntities(this.mResultComps, (rComps) => 
            {
                this.Move(this.mResultComps.Get<ComponentMove>(), this.mResultComps.Get<ComponentAnimator>());
            },
            typeof(ComponentMove),
            typeof(ComponentAnimator));
        }

        private void Move(ComponentMove rCompMove, ComponentAnimator rCompAnim)
        {
            if (rCompMove.MoveSpeed.magnitude > 1.0f) rCompMove.MoveSpeed.Normalize();

            rCompMove.MoveSpeed = rCompMove.Trans.InverseTransformDirection(rCompMove.MoveSpeed);
            CheckGroundStatus(rCompMove, rCompAnim);
            rCompMove.MoveSpeed = Vector3.ProjectOnPlane(rCompMove.MoveSpeed, rCompMove.mGroundNormal);
            rCompMove.mTurnAmount = Mathf.Atan2(rCompMove.MoveSpeed.x, rCompMove.MoveSpeed.z);
            rCompMove.mForwardAmount = rCompMove.MoveSpeed.z;

            // 和墙体的碰撞检测
            rCompMove.MoveSpeed = Move_RayForword(rCompMove, rCompMove.MoveSpeed);

            bool bIsMove = !rCompMove.MoveSpeed.Equals(Vector3.zero);

            rCompMove.Trans.Translate(rCompMove.MoveSpeed * 0.075f * (rCompAnim.IsRun ? 2 : 1));
            rCompMove.Trans.position = new Vector3(rCompMove.Trans.position.x, rCompMove.mGroundedY, rCompMove.Trans.position.z);

            // 应用角色的行走动画
            // ApplyAnimation(bIsMove, rCompAnim.IsRun);
        }

        private void CheckGroundStatus(ComponentMove rCompMove, ComponentAnimator rCompAnim)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompMove.Trans.position + Vector3.up * 1.0f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + Vector3.down * rCompMove.groundCheckDistance);
#endif
            if (Physics.Raycast(rActorPos, Vector3.down, out rHitInfo, rCompMove.groundCheckDistance, 1 << LayerMask.NameToLayer("Road")))
            {
                rCompMove.mGroundNormal = rHitInfo.normal;
                rCompMove.mGroundedY = rHitInfo.point.y;
            }
            else
            {
                rCompMove.mGroundNormal = Vector3.up;
                rCompMove.mGroundedY = 0;
            }

            ApplyExtraRatation(rCompMove);
        }

        private void ApplyExtraRatation(ComponentMove rCompMove)
        {
            float fTurnSpeed = Mathf.Lerp(rCompMove.stationaryTurnSpeed, rCompMove.movingTurnSpeed, rCompMove.mForwardAmount);
            rCompMove.Trans.Rotate(0, rCompMove.mTurnAmount * fTurnSpeed * Time.deltaTime, 0);
        }

        private Vector3 Move_RayForword(ComponentMove rCompMove, Vector3 rMove)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompMove.Trans.position + Vector3.up * 0.2f;
            float rActorRadius = 1.2f;// this.Actor.Hero.Radius + 0.2f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + rMove, Color.red);
            Debug.DrawLine(rActorPos, rActorPos + rCompMove.Trans.forward * rActorRadius, Color.green);
#endif
            if (Physics.Raycast(rActorPos, rCompMove.Trans.forward, out rHitInfo, rActorRadius, 1 << LayerMask.NameToLayer("Wall")))
            {
                rMove = Vector3.ProjectOnPlane(rMove, rHitInfo.normal);
                int k = Vector3.Dot(rCompMove.Trans.forward, rMove) >= 0 ? 1 : -1;
                rMove = rCompMove.Trans.InverseTransformDirection(rMove) * k;
#if UNITY_EDITOR
                Debug.DrawLine(rActorPos, rActorPos + rMove, Color.yellow);
#endif
            }
            return rMove;
        }
    }
}
