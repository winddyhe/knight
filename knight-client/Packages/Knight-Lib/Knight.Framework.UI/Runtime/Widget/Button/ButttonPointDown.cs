using Knight.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace UnityEditor.UI
{
    [RequireComponent(typeof(Button))]
    public class ButttonPointDown : MonoBehaviour, IPointerDownHandler
    {
        public ButtonClickedEvent OnBtnPointerDown = new ButtonClickedEvent();
        public void OnPointerDown(PointerEventData eventData)
        {
            this.OnBtnPointerDown?.Invoke();
        }
    }
}
