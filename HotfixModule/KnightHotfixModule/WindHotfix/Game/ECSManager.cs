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

        public void AddSystem(GameSystem rSystem)
        {
            mSystems.Add(rSystem);
        }

        public void AddEntity(GameEntity rEntity)
        {
            mEntities.Add(rEntity);
        }

        public void InitializeSystems()
        {
            for (int i = 0; i < mSystems.Count; i++)
            {
                mSystems[i].Initialize();
            }
        }

        public void Update()
        {
            for (int i = 0; i < mSystems.Count; i++)
            {
                if (mSystems[i].IsActive)
                    mSystems[i].Update();
            }
        }

        public void ForeachEntities(GCContainer rGCContainer, Action<GCContainer> rExecuteComponents, params Type[] rTypes)
        {
            rGCContainer.Clear();
            for (int i = 0; i < mEntities.Count; i++)
            {
                var rEntity = mEntities[i];
                for (int k = 0; k < rTypes.Length; k++)
                {
                    var rComp = rEntity.Components.Find((rItem) => { return rItem.GetType().Equals(rTypes[k]); });
                    rGCContainer.Add(rComp);
                }
            }
            UtilTool.SafeExecute(rExecuteComponents, rGCContainer);
        }
    }
}
