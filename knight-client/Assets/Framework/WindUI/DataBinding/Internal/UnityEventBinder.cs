using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using Knight.Core;

namespace UnityEngine.UI
{
    public static class UnityEventBinderFactory
    {
        public static UnityEventBinderBase Create(UnityEventBase rUnityEventBase, Action rAction)
        {
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
        private UnityEvent      mUnityEvent;
        private readonly Action mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action rAction)
        {
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
            UtilTool.SafeExecute(this.mAction);
        }
    }

    public class UnityEventBinder<T0> : UnityEventBinderBase
    {
        private UnityEvent<T0>      mUnityEvent;
        private readonly Action     mAction;
    
        public UnityEventBinder(UnityEventBase rUnityEvent, Action rAction)
        {
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
            UtilTool.SafeExecute(this.mAction);
        }
    }

    public class UnityEventBinder<T0, T1> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1>      mUnityEvent;
        private readonly Action         mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action rAction)
        {
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
            UtilTool.SafeExecute(this.mAction);
        }
    }

    public class UnityEventBinder<T0, T1, T2> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2>      mUnityEvent;
        private readonly Action             mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action rAction)
        {
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
            UtilTool.SafeExecute(this.mAction);
        }
    }

    public class UnityEventBinder<T0, T1, T2, T3> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2, T3>      mUnityEvent;
        private readonly Action                 mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action rAction)
        {
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
            UtilTool.SafeExecute(this.mAction);
        }
    }
}
