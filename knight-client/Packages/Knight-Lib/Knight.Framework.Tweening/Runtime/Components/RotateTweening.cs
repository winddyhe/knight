using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Knight.Framework.Tweening
{
    public class RotateTweening : TweeningAnimation
    {
        public Vector3  Start;
        public Vector3  End;

        public bool     IsLocal;

        protected override void OnCreateTweener()
        {
            if (!this.IsLocal)
            {
                this.transform.eulerAngles = this.Start;
                this.mTweener = this.transform.DORotate(this.End, this.Duration);
            }
            else
            {
                this.transform.localEulerAngles = this.Start;
                this.mTweener = this.transform.DOLocalRotate(this.End, this.Duration);
            }
        }
    }
}

