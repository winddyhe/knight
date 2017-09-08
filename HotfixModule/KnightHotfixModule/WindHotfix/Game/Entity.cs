using System;
using System.Collections.Generic;

namespace WindHotfix.Game
{
    public class Entity
    {
        private List<Component> mComponents;

        public Entity()
        {
            mComponents = new List<Component>();
        }

        public void Update()
        {
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var rComp = Entity.Create<T>(this);
            mComponents.Add(rComp);
            return rComp;
        }

        public T GetComponent<T>() where T : Component
        {
            var rComp = mComponents.Find((rItem) => { return rItem.GetType().Equals(typeof(T)); });
            if (rComp != null) return rComp as T;
            return null;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            List<T> rComps = new List<T>();
            for (int i = 0; i < mComponents.Count; i++)
            {
                if (mComponents[i].GetType().Equals(typeof(T)))
                    rComps.Add(mComponents[i] as T);
            }
            return rComps;
        }
        
        public T ReceiveComponent<T>() where T : Component, new()
        {
            var rComp = this.GetComponent<T>();
            if (rComp == null) rComp = this.AddComponent<T>();
            return rComp;
        }

        public static T Create<T>(Entity rEntity) where T : Component, new()
        {
            T rComp = new T();
            rComp.GUID = System.Guid.NewGuid().ToString();
            rComp.Entity = rEntity;
            return rComp;
        }
    }
}
