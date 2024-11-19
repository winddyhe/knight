using System;
using System.Collections.Generic;
using System.IO;

namespace Knight.Core
{
    public static class UtilTool
    {
        public static float WrapAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
