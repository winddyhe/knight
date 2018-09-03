using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Knight.Core;

namespace Knight.Framework.Tweening
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasAlphaTweening : TweeningAnimation
    {
        public  float       Start;
        public  float       End;

        private CanvasGroup CanvasGroup = null;
        
        protected override void OnCreateTweener()
        {
            this.CanvasGroup = this.gameObject.ReceiveComponent<CanvasGroup>();
            this.CanvasGroup.alpha = this.Start;

            this.mTweener = DOTween.To(this.Getter, this.Setter, this.End, this.Duration);
        }

        private float Getter()
        {
            return this.CanvasGroup.alpha;
        }

        private void Setter(float fAlpha)
        {
            this.CanvasGroup.alpha = fAlpha;
        }
    }
}