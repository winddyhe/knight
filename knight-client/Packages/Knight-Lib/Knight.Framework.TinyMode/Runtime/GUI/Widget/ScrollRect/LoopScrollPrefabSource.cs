using UnityEngine;
using System.Collections;
using Knight.Core;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public GameObject rootPoolObj;
        public GameObject prefabObj;
        public int poolSize = 5;

        private GameObjectPool objectPool;
        private bool inited = false;

        public virtual GameObject GetObject()
        {
            if(!inited)
            {
                objectPool = new GameObjectPool(rootPoolObj, prefabObj, 0);
                inited = true;
            }
            return objectPool.Alloc();
        }

        public virtual void Free(GameObject rGo)
        {
            if (objectPool == null) return;
            objectPool.Free(rGo);
        }
    }
}
