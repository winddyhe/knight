using System;
using System.Collections.Generic;
using Knight.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Knight.Framework.Tweening
{
    public class ColorTweening : TweeningAnimation
    {
        public  Color           Start;
        public  Color           End;

        private List<Graphic>   mGrahics;
        private List<Material>  mMaterails;
        private Color           mCurColor;

        protected override void OnCreateTweener()
        {
            this.mGrahics = new List<Graphic>(this.gameObject.GetComponentsInChildren<Graphic>());

            this.mMaterails = new List<Material>();
            var rRenderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < rRenderers.Length; i++)
            {
                var rMat = rRenderers[i].sharedMaterial;
                if (!this.mMaterails.Contains(rMat))
                    this.mMaterails.Add(rMat);
            }

            this.Setter(this.Start);
            this.mTweener = DOTween.To(this.Getter, this.Setter, this.End, this.Duration);
        }

        private Color Getter()
        {
            return mCurColor;
        }

        private void Setter(Color rColor)
        {
            this.mCurColor = rColor;

            if (this.mGrahics != null)
            {
                for (int i = 0; i < this.mGrahics.Count; i++)
                {
                    this.mGrahics[i].color = rColor;
                }
            }

            if (this.mMaterails != null)
            {
                for (int i = 0; i < this.mMaterails.Count; i++)
                {
                    this.mMaterails[i].color = rColor;
                }
            }
        }
    }
}
