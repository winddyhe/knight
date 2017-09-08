using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Hotfix;

namespace WindHotfix.Game
{
    public class GameLogicBase
    {
        private List<Component> mGameComps;

        public virtual IEnumerator Initialize()
        {
            mGameComps = new List<Component>();
            yield break;
        }

        public virtual void Update()
        {
        }
    }
}
