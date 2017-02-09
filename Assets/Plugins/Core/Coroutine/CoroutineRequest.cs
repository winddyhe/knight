//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class BaseCoroutineRequest<T> where T : class
    {
        private CoroutineWrapper mCoroutineWrapper;

        public Coroutine Coroutine
        {
            get { return mCoroutineWrapper != null ? mCoroutineWrapper.Coroutine : null; }
        }

        public void Start(IEnumerator rIEnum)
        {
            mCoroutineWrapper = CoroutineManager.Instance.StartWrapper(rIEnum);
        }

        public void Stop()
        {
            if (mCoroutineWrapper != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineWrapper);
            }
        }
    }
}