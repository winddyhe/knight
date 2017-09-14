using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    /// <summary>
    /// Move 系统依赖 
    ///     CompMove和Collider组件
    /// </summary>
    public class SystemMove : TGameSystem<ComponentMove, ComponentCollider>
    {
        protected override void OnUpdate(ComponentMove rCompMove, ComponentCollider rCompCollider)
        {
            if (rCompMove.MoveSpeed.magnitude > 1.0f) rCompMove.MoveSpeed.Normalize();
            
            // 和地面的碰撞检测
            this.CheckGroundStatus(rCompMove);
            
            rCompMove.MoveSpeed = Vector3.ProjectOnPlane(rCompMove.MoveSpeed, rCompMove.GroundNormal);
            rCompMove.TurnAmount = Mathf.Atan2(rCompMove.MoveSpeed.x, rCompMove.MoveSpeed.z);
            rCompMove.ForwardAmount = rCompMove.MoveSpeed.z;

            // 和墙体的碰撞检测
            Move_RayForword(rCompCollider, rCompMove);

            // 设置位置的高度
            rCompMove.Position = new Vector3(rCompMove.Position.x, rCompMove.GroundedY, rCompMove.Position.z);
        }

        private void CheckGroundStatus(ComponentMove rCompMove)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompMove.Position + Vector3.up * 1.0f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + Vector3.down * rCompMove.GroundCheckDistance);
#endif
            if (Physics.Raycast(rActorPos, Vector3.down, out rHitInfo, rCompMove.GroundCheckDistance, 1 << LayerMask.NameToLayer("Road")))
            {
                rCompMove.GroundNormal = rHitInfo.normal;
                rCompMove.GroundedY = rHitInfo.point.y;
            }
            else
            {
                rCompMove.GroundNormal = Vector3.up;
                rCompMove.GroundedY = 0;
            }
            rCompMove.TurnSpeed = Mathf.Lerp(rCompMove.StationaryTurnSpeed, rCompMove.MovingTurnSpeed, rCompMove.ForwardAmount);
        }

        private void Move_RayForword(ComponentCollider rCompCollider, ComponentMove rCompMove)
        {
            RaycastHit rHitInfo;
            Vector3 rActorPos = rCompMove.Position + Vector3.up * 0.2f;
            float rActorRadius = rCompCollider.Radius + 0.2f;
#if UNITY_EDITOR
            Debug.DrawLine(rActorPos, rActorPos + rCompMove.MoveSpeed, Color.red);
            Debug.DrawLine(rActorPos, rActorPos + rCompMove.Forward * rActorRadius, Color.green);
#endif
            if (Physics.Raycast(rActorPos, rCompMove.Forward, out rHitInfo, rActorRadius, 1 << LayerMask.NameToLayer("Wall")))
            {
                rCompMove.MoveSpeed = Vector3.ProjectOnPlane(rCompMove.MoveSpeed, rHitInfo.normal);
                int k = Vector3.Dot(rCompMove.Forward, rCompMove.MoveSpeed) >= 0 ? 1 : -1;
#if UNITY_EDITOR
                Debug.DrawLine(rActorPos, rActorPos + rCompMove.MoveSpeed, Color.yellow);
#endif
            }
        }
    }
}
