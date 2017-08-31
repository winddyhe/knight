using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Hotfix
{
    public class HotfixEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        public int EventTypeMask = 0;

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnCancel(BaseEventData eventData)
        {

        }

        public void OnDeselect(BaseEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnDrop(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {

        }

        public void OnMove(AxisEventData eventData)
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogError("OnPointerClick...");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.LogError("OnPointerDown...");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.LogError("OnPointerEnter...");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.LogError("OnPointerExit...");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.LogError("OnPointerUp...");
        }

        public void OnScroll(PointerEventData eventData)
        {

        }

        public void OnSelect(BaseEventData eventData)
        {

        }

        public void OnSubmit(BaseEventData eventData)
        {

        }

        public void OnUpdateSelected(BaseEventData eventData)
        {

        }
    }
}
