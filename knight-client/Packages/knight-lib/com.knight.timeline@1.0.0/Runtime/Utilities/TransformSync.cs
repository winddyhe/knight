using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.Timeline
{
    [System.Serializable]
    public class TransformSyncData
    {
        public bool IsNeedMirror;
        public bool IsSetMirror;
        public bool IsFollowPosition;
        public int EffectViewSpecialType;
        public int EffectType;
        public bool IsPositionLocal;
        public Vector3 PositionOffset;
        public bool IsIgnoreOffsetY;

        public bool IsFollowRotation;
        public Vector3 RotateOffset;

        public bool IsFollowScale;
        public Vector3 ScaleOffset;

        public bool IsFollowInUpdate;
        public bool IsLinkTransformAsChild;

        public bool IsFollowRotationInit;
        public bool IsFollowScaleInit;

        public TransformSyncData Clone()
        {
            return new TransformSyncData()
            {
                IsNeedMirror = this.IsNeedMirror,
                IsSetMirror = this.IsSetMirror,
                IsFollowPosition = this.IsFollowPosition,
                EffectViewSpecialType = this.EffectViewSpecialType,
                EffectType = this.EffectType,
                IsPositionLocal = this.IsPositionLocal,
                PositionOffset = this.PositionOffset,
                IsIgnoreOffsetY = this.IsIgnoreOffsetY,
                IsFollowRotation = this.IsFollowRotation,
                RotateOffset = this.RotateOffset,
                IsFollowScale = this.IsFollowScale,
                ScaleOffset = this.ScaleOffset,
                IsFollowInUpdate = this.IsFollowInUpdate,
                IsLinkTransformAsChild = this.IsLinkTransformAsChild,
                IsFollowRotationInit = this.IsFollowRotationInit,
                IsFollowScaleInit = this.IsFollowScaleInit,
            };
        }
    }

    [ExecuteInEditMode]
    public class TransformSync : MonoBehaviour
    {
        public Transform Target;
        public TransformSyncData SyncData = new TransformSyncData();

        private void Start()
        {
            this.SetInitialTransform();
        }

        private void SetPosition()
        {
            if (this.SyncData.IsPositionLocal)
            {
                this.gameObject.transform.position = this.Target.position +
                                                     this.SyncData.PositionOffset.x * this.Target.right +
                                                     (this.SyncData.IsIgnoreOffsetY ? 0 : this.SyncData.PositionOffset.y) * this.Target.up +
                                                     this.SyncData.PositionOffset.z * this.Target.forward;
            }
            else
            {
                this.gameObject.transform.position = this.Target.position + this.SyncData.PositionOffset;
                if (this.SyncData.IsIgnoreOffsetY)
                {
                    this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z);
                }
            }
        }

        public void SetInitialTransform()
        {
            if (this.Target)
            {
                this.SetPosition();

                if (this.SyncData.IsFollowRotation || this.SyncData.IsFollowRotationInit)
                    this.gameObject.transform.eulerAngles = this.Target.eulerAngles + this.SyncData.RotateOffset;
                else
                    this.gameObject.transform.eulerAngles = Vector3.zero;

                if (this.SyncData.IsFollowScale || this.SyncData.IsFollowScaleInit)
                    this.gameObject.transform.localScale = this.Target.localScale + this.SyncData.ScaleOffset;
                else
                    this.gameObject.transform.localScale = Vector3.one;
            }
        }

        private void OnEnable()
        {
            this.SetInitialTransform();
        }

        private void LateUpdate()
        {
            if (this.Target == null) return;
            if (this.SyncData == null) return;
            if (!this.SyncData.IsFollowInUpdate) return;

            if (this.SyncData.IsFollowPosition)
            {
                this.SetPosition();
            }
            if (this.SyncData.IsFollowRotation)
            {
                this.gameObject.transform.eulerAngles = this.Target.eulerAngles + this.SyncData.RotateOffset;
            }
            if (this.SyncData.IsFollowScale)
            {
                this.gameObject.transform.localScale = this.Target.lossyScale + this.SyncData.ScaleOffset;
            }
        }
    }
}
