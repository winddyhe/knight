using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WindHotfix.Game
{
    public class GameSystem
    {
        public    bool        IsActive;

        protected GCContainer mResultComps;

        public virtual void Initialize()
        {
            this.IsActive = true;
            this.mResultComps = new GCContainer();
        }

        public virtual void Update()
        {
        }
    }
}