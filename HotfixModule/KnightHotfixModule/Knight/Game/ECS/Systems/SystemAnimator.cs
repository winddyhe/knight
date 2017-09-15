using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemAnimator : TGameSystem<ComponentAnimator, ComponentUnityAnimator>
    {
        protected override void OnUpdate(ComponentAnimator rCompAnim, ComponentUnityAnimator rCompUnityAnim)
        {
            if (rCompAnim.IsRun)
            {
                rCompUnityAnim.Animator.SetBool("IsRun",  rCompAnim.IsRun);
                rCompUnityAnim.Animator.SetBool("IsWalk", rCompAnim.IsMove);
            }
            else
            {
                rCompUnityAnim.Animator.SetBool("IsRun",  rCompAnim.IsRun);
                rCompUnityAnim.Animator.SetBool("IsWalk", rCompAnim.IsMove);
            }
        }
    }

    public class SystemAnimatorMove : TGameSystem<ComponentMove, ComponentAnimator>
    {
        protected override void OnUpdate(ComponentMove rCompMove, ComponentAnimator rCompAnim)
        {
            rCompAnim.IsMove = !rCompMove.MoveSpeed.Equals(Vector3.zero);

            rCompMove.SpeedRate = rCompAnim.IsRun ? 2.0f : 1.0f;
        }
    }
}
