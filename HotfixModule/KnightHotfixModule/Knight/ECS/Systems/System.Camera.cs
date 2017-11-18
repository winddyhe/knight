//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using WindHotfix.Game;

namespace Game.Knight
{
    public class SystemCamera : TGameSystem<ComponentCamera, ComponentCameraSetting, ComponentCameraFollower>
    {
        protected override void OnStart(ComponentCamera rCompCamera, ComponentCameraSetting rCompCameraSetting, ComponentCameraFollower rCompCameraFollower)
        {
            rCompCameraFollower.Enable = true;
        }

        protected override void OnUpdate(ComponentCamera rCompCamera, ComponentCameraSetting rCompCameraSetting, ComponentCameraFollower rCompCameraFollower)
        {
            rCompCamera.Offset.y = rCompCameraSetting.OffsetY;

            Vector3 rTargetPos = rCompCameraSetting.Target.transform.position + rCompCamera.Offset +
                                 Quaternion.AngleAxis(rCompCameraSetting.AngleX, Vector3.up) *
                                 Quaternion.AngleAxis(rCompCameraSetting.AngleY, Vector3.right) * rCompCamera.Direction * rCompCameraSetting.Distance;

            if (rCompCameraFollower.Enable)
            {
                Vector3 rDisp = rCompCamera.Camera.transform.position - rTargetPos;
                float rLength = rDisp.magnitude;

                if (rLength <= Mathf.Epsilon) return;

                rDisp = rDisp / rLength;
                rDisp *= -(rCompCameraFollower.SpringConst * rCompCameraFollower.SpringRate) * rLength * Time.deltaTime;
                rCompCamera.Camera.transform.position += rDisp;
            }

            rCompCamera.Camera.transform.rotation = Quaternion.LookRotation(rCompCameraSetting.Target.transform.position + rCompCamera.Offset - rTargetPos);
        }
    }
}
