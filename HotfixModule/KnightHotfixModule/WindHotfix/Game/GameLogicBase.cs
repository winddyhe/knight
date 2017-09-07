using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Hotfix;

namespace WindHotfix.Game
{
    public class GameLogicBase
    {
        private List<GameComponent> mGameComps;

        public virtual IEnumerator Initialize()
        {
            mGameComps = new List<GameComponent>();
            yield break;
        }

        public virtual void Update()
        {
        }

        public GameComponent AddComponent(HotfixMBContainer rMBContainer)
        {
            GameComponent rGameComp = new GameComponent(rMBContainer);
            mGameComps.Add(rGameComp);
            return rGameComp;
        }
    }
}
