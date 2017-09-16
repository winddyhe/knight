using System;
using System.Collections.Generic;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemTransform : TGameSystem<ComponentTransform, ComponentUnityGo>
    {
        protected override void OnUpdate(ComponentTransform rCompTrans, ComponentUnityGo rCompGo)
        {
            rCompGo.GameObject.transform.position   = rCompTrans.Position;
            rCompGo.GameObject.transform.localScale = rCompTrans.Scale;
            rCompGo.GameObject.transform.forward    = rCompTrans.Forward;
        }
    }

    public class SystemAnimator : TGameSystem<ComponentAnimator, ComponentUnityAnimator>
    {
        protected override void OnUpdate(ComponentAnimator rCompAnim, ComponentUnityAnimator rCompUnityAnim)
        {
            if (rCompAnim.IsRun)
            {
                rCompUnityAnim.Animator.SetBool("IsRun", rCompAnim.IsRun);
                rCompUnityAnim.Animator.SetBool("IsMove", rCompAnim.IsMove);
            }
            else
            {
                rCompUnityAnim.Animator.SetBool("IsRun", rCompAnim.IsRun);
                rCompUnityAnim.Animator.SetBool("IsMove", rCompAnim.IsMove);
            }
        }
    }
}
