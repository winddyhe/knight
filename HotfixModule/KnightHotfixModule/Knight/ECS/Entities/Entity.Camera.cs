using System;
using System.Collections.Generic;
using WindHotfix.Game;
using UnityEngine;

namespace Game.Knight
{
    public class EntityCamera : GameEntity
    {
        public ComponentCamera          CompCamera;
        public ComponentCameraSetting   CompCameraSetting;
        public ComponentCameraFollower  CompCameraFollower;

        public void Create(Camera rMainCamera, StageConfig rStageConfig, GameObject rTargetGo)
        {
            this.CompCamera = this.AddComponent<ComponentCamera>();
            this.CompCamera.Camera = rMainCamera;

            this.CompCameraSetting = this.AddComponent<ComponentCameraSetting>();
            this.CompCameraSetting.AngleX   = rStageConfig.CameraSettings[0];
            this.CompCameraSetting.AngleY   = rStageConfig.CameraSettings[1];
            this.CompCameraSetting.Distance = rStageConfig.CameraSettings[2];
            this.CompCameraSetting.OffsetY  = rStageConfig.CameraSettings[3];
            this.CompCameraSetting.Target   = rTargetGo;

            this.CompCameraFollower = this.AddComponent<ComponentCameraFollower>();
        }                                       
    }
}
                                