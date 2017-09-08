using UnityEngine;
using System.Collections;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemAnimator : GameSystem
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            if (!this.IsActive) return;

            //ECSManager.Instance.ForeachEntities((rComps) =>
            //{

            //},
            //typeof(ComponentAnimator));
        }
    }
}