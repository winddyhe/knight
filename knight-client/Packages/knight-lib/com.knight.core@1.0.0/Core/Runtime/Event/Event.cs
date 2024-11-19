using System;

namespace Knight.Core
{
    public interface IEvent
    {
    }

    public class Event : IEvent
    {
        protected Action mTriggerCallback;

        public Event(Action rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger()
        {
            this.mTriggerCallback?.Invoke();
        }
    }

    public class Event<T1> : IEvent
    {
        protected Action<T1> mTriggerCallback;

        public Event(Action<T1> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1)
        {
            this.mTriggerCallback?.Invoke(rArg1);
        }
    }

    public class Event<T1, T2> : IEvent
    {
        protected Action<T1, T2> mTriggerCallback;

        public Event( Action<T1, T2> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2);
        }
    }

    public class Event<T1, T2, T3> : IEvent
    {
        protected Action<T1, T2, T3> mTriggerCallback;

        public Event(Action<T1, T2, T3> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3);
        }
    }

    public class Event<T1, T2, T3, T4> : IEvent
    {
        protected Action<T1, T2, T3, T4> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4);
        }
    }

    public class Event<T1, T2, T3, T4, T5> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5);
        }
    }

    public class Event<T1, T2, T3, T4, T5, T6> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5, T6> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5, T6> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6);
        }
    }

    public class Event<T1, T2, T3, T4, T5, T6, T7> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5, T6, T7> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5, T6, T7> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7);
        }
    }

    public class Event<T1, T2, T3, T4, T5, T6, T7, T8> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5, T6, T7, T8> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5, T6, T7, T8> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8);
        }
    }

    public class Event<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8, T9 rArg9)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8, rArg9);
        }
    }

    public class Event<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IEvent
    {
        protected Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> mTriggerCallback;

        public Event(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> rTriggerCallback)
        {
            this.mTriggerCallback = rTriggerCallback;
        }

        public void Trigger(T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8, T9 rArg9, T10 rArg10)
        {
            this.mTriggerCallback?.Invoke(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8, rArg9, rArg10);
        }
    }
}
