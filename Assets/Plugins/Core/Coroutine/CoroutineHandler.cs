//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class CoroutineWrapper
    {
        public Coroutine Coroutine;
        public CoroutineHandler Handler;

        public bool IsCompleted;
        public bool IsRunning;
    }

    public class CoroutineHandler : MonoBehaviour
    {
        public CoroutineWrapper StartHandler(IEnumerator rIEnum)
        {
            CoroutineWrapper rCoroutineWrapper = new CoroutineWrapper();
            rCoroutineWrapper.IsCompleted = false;
            rCoroutineWrapper.IsRunning = true;
            rCoroutineWrapper.Coroutine = this.StartCoroutine(StartHandler(rCoroutineWrapper, rIEnum));
            rCoroutineWrapper.Handler = this;
            return rCoroutineWrapper;
        }

        private IEnumerator StartHandler(CoroutineWrapper rCoroutineWrapper, IEnumerator rIEnum)
        {
            yield return this.StartCoroutine(rIEnum);
            rCoroutineWrapper.IsRunning = false;
            rCoroutineWrapper.IsCompleted = true;
            // @TODO: 可能会出问题的地方，自己把自己删掉，并将对应的数据都清空
            CoroutineManager.Instance.Stop(rCoroutineWrapper);
        }
    }
}
