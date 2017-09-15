//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    /// <summary>
    /// 内存对象池，只针对GameObject
    /// </summary>
    public class GameObjectPool
    {
        /// <summary>
        /// 缓存，这个和rootGo节点下的对象个数一一对应
        /// </summary>
        private TObjectPool<GameObject> mObjectPool;
        /// <summary>
        /// 模板对象
        /// </summary>
        private GameObject              mPrefabGo;
        /// <summary>
        /// 对象池的根节点
        /// </summary>
        private GameObject              mRootGo;
        /// <summary>
        /// 对象池的根节点
        /// </summary>
        public  GameObject              RootGo          { get { return mRootGo; } }

        public GameObjectPool(string rPoolName, GameObject rPrefabGo, int rInitCount = 0)
        {
            this.mPrefabGo = rPrefabGo;
            this.mObjectPool = new TObjectPool<GameObject>(OnAlloc, OnFree, OnDestroy);

            this.mRootGo = UtilTool.CreateGameObject(rPoolName);
            this.mRootGo.SetActive(false);
            this.mRootGo.transform.position = new Vector3(0, 1000, 0);
            
            for (int i = 0; i < rInitCount; i++)
            {
                this.mObjectPool.Alloc();
            }
        }

        public GameObject Alloc()
        {
            return this.mObjectPool.Alloc();
        }

        public void Free(GameObject rGo)
        {
            if (rGo == null) return;
            this.mObjectPool.Free(rGo);
        }

        public void Destroy()
        {
            this.mPrefabGo = null;
            this.mObjectPool.Destroy();
            UtilTool.SafeDestroy(this.mRootGo);
        }

        private GameObject OnAlloc()
        {
            return GameObject.Instantiate(this.mPrefabGo);
        }

        private void OnFree(GameObject rGo)
        {
            rGo.transform.parent = this.mRootGo.transform;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;
        }

        private void OnDestroy(GameObject rGo)
        {
            UtilTool.SafeDestroy(rGo);
        }
    }
}