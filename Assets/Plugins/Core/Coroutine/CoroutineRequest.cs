//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Core
{
    public class CoroutineRequest<T> : CustomYieldInstruction where T : class
    {
        public CoroutineHandler Handler;
                
        public override bool    keepWaiting
        {
            get
            {
                bool result = (this.Handler == null || (this.Handler != null && !this.Handler.IsRunning && this.Handler.IsCompleted));
                return !result;
            }
        }

        public CoroutineRequest<T> Start(IEnumerator rIEnum)
        {
            this.Handler = CoroutineManager.Instance.StartHandler(rIEnum);
            return this;
        }

        public void Stop()
        {
            CoroutineManager.Instance.Stop(this.Handler);
        }
    }
}