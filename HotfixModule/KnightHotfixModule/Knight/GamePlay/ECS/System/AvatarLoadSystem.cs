using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using WindHotfix.GamePlay;

namespace Game.Knight
{
    public class AvatarLoadSystem : GameSystem
    {
        public override IEnumerator ExecuteComponents_Async(List<GameComponent> rComps)
        {
            for (int i = 0; i < rComps.Count; i++)
            {
                AvatarComponent rAvatarComp = rComps[i] as AvatarComponent;
                if (rAvatarComp != null)
                {
                    yield return LoadAvatar_Async(rAvatarComp);
                }
            }
        }

        private IEnumerator LoadAvatar_Async(AvatarComponent rAvatarComp)
        {
            var rAvatarRequest = AvatarAssetLoader.Instance.Load(rAvatarComp.AssetPath, rAvatarComp.AssetName);
            yield return rAvatarRequest;
            rAvatarComp.AvatarGo = rAvatarRequest.AvatarGo;
        }
    }
}
