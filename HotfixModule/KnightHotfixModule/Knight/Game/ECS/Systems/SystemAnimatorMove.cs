using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemAnimatorMove : TGameSystem<ComponentMove, ComponentAnimator>
    {
        protected override void OnUpdate(ComponentMove rCompMove, ComponentAnimator rCompAnim)
        {
            rCompAnim.IsMove = !rCompMove.MoveSpeed.Equals(Vector3.zero);

            rCompMove.SpeedRate = rCompAnim.IsRun ? 2.0f : 1.0f;
        }
    }
}
