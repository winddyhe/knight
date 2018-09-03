using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Knight.Framework.Tweening
{
    public class ScaleTweening : TweeningAnimation
    {
        public Vector3  Start;
        public Vector3  End;

        protected override void OnCreateTweener()
        {
            this.transform.localScale = this.Start;
            this.mTweener = this.transform.DOScale(this.End, this.Duration);
        }
    }
}
