//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using Knight.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{
    public class UnityEventWatcher : IDisposable
    {
        private UnityEventBinderBase    mUnityEventBinder;
        private bool                    mIsDisposed;

        public UnityEventWatcher(Component rComp, string rEventName, Action<EventArg> rAction)
        {
            var rBindableEvent = DataBindingTypeResolve.GetBoundEvent(rEventName, rComp);
            this.mUnityEventBinder = UnityEventBinderFactory.Create(rBindableEvent?.UnityEvent, rAction);
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool bIsDisposing)
        {
            if (this.mIsDisposed) return;

            if (bIsDisposing && this.mUnityEventBinder != null)
            {
                this.mUnityEventBinder.Dispose();
                this.mUnityEventBinder = null;
            }
            this.mIsDisposed = true;
        }
    }
}
