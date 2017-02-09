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
        private Stack<GameObject>   objectsCache;
        /// <summary>
        /// 对象池的根节点
        /// </summary>
        private GameObject          rootGo;
        /// <summary>
        /// 对象池的模板对象，通常是从资源包取出来的预制件对象
        /// </summary>
        private GameObject          templateGo;

        public GameObjectPool(string rPoolName, GameObject rTemplateObj, int rInitCount = 0)
        {
            this.objectsCache = new Stack<GameObject>();

            this.rootGo = UtilTool.CreateGameObject(rPoolName);
            this.rootGo.SetActive(false);
            this.rootGo.transform.position = new Vector3(0, 1000, 0);

            this.templateGo = rTemplateObj;

            for (int i = 0; i < rInitCount; i++)
            {
                GameObject rGo = UtilTool.CreateGameObject(this.templateGo, this.rootGo);
                this.objectsCache.Push(rGo);
            }
        }

        public GameObject Alloc()
        {
            if (this.objectsCache.Count == 0)
            {
                return UtilTool.CreateGameObject(this.templateGo);
            }
            GameObject rGo = this.objectsCache.Pop();
            rGo.transform.parent = null;
            return rGo;
        }

        public void Free(GameObject rGo)
        {
            if (rGo == null) return;

            rGo.transform.parent = this.rootGo.transform;
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localRotation = Quaternion.identity;
            rGo.transform.localScale = Vector3.one;

            this.objectsCache.Push(rGo);
        }

        public void Destroy()
        {
            UtilTool.SafeDestroy(this.templateGo);
            UtilTool.SafeDestroy(this.rootGo);
            this.objectsCache.Clear();
        }
    }
}