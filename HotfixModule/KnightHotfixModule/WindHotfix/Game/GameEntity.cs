//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace WindHotfix.Game
{
    public class GameEntity
    {
        protected List<GameComponent> mComponents;
        public    List<GameComponent> Components  { get { return mComponents; } }

        public GameEntity()
        {
            mComponents = new List<GameComponent>();
        }

        public void AddComponent(GameComponent rComp)
        {
            mComponents.Add(rComp);
        }
        
        public T AddComponent<T>() where T : GameComponent, new()
        {
            var rComp = GameEntity.Create<T>(this);
            mComponents.Add(rComp);
            return rComp;
        }

        public T GetComponent<T>() where T : GameComponent
        {
            var rComp = mComponents.Find((rItem) => { return rItem.GetType().Equals(typeof(T)); });
            if (rComp != null) return rComp as T;
            return null;
        }

        public List<T> GetComponents<T>() where T : GameComponent
        {
            List<T> rComps = new List<T>();
            for (int i = 0; i < mComponents.Count; i++)
            {
                if (mComponents[i].GetType().Equals(typeof(T)))
                    rComps.Add(mComponents[i] as T);
            }
            return rComps;
        }
        
        public T ReceiveComponent<T>() where T : GameComponent, new()
        {
            var rComp = this.GetComponent<T>();
            if (rComp == null) rComp = this.AddComponent<T>();
            return rComp;
        }

        public static T Create<T>(GameEntity rEntity) where T : GameComponent, new()
        {
            T rComp = new T();
            rComp.GUID = System.Guid.NewGuid().ToString();
            rComp.Entity = rEntity;
            return rComp;
        }
    }
}
