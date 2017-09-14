using UnityEngine;
using System.Collections;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemAnimator : TGameSystem<ComponentAnimator>
    {
        protected override void OnUpdate(ComponentAnimator rCompAnimator)
        {
            if (rCompAnimator.IsRun)
            {
                rCompAnimator.Animator.SetBool("IsRun",  rCompAnimator.IsMove);
                rCompAnimator.Animator.SetBool("IsMove", rCompAnimator.IsMove);
            }
            else
            {
                rCompAnimator.Animator.SetBool("IsMove", rCompAnimator.IsMove);
                rCompAnimator.Animator.SetBool("IsRun",  false);
            }
        }
    }
}