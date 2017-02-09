//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;

namespace Core
{
    /// <summary>
    /// 协程管理器
    /// </summary>
    public class CoroutineManager : TSingleton<CoroutineManager>
    {
        private GameObject mCoroutineRootObj;

        private CoroutineManager() { }

        public void Initialize()
        {
            this.mCoroutineRootObj = new GameObject("___CoroutineRoot");
            this.mCoroutineRootObj.transform.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(this.mCoroutineRootObj);
        }

        public CoroutineWrapper StartWrapper(IEnumerator rIEnum)
        {
            var rCourtineObj = UtilTool.CreateGameObject(this.mCoroutineRootObj, "coroutine");
            CoroutineHandler rHandler = rCourtineObj.ReceiveComponent<CoroutineHandler>();
            return rHandler.StartHandler(rIEnum);
        }

        public Coroutine Start(IEnumerator rIEnum)
        {
            return this.StartWrapper(rIEnum).Coroutine;
        }

        public void Stop(CoroutineWrapper rCoroutineWrapper)
        {
            if (rCoroutineWrapper != null)
            {
                if (rCoroutineWrapper.Handler != null)
                {
                    rCoroutineWrapper.Handler.StopAllCoroutines();
                    GameObject.DestroyImmediate(rCoroutineWrapper.Handler.gameObject);
                }
                rCoroutineWrapper.Coroutine = null;
                rCoroutineWrapper.Handler = null;
            }
        }
    }
}

