//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Hotfix
{
    public class HotfixEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        public enum TriggerType
        {
            PointerEnter            = 1 << 0,
            PointerExit             = 1 << 1,
            PointerDown             = 1 << 2,
            PointerUp               = 1 << 3,
            PointerClick            = 1 << 4,
            Drag                    = 1 << 5,
            Drop                    = 1 << 6,
            Scroll                  = 1 << 7,
            UpdateSelected          = 1 << 8,
            Select                  = 1 << 9,
            Deselect                = 1 << 10,
            Move                    = 1 << 11,
            InitializePotentialDrag = 1 << 12,
            BeginDrag               = 1 << 13,
            EndDrag                 = 1 << 14,
            Submit                  = 1 << 15,
            Cancel                  = 1 << 16
        }
        
        public TriggerType          EventTypeMask = 0;
        public UnityEngine.Object   EventObj;
        
        private void Handle(BaseEventData eventData, EventTriggerType eventType)
        {
            HotfixEventManager.Instance.Handle(this.EventObj, eventType);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.BeginDrag);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.BeginDrag);
            }
        }

        public void OnCancel(BaseEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Cancel);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Cancel);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Deselect);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Deselect);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Drag);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {

                this.Handle(eventData, EventTriggerType.Drag);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Drop);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Drop);

            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.EndDrag);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {

                this.Handle(eventData, EventTriggerType.EndDrag);
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.InitializePotentialDrag);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {

                this.Handle(eventData, EventTriggerType.InitializePotentialDrag);
            }
        }

        public void OnMove(AxisEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Move);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {

                this.Handle(eventData, EventTriggerType.Move);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.PointerClick);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.PointerClick);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.PointerDown);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.PointerDown);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.PointerEnter);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.PointerEnter);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.PointerExit);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.PointerExit);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.PointerUp);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.PointerUp);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Scroll);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Scroll);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Select);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Select);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.Submit);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.Submit);
            }
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            int nTemp = (1 << (int)EventTriggerType.UpdateSelected);
            int nEventMask = (int)this.EventTypeMask;
            if ((nEventMask & nTemp) == nTemp)
            {
                this.Handle(eventData, EventTriggerType.UpdateSelected);
            }
        }

        public void OnSpecial()
        {
            HotfixEventManager.Instance.Handle(this.EventObj, (EventTriggerType)100);
        }
    }
}
