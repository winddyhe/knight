using System;
using System.Collections.Generic;

namespace WindHotfix.GamePlay
{
    public class Module
    {
        private List<Entity>    mEntities;
        private List<SubSystem> mSubSystems;

        public Module()
        {
            mEntities = new List<Entity>();
            mSubSystems = new List<SubSystem>();
        }

        public void AddEntity(Entity rEntity)
        {
            mEntities.Add(rEntity);
        }

        public void AddSystem(SubSystem rSystem)
        {
            mSubSystems.Add(rSystem);
        }
    }
}
