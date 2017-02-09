//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;

namespace Framework
{
    public class Joystick : TSingleton<Joystick>
    {
        public float x;
        public float y;

        private Joystick()
        {
        }

        public void Reset()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}


