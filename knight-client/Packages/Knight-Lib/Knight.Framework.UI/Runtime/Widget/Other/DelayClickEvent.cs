using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditor;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/LongClickButton")]
    public class DelayClickEvent : Button
    {
        [Serializable]
        public class LongClickEvent : UnityEvent<bool> { }

        public LongClickEvent OnLongClick = new LongClickEvent();

        private DateTime mFirstClickTime = default;
        private DateTime mSecondClickTime = default;

        [SerializeField]
        [HideInInspector]
        public float mLongClickLimitTime;
        public float LongClickLimitTime
        {
            get
            {
                return this.mLongClickLimitTime;
            }
            set
            {
                this.mLongClickLimitTime = value;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (this.mFirstClickTime.Equals(default))
                this.mFirstClickTime = DateTime.Now;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!this.mFirstClickTime.Equals(default))
                this.mSecondClickTime = DateTime.Now;
            if (!this.mFirstClickTime.Equals(default) && !this.mSecondClickTime.Equals(default))
            {
                var rTimeSpan = this.mSecondClickTime - this.mFirstClickTime;
                int nMS = rTimeSpan.Milliseconds + rTimeSpan.Seconds * 1000;
                this.OnLongClick?.Invoke(nMS >= this.mLongClickLimitTime * 1000);
                this.ResetDefaultTime();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            this.ResetDefaultTime();
        }

        private void ResetDefaultTime()
        {
            this.mFirstClickTime = default;
            this.mSecondClickTime = default;
        }
    }
}
