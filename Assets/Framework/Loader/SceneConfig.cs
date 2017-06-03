//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public class SceneConfig : MonoBehaviour
    {
        public Vector3  CameraPos       = Vector3.zero;
        public Vector3  CameraRotate    = Vector3.zero;
        public Color    CameraBGColor   = Color.black;
        public float    CameraFOV       = 60f;
        public float    CameraFar       = 500f;
        public float    CameraNear      = 0.3f;
    }
}

