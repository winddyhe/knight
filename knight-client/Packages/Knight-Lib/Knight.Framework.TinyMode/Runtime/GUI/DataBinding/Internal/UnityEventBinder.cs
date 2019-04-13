//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using Knight.Core;
using Knight.Framework;

namespace Knight.Framework.TinyMode.UI
{
    public static class UnityEventBinderFactory
    {
        public static UnityEventBinderBase Create(UnityEventBase rUnityEventBase, Action<EventArg> rAction)
        {
            if (rUnityEventBase == null)
                return null;

            var eventArgumentTypes = rUnityEventBase.GetType().BaseType.GetGenericArguments();

            if (!eventArgumentTypes.Any())
            {
                return new UnityEventBinder(rUnityEventBase, rAction);
            }

            try
            {
                var genericType = typeof(UnityEventBinder<>).MakeGenericType(eventArgumentTypes);
                return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
            }
            catch (ArgumentException ex)
            {
                throw new Exception("Cannot bind event with more than 5 arguments", ex);
            }
        }
    }

    public abstract class UnityEventBinderBase : IDisposable
    {
        public abstract void Dispose();
    }

    public class UnityEventBinder : UnityEventBinderBase
    {
        private UnityEvent                  mUnityEvent;
        private readonly Action<EventArg>   mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<EventArg> rAction)
        {
            if (rUnityEvent == null) return;
            this.mUnityEvent = (UnityEvent)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler()
        {
            UtilTool.SafeExecute(this.mAction, new EventArg());
        }
    }

    public class UnityEventBinder<T0> : UnityEventBinderBase
    {
        private UnityEvent<T0>              mUnityEvent;
        private readonly Action<EventArg>   mAction;
    
        public UnityEventBinder(UnityEventBase rUnityEvent, Action<EventArg> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0>)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0)
        {
            UtilTool.SafeExecute(this.mAction, new EventArg(rArg0));
        }
    }

    public class UnityEventBinder<T0, T1> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1>          mUnityEvent;
        private readonly Action<EventArg>   mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<EventArg> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1>)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1)
        {
            UtilTool.SafeExecute(this.mAction, new EventArg(rArg0, rArg1));
        }
    }

    public class UnityEventBinder<T0, T1, T2> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2>      mUnityEvent;
        private readonly Action<EventArg>   mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<EventArg> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1, T2>)rUnityEvent;
            this.mAction = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1, T2 rArg2)
        {
            UtilTool.SafeExecute(this.mAction, new EventArg(rArg0, rArg1, rArg2));
        }
    }

    public class UnityEventBinder<T0, T1, T2, T3> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2, T3>      mUnityEvent;
        private readonly Action<EventArg>       mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<EventArg> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1, T2, T3>)rUnityEvent;
            this.mAction = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1, T2 rArg2, T3 rArg3)
        {
            UtilTool.SafeExecute(this.mAction, new EventArg(rArg0, rArg1, rArg2, rArg3));
        }
    }
}
