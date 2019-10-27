using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class DragPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    {
        public class OnDropEvent : UnityEvent
        {
        }

        public Action       OnDragBeginEvent;
        public Action       OnDragEvent;

        public OnDropEvent  OnDropHandler = new OnDropEvent();

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.OnDragBeginEvent?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.OnDragEvent?.Invoke();
        }

        public void OnDrop(PointerEventData eventData)
        {
            this.OnDropHandler?.Invoke();
        }
    }
}
