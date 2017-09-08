using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Hotfix;

namespace WindHotfix.Game
{
    public class LogicSystemManager
    {
        protected static LogicSystemManager __instance;
        public    static LogicSystemManager Instance    { get { return __instance; } }

        private List<Entity>                mEntities;
        private List<LogicSystem>           mLogicSystems;

        public virtual IEnumerator Initialize()
        {
            __instance = this;

            mEntities = new List<Entity>();
            mLogicSystems = new List<LogicSystem>();
            yield break;
        }

        public virtual void Update()
        {
            if (mLogicSystems != null)
            {
                for (int i = 0; i < mLogicSystems.Count; i++)
                {
                    mLogicSystems[i].Update();
                }
            }
        }
    }
}
