using System;
using System.Collections.Generic;

namespace WindHotfix.GamePlay
{
    public class Entity
    {
        private List<Component> mComponents;

        public Entity()
        {
            this.mComponents = new List<Component>();
        }

        public void AddComponent(Component rComp)
        {
            this.mComponents.Add(rComp);
        }

        public void RemoveComponent(Component rComp)
        {
            this.mComponents.Remove(rComp);
        }

        public List<Component> Components
        {
            get { return mComponents; }
        }
    }
}