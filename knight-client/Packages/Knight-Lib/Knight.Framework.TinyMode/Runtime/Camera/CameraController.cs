//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Knight.Core;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode
{
    [System.Serializable]
    public class CameraSetting
    {
        public float        AngleX   = -1f;
        public float        AngleY   = 36;
        public float        Distance = 18;
        public float        OffsetY  = 0.5f;
        public GameObject   Target;
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public Camera           Camera;
        public CameraSetting    CameraSettings;
        public CameraFollower   CameraFollower;
        public bool             IsDirectFollow;

        private Vector3         mOffset;
        private Vector3         mDirection = new Vector3(0, 0, -1);
        
        void Start()
        {
            this.Camera = this.gameObject.GetComponent<Camera>();
            this.CameraFollower = new CameraFollower();
            this.CameraFollower.Initialize(this.Camera, this.CameraSettings);
        }
        
        void FixedUpdate()
        {
            if (this.Camera == null) return;
            if (this.CameraSettings.Target == null) return;

            this.mOffset.y = this.CameraSettings.OffsetY;

            Vector3 rTargetPos = this.CameraSettings.Target.transform.position + this.mOffset + 
                                 Quaternion.AngleAxis(this.CameraSettings.AngleX, Vector3.up) *
                                 Quaternion.AngleAxis(this.CameraSettings.AngleY, Vector3.right) * mDirection * this.CameraSettings.Distance;

            if (this.IsDirectFollow)
            {
                this.Camera.transform.position = rTargetPos;
            }
            else
            {
                if (this.CameraFollower != null)
                    this.CameraFollower.Update(rTargetPos);
            }
            this.Camera.transform.rotation = Quaternion.LookRotation(this.CameraSettings.Target.transform.position + mOffset - rTargetPos);
        }

    }
}