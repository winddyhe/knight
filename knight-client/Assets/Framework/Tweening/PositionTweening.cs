using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Knight.Framework.Tweening
{
    public class PositionTweening : TweeningAnimation
    {
        public Vector3  Start;
        public Vector3  End;
        
        public bool     IsLocal;

        protected override void OnCreateTweener()
        {
            if (!this.IsLocal)
            {
                this.transform.position = this.Start;
                this.mTweener = this.transform.DOMove(this.End, this.Duration);
            }
            else
            {
                this.transform.localPosition = this.Start;
                this.mTweener = this.transform.DOLocalMove(this.End, this.Duration);
            }
        }
    }
}
