//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;

namespace Framework
{
    [System.Serializable]
    public class CameraSetting
    {
        public float        AngleX   = 0;
        public float        AngleY   = 36;
        public float        Distance = 10;
        public float        OffsetY  = 0.5f;
        public GameObject   Target;
    }

    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public Camera           Camera;
        public CameraSetting    CameraSettings;
        public CameraFollower   CameraFollower;

        private Vector3         mOffset;
        private Vector3         mDirection = new Vector3(0, 0, -1);
        
        void Start()
        {
            this.Camera = this.gameObject.GetComponent<Camera>();
            this.CameraFollower = new CameraFollower();
            this.CameraFollower.Initialize(this.Camera, this.CameraSettings);
        }

        void Update()
        {
            this.mOffset.y = this.CameraSettings.OffsetY;

            Vector3 rTargetPos = this.CameraSettings.Target.transform.position + this.mOffset + 
                                 Quaternion.AngleAxis(this.CameraSettings.AngleX, Vector3.up) *
                                 Quaternion.AngleAxis(this.CameraSettings.AngleY, Vector3.right) * mDirection * this.CameraSettings.Distance;

            if (this.CameraFollower != null)
                this.CameraFollower.Update(rTargetPos);

            this.Camera.transform.rotation = Quaternion.LookRotation(this.CameraSettings.Target.transform.position + mOffset - rTargetPos);
        }
    }
}