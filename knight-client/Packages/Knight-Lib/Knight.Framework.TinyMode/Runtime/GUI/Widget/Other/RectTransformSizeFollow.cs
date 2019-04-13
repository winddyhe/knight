//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{
    [ExecuteInEditMode]
    public class RectTransformSizeFollow : MonoBehaviour
    {
        public RectTransform    FollowTarget;
        public Rect             PaddingRect;

        private RectTransform   mSelfTrans;

        public bool             LimitMaxWidth;
        public int              MaxWidth;

        public bool             LimitMaxHeight;
        public int              MaxHeight;

        private void OnEnable()
        {
            this.Follow();
        }

        private void OnDisable()
        {
            this.Follow();
        }

        private void Update()
        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying)
//            {
                this.Follow();
//            }
//#endif
        }

        public void Follow()
        {
            if (this.FollowTarget == null) return;
            if (this.mSelfTrans == null)
                this.mSelfTrans = this.gameObject.GetComponent<RectTransform>();
            if (this.mSelfTrans == null) return;

            float fWidth = this.FollowTarget.rect.width + this.PaddingRect.width;
            if (this.LimitMaxWidth && fWidth > MaxWidth)
            {
                fWidth = MaxWidth;
            }
            float fHeight = this.FollowTarget.rect.height + this.PaddingRect.height;
            if (this.LimitMaxHeight && fHeight > MaxHeight)
            {
                fHeight = MaxHeight;
            }
            this.mSelfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fWidth);
            this.mSelfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fHeight);
        }
    }
}
