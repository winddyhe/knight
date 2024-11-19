using System;
using System.Collections.Generic;

namespace Knight.Core
{
    /// <summary>
    /// 事件系统，一个继承于Event类作为一个事件，使用Type作为事件类型的唯一标识
    /// Event类不同的对象作为同一个事件的不同订阅者，订阅者可以订阅同一个事件的同样的参数
    /// </summary>
    public class EventManager : TSingleton<EventManager>
    {
        public Dictionary<Type, List<IEvent>> TypeEvents;

        private EventManager()
        {
        }

        public void Initialize()
        {
            this.TypeEvents = new Dictionary<Type, List<IEvent>>();
        }

        public void Destroy()
        {
            this.TypeEvents.Clear();
        }

        public void Binding(IEvent rEvent)
        {
            var rEventType = rEvent.GetType();
            if (!this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                rEventList = new List<IEvent>();
                this.TypeEvents.Add(rEventType, rEventList);
            }
            rEventList.Add(rEvent);
        }

        public void Unbinding(IEvent rEvent)
        {
            var rEventType = rEvent.GetType();
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                rEventList.Remove(rEvent);
            }
        }

        public void Distribute(Type rEventType)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event rEvent)
                    {
                        rEvent.Trigger();
                    }
                }
            }
        }

        public void Distribute<T1>(Type rEventType, T1 rArg1)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1> rEvent)
                    {
                        rEvent.Trigger(rArg1);
                    }
                }
            }
        }

        public void Distribute<T1, T2>(Type rEventType, T1 rArg1, T2 rArg2)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5, T6>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5, T6> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5, T6, T7>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5, T6, T7> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5, T6, T7, T8>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5, T6, T7, T8> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8, T9 rArg9)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5, T6, T7, T8, T9> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8, rArg9);
                    }
                }
            }
        }

        public void Distribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Type rEventType, T1 rArg1, T2 rArg2, T3 rArg3, T4 rArg4, T5 rArg5, T6 rArg6, T7 rArg7, T8 rArg8, T9 rArg9, T10 rArg10)
        {
            if (this.TypeEvents.TryGetValue(rEventType, out var rEventList))
            {
                foreach (var rEventItem in rEventList)
                {
                    if (rEventItem is Event<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> rEvent)
                    {
                        rEvent.Trigger(rArg1, rArg2, rArg3, rArg4, rArg5, rArg6, rArg7, rArg8, rArg9, rArg10);
                    }
                }
            }
        }
    }
}
