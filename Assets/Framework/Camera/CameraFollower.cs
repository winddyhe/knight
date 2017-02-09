//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public class CameraFollower
    {
        public bool          Enable;
        public CameraSetting CameraSettings;
        public Camera        Camera;

        public float         SpringConst = 4.0f;
        public float         SpringRate  = 2.0f;

        public void Initialize(Camera rCamera, CameraSetting rSetting)
        {
            this.Camera = rCamera;
            this.CameraSettings = rSetting;
            this.Enable = true;
        }

        /// <summary>
        /// 弹簧跟随算法
        /// </summary>
        public void Update(Vector3 rTargetPos)
        {
            if (!this.Enable) return;
            if (this.CameraSettings == null || this.CameraSettings.Target == null) return;

            Vector3 rDisp = this.Camera.transform.position - rTargetPos;
            float rLength = rDisp.magnitude;

            if (rLength <= Mathf.Epsilon) return;

            rDisp = rDisp / rLength;
            rDisp *= -(this.SpringConst * this.SpringRate) * rLength * Time.deltaTime;
            this.Camera.transform.position += rDisp;
        }
    }
}

