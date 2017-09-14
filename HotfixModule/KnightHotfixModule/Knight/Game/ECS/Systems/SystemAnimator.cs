using System;
using System.Collections.Generic;
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
}
