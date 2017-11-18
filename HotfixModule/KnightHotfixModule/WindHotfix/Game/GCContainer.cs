//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Core;

namespace WindHotfix.Game
{
    public class GCContainer
    {
        public List<GameComponent>  mResultComps;

        public GCContainer()
        {
            mResultComps = new List<GameComponent>();
        }

        public void Add(GameComponent rComp)
        {
            this.mResultComps.Add(rComp);
        }

        public void Clear()
        {
            this.mResultComps.Clear();
        }

        public T Get<T>() where T : GameComponent
        {
            if (mResultComps == null) return null;
            return mResultComps.Find((rItem) => { return rItem.GetType().Equals(typeof(T)); }) as T;
        }

        public T Get<T>(int nIndex) where T : GameComponent
        {
            if (mResultComps == null) return null;
            if (nIndex < 0 || nIndex >= mResultComps.Count) return null;
            return mResultComps[nIndex] as T;
        }

        public GameComponent this[int nIndex]
        {
            get
            {
                if (mResultComps == null) return null;
                if (nIndex < 0 || nIndex >= mResultComps.Count) return null;
                return mResultComps[nIndex];
            }
        }
    }
}
