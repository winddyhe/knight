//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Hotfix;
using WindHotfix.Core;
using Core;

namespace WindHotfix.Game
{
    public class ECSManager : THotfixSingleton<ECSManager>
    {
        private List<GameEntity>    mEntities;
        private List<GameSystem>    mSystems;

        public List<GameSystem>     Systems     { get { return mSystems;    } }
        public List<GameEntity>     Entities    { get { return mEntities;   } }

        private ECSManager()
        {
            this.mEntities = new List<GameEntity>();
            this.mSystems  = new List<GameSystem>();
        }

        public ECSManager AddSystem(GameSystem rSystem)
        {
            mSystems.Add(rSystem);
            return this;
        }

        public void AddEntity(GameEntity rEntity)
        {
            mEntities.Add(rEntity);
        }

        public void Initialize()
        {
            for (int i = 0; i < mSystems.Count; i++)
            {
                mSystems[i].Start();
            }
        }

        public void Update()
        {
            for (int i = 0; i < mSystems.Count; i++)
            {
                if (mSystems[i].IsActive)
                {
                    mSystems[i].Update();
                }
            }
        }
        
        public void Destroy()
        {
            this.mEntities.Clear();
            this.mSystems.Clear();
        }

        public void ForeachEntities(GCContainer rGCContainer, Action<GCContainer> rExecuteComponents, params Type[] rTypes)
        {
            if (rTypes.Length == 0) return;

            for (int i = 0; i < mEntities.Count; i++)
            {
                var rEntity = mEntities[i];
                rGCContainer.Clear();
                bool bIsIgnore = false;
                for (int k = 0; k < rTypes.Length; k++)
                {
                    var rComp = rEntity.Components.Find((rItem) => { return rItem.GetType().Equals(rTypes[k]); });
                    if (rComp == null)
                    {
                        bIsIgnore = true;
                        break;
                    }
                    rGCContainer.Add(rComp);
                }
                if (!bIsIgnore)
                {
                    UtilTool.SafeExecute(rExecuteComponents, rGCContainer);
                }
            }
        }
    }
}
