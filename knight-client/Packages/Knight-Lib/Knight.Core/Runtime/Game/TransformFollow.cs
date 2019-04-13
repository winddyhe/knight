using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Knight.Core
{
    [ExecuteInEditMode]
    public class TransformFollow : MonoBehaviour
    {
        public Transform    Target;

        public bool         IsFollowPos;
        [ShowIf("IsFollowPos")]
        public Vector3      PositionOffset;

        public bool         IsFollowRotate;
        [ShowIf("IsFollowRotate")]
        public Vector3      RotateOffset;

        public bool         IsFollowScale;
        [ShowIf("IsFollowScale")]
        public float        ScaleOffset = 1.0f;

        private void Update()
        {
            if (this.Target == null) return;

            if (this.IsFollowPos)
            {
                this.transform.position = this.Target.transform.position + (this.PositionOffset*this.Target.lossyScale.x);
            }
            if (this.IsFollowRotate)
            {
                this.transform.eulerAngles = this.Target.transform.eulerAngles + this.RotateOffset;
            }
            if (this.IsFollowScale)
            {
                this.transform.localScale = this.Target.transform.lossyScale * this.ScaleOffset;
            }
        }
    }
}
