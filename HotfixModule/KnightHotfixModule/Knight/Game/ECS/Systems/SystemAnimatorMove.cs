using System;
using System.Collections.Generic;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemAnimatorMove : TGameSystem<ComponentMove, ComponentAnimator>
    {
        protected override void OnUpdate(ComponentMove rCompMove, ComponentAnimator rCompAnim)
        {
            rCompMove.SpeedRate = rCompAnim.IsRun ? 2.0f : 1.0f;
        }
    }
}
