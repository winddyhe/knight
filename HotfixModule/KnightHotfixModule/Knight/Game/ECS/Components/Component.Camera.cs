using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    public class ComponentCamera : GameComponent
    {
        public Camera           Camera;

        public Vector3          Offset;
        public Vector3          Direction = new Vector3(0, 0, -1);
    }

    public class ComponentCameraSetting : GameComponent
    {
        public float            AngleX;
        public float            AngleY;
        public float            Distance;
        public float            OffsetY;
        public GameObject       Target;
    }

    public class ComponentCameraFollower : GameComponent
    {
        public bool             Enable;

        public float            SpringConst = 4.0f;
        public float            SpringRate = 2.0f;
    }
}
