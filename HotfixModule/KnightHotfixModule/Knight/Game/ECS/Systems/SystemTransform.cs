using System;
using System.Collections.Generic;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemTransform : TGameSystem<ComponentTransform, ComponentUnityTrans>
    {
        protected override void OnUpdate(ComponentTransform rCompTrans, ComponentUnityTrans rCompGo)
        {
            rCompGo.Transform.position   = rCompTrans.Position;
            rCompGo.Transform.rotation   = rCompTrans.Rotation;
            rCompGo.Transform.localScale = rCompTrans.Scale;

            rCompGo.Transform.forward    = rCompTrans.Forward;
            rCompGo.Transform.right      = rCompTrans.Right;
        }
    }
}
