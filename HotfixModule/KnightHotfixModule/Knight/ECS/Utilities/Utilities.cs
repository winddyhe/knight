//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Game;

namespace Game.Knight
{
    public class Utilities
    {
        public static EntityCamera GetEntityCamera()
        {
            return World.Instance.GameWorld.MainCamera;
        }
    }
}
