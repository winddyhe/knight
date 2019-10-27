using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonPosHelper : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public class OnShowEvent : UnityEvent<Vector3,bool> { }
    public OnShowEvent ButtonEvent = new OnShowEvent();
    public void OnPointerDown(PointerEventData eventData)
    {
        this.ButtonEvent?.Invoke(this.gameObject.transform.localPosition, true);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        this.ButtonEvent?.Invoke(this.gameObject.transform.localPosition, false);
    }
}
