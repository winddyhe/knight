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
