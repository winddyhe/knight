//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixModule.Hotfix
{
    public class HotfixEventHandler
    {
        public class EventObject
        {
            public UnityEngine.Object               Target;
            public event Action<UnityEngine.Object> Event;
            public int                              ReferCount;

            public EventObject(UnityEngine.Object rTarget)
            {
                this.Target = rTarget;
            }

            public void Dispose()
            {
                this.Event = null;
                this.Target = null;
                this.ReferCount = 0;
            }

            public void AddEvent(Action<UnityEngine.Object> rAction)
            {
                this.Event += rAction;
                this.ReferCount++;
            }

            public void RemoveEvent(Action<UnityEngine.Object> rAction)
            {
                this.Event -= rAction;
                this.ReferCount--;

                if (this.ReferCount < 0)
                    this.ReferCount = 0;
            }

            public void Handle()
            {
                if (this.Event != null)
                    this.Event(this.Target);
            }
        }

        private Dict<UnityEngine.Object, EventObject> mEvents;
        public  Dict<UnityEngine.Object, EventObject> Events { get { return mEvents; } }

        public HotfixEventHandler(List<UnityEngine.Object> mObjs)
        {
            mEvents = new Dict<UnityEngine.Object, EventObject>();
        }

        public void AddEventListener(UnityEngine.Object rObj, Action<UnityEngine.Object> rAction)
        {
            EventObject rEventObject = null;
            if (!this.mEvents.TryGetValue(rObj, out rEventObject))
            {
                rEventObject = new EventObject(rObj);
                rEventObject.AddEvent(rAction);
                mEvents.Add(rObj, rEventObject);
            }
            else
            {
                rEventObject.AddEvent(rAction);
            }
        }

        public void RemoveEventListener(UnityEngine.Object rObj, Action<UnityEngine.Object> rAction)
        {
            EventObject rEventObject = null;
            if (this.mEvents.TryGetValue(rObj, out rEventObject))
            {
                rEventObject.RemoveEvent(rAction);
            }
            if (rEventObject.ReferCount == 0)
            {
                rEventObject.Dispose();
                this.mEvents.Remove(rObj);
            }
        }

        public void RemoveOne(UnityEngine.Object rObj)
        {
            EventObject rEventObject = null;
            if (this.mEvents.TryGetValue(rObj, out rEventObject))
            {
                rEventObject.Dispose();
                this.mEvents.Remove(rObj);
            }
        }

        public void RemoveAll()
        {
            foreach (var rItem in mEvents)
            {
                rItem.Value.Dispose();
            }
            mEvents.Clear();
        }

        public void Handle(UnityEngine.Object rTarget)
        {
            EventObject rEventObject = null;
            if (this.mEvents.TryGetValue(rTarget, out rEventObject))
            {
                rEventObject.Handle();
            }
        }
    }
}
