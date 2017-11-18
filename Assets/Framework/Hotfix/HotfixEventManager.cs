//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Hotfix
{
    public class HotfixEventManager : TSingleton<HotfixEventManager>
    {
        public class EventObject
        {
            public UnityEngine.Object               Target;
            public EventTriggerType                 Type;
            public event Action<UnityEngine.Object> Event;
            public int                              ReferCount;

            public EventObject(UnityEngine.Object rTarget, EventTriggerType rType)
            {
                this.Target = rTarget;
                this.Type = rType;
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
        
        public class EventTypeObject
        {
            public UnityEngine.Object                   TargetGo;
            public Dict<EventTriggerType, EventObject>  EventTypeObjs;

            public EventTypeObject(UnityEngine.Object rTargetGo)
            {
                this.TargetGo = rTargetGo;
                this.EventTypeObjs = new Dict<EventTriggerType, EventObject>();
            }

            public void AddEvent(EventTriggerType rEventType, System.Action<UnityEngine.Object> rEventHandler)
            {
                EventObject rEventObject = null;
                if (!this.EventTypeObjs.TryGetValue(rEventType, out rEventObject))
                {
                    rEventObject = new EventObject(this.TargetGo, rEventType);
                    rEventObject.AddEvent(rEventHandler);
                    EventTypeObjs.Add(rEventType, rEventObject);
                }
                else
                {
                    rEventObject.AddEvent(rEventHandler);
                }
            }

            public void RemoveEvent(EventTriggerType rEventType, Action<UnityEngine.Object> rAction)
            {
                EventObject rEventObject = null;
                if (this.EventTypeObjs.TryGetValue(rEventType, out rEventObject))
                {
                    rEventObject.RemoveEvent(rAction);
                }
                if (rEventObject != null && rEventObject.ReferCount == 0)
                {
                    rEventObject.Dispose();
                    this.EventTypeObjs.Remove(rEventType);
                }
            }

            public void RemoveAll()
            {
                foreach (var rItem in EventTypeObjs)
                {
                    rItem.Value.Dispose();
                }
                EventTypeObjs.Clear();
            }

            public void Handle(EventTriggerType rEventType)
            {
                EventObject rEventObject = null;
                if (this.EventTypeObjs.TryGetValue(rEventType, out rEventObject))
                {
                    rEventObject.Handle();
                }
            }

            public void Dispose()
            {
                foreach (var rItem in EventTypeObjs)
                {
                    if (rItem.Value.ReferCount == 0)
                    {
                        rItem.Value.Dispose();
                    }
                }
            }

            public int ReferCount
            {
                get
                {
                    int nReferCount = 0;
                    foreach (var rItem in EventTypeObjs)
                    {
                        nReferCount += rItem.Value.ReferCount;
                    }
                    return nReferCount;
                }
            }

            public void Remove(EventTriggerType rEventType)
            {
                EventObject rEventObject = null;
                if (this.EventTypeObjs.TryGetValue(rEventType, out rEventObject))
                {
                    rEventObject.Dispose();
                }
                this.EventTypeObjs.Remove(rEventType);
            }
        }

        private Dict<UnityEngine.Object, EventTypeObject>   mEvents;
        public  Dict<UnityEngine.Object, EventTypeObject>   Events { get { return mEvents; } }
        
        private HotfixEventManager()
        {
        }
        
        public void Initialize()
        {
            // 加载Hotfix端的代码
            mEvents = new Dict<UnityEngine.Object, EventTypeObject>();
        }

        public void Handle(UnityEngine.Object rTargetGo, EventTriggerType rEventType)
        {
            EventTypeObject rEventObject = null;
            if (this.mEvents.TryGetValue(rTargetGo, out rEventObject))
            {
                rEventObject.Handle(rEventType);
            }
        }
        
        public void Binding(UnityEngine.Object rTargetGo, EventTriggerType rEventType, System.Action<UnityEngine.Object> rEventHandler)
        {
            EventTypeObject rEventObject = null;
            if (!this.mEvents.TryGetValue(rTargetGo, out rEventObject))
            {
                rEventObject = new EventTypeObject(rTargetGo);
                rEventObject.AddEvent(rEventType, rEventHandler);
                mEvents.Add(rTargetGo, rEventObject);
            }
            else
            {
                rEventObject.AddEvent(rEventType, rEventHandler);
            }
        }

        public void UnBinding(UnityEngine.Object rTargetGo, EventTriggerType rEventType, System.Action<UnityEngine.Object> rEventHandler)
        {
            EventTypeObject rEventObject = null;
            if (this.mEvents.TryGetValue(rTargetGo, out rEventObject))
            {
                rEventObject.RemoveEvent(rEventType, rEventHandler);
            }
            if (rEventObject != null && rEventObject.ReferCount == 0)
            {
                rEventObject.Dispose();
                this.mEvents.Remove(rTargetGo);
            }
        }

        public void RemoveOne(UnityEngine.Object rObj)
        {
            EventTypeObject rEventObject = null;
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
    }
}
