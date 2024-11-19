using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class RenderResolutionSetting
    {
        private static int mOriginWidth = 1920;
        private static int mOriginHeight = 1080;

        public static int OriginWidth { get { return mOriginWidth; } }
        public static int OriginHeight { get { return mOriginHeight; } }

        public static int LevelHeight { get; set; }
        public static int LevelWidth { get; set; }

        public static void Initialize()
        {
            mOriginWidth = Screen.width;
            mOriginHeight = Screen.height;

            Knight.Core.LogManager.Log("Origin Resolution: " + mOriginWidth + ", " + mOriginHeight);
        }

        public static int GetCurResolution()
        {
            return Screen.height;
        }

        public static void SetResolution(int nLevelHeight)
        {
            LevelHeight = nLevelHeight;

#if !UNITY_EDITOR && !UNITY_STANDALONE
            var fOriginRatio = (float)mOriginWidth / (float)mOriginHeight;

            if (nLevelHeight > mOriginHeight)  // 取小
                LevelHeight = mOriginHeight;
            LevelWidth = (int)(fOriginRatio * LevelHeight);

            // 只有原始的分辨率比Level分辨率大的时候才设置
            Knight.Core.LogManager.Log("Screen.SetResolution: " + LevelWidth + ", " + LevelHeight);
            Screen.SetResolution(LevelWidth, LevelHeight, true);
#endif
        }
    }
}
