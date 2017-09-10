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
                this.Move(this.mResultComps.Get<ComponentMove>(), 
                          this.mResultComps.Get<ComponentAnimator>(),
                          this.mResultComps.Get<ComponentCollider>(),
                          this.mResultComps.Get<ComponentTransform>());
            },
            typeof(ComponentMove),
            typeof(ComponentAnimator),
            typeof(ComponentCollider),
            typeof(ComponentTransform));
        }

        private void Move(ComponentMove rCompMove, ComponentAnimator rCompAnim, ComponentCollider rCompCollider, ComponentTransform rCompTrans)
        {
            if (rCompMove.MoveSpeed.magnitude > 1.0f) rCompMove.MoveSpeed.Normalize();

            rCompMove.MoveSpeed = rCompTrans.Transform.InverseTransformDirection(rCompMove.MoveSpeed);
            CheckGroundStatus(rCompMove, rCompAnim, rCompTrans);
            rCompMove.MoveSpeed = Vector3.ProjectOnPlane(rCompMove.MoveSpeed, rCompMove.mGroundNormal);
            rCompMove.mTurnAmount = Mathf.Atan2(rCompMove.MoveSpeed.x, rCompMove.MoveSpeed.z);
            rCompMove.mForwardAmount = rCompMove.MoveSpeed.z;

            // 和墙体的碰撞检测
            rCompMove.MoveSpeed = Move_RayForword(rCompMove, rCompMove.MoveSpeed, rCompCollider, rCompTrans);

            bool bIsMove = !rCompMove.MoveSpeed.Equals(Vector3.zero);

            rCompTrans.Transform.Translate(rCompMove.MoveSpeed * 0.075f * (rCompAnim.IsRun ? 2 : 1));
            rCompTrans.Transform.position = new Vector3(rCompTrans.Transform.position.x, rCompMove.mGroundedY, rCompTrans.Transform.position.z);
        }

        private void CheckGroundStatus(ComponentMove rCompMove, ComponentAnimator rCompAnim, ComponentTransform rCompTrans)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompTrans.Transform.position + Vector3.up * 1.0f;
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

            ApplyExtraRatation(rCompMove, rCompTrans);
        }

        private void ApplyExtraRatation(ComponentMove rCompMove, ComponentTransform rCompTrans)
        {
            float fTurnSpeed = Mathf.Lerp(rCompMove.stationaryTurnSpeed, rCompMove.movingTurnSpeed, rCompMove.mForwardAmount);
            rCompTrans.Transform.Rotate(0, rCompMove.mTurnAmount * fTurnSpeed * Time.deltaTime, 0);
        }

        private Vector3 Move_RayForword(ComponentMove rCompMove, Vector3 rMove, ComponentCollider rCompCollider, ComponentTransform rCompTrans)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompTrans.Transform.position + Vector3.up * 0.2f;
            float rActorRadius = rCompCollider.Radius + 0.2f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + rMove, Color.red);
            Debug.DrawLine(rActorPos, rActorPos + rCompMove.Trans.forward * rActorRadius, Color.green);
#endif
            if (Physics.Raycast(rActorPos, rCompTrans.Transform.forward, out rHitInfo, rActorRadius, 1 << LayerMask.NameToLayer("Wall")))
            {
                rMove = Vector3.ProjectOnPlane(rMove, rHitInfo.normal);
                int k = Vector3.Dot(rCompTrans.Transform.forward, rMove) >= 0 ? 1 : -1;
                rMove = rCompTrans.Transform.InverseTransformDirection(rMove) * k;
#if UNITY_EDITOR
                Debug.DrawLine(rActorPos, rActorPos + rMove, Color.yellow);
#endif
            }
            return rMove;
        }
    }
}
