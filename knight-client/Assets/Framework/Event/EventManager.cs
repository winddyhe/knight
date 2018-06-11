//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Knight.Core;

namespace Knight.Framework
{
    public class EventArg
    {
        public List<object>                 Args;

        public EventArg(params object[] rArgs)
        {
            this.Args = new List<object>(rArgs);
        }
        
        public T Get<T>(int nIndex)
        {
            if (this.Args == null) return default(T);
            if (nIndex < 0 || nIndex >= this.Args.Count) return default(T);
            return (T)this.Args[nIndex];
        }
    }

    public class EventManager : TSingleton<EventManager>
    {
        public class Event
        {
            public int                      MsgCode;
            public List<Action<EventArg>>   Callbacks;
        }

        public Dict<int, Event>             mEvents;

        private EventManager()
        {
        }

        public void Initialize()
        {
            this.mEvents = new Dict<int, Event>();
        }

        public void Binding(int nMsgCode, Action<EventArg> rEventCallback)
        {
            Event rEvent = null;
            if (this.mEvents.TryGetValue(nMsgCode, out rEvent))
            {
                if (rEvent.Callbacks == null)
                {
                    rEvent.Callbacks = new List<Action<EventArg>>();
                }
                else
                {
                    if (!rEvent.Callbacks.Contains(rEventCallback))
                    {
                        rEvent.Callbacks.Add(rEventCallback);
                    }
                }
            }
            else
            {
                rEvent = new Event() { MsgCode = nMsgCode, Callbacks = new List<Action<EventArg>>() { rEventCallback } };
                this.mEvents.Add(nMsgCode, rEvent);
            }
        }

        public void Unbinding(int nMsgCode, Action<EventArg> rEventCallback)
        {
            Event rEvent = null;
            if (this.mEvents.TryGetValue(nMsgCode, out rEvent))
            {
                if (rEvent.Callbacks != null)
                    rEvent.Callbacks.Remove(rEventCallback);
            }
        }

        public void Distribute(int nMsgCode, params object[] rEventArgs)
        {
            EventArg rEventArg = new EventArg() { Args = new List<object>(rEventArgs) };
            this.DistributeArg(nMsgCode, rEventArg);
        }

        public void DistributeArg(int nMsgCode, EventArg rEventArg)
        {
            Event rEvent = null;
            if (this.mEvents.TryGetValue(nMsgCode, out rEvent))
            {
                if (rEvent.Callbacks != null)
                {
                    for (int i = 0; i < rEvent.Callbacks.Count; i++)
                    {
                        UtilTool.SafeExecute(rEvent.Callbacks[i], rEventArg);
                    }
                }
            }
        }
    }
}
