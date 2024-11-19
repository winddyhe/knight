using Knight.Core;
using System.IO;
using UnityEngine;

namespace Knight.Framework.GameSave
{
    public class GameSavePath
    {
        public static string GetSavePath(string rSaveFileName)
        {
            var rSavePath = "";
#if UNITY_ANDROID
            rSavePath = UtilUnityTool.GetAndroidHideDataRoot() + "/save_datas/" + rSaveFileName + ".bin";
#else
            rSavePath = Application.persistentDataPath + "/save_datas/" + rSaveFileName + ".bin";
#endif
            var rSaveDir = Path.GetDirectoryName(rSavePath);
            if (!Directory.Exists(rSaveDir))
            {
                Directory.CreateDirectory(rSaveDir);
            }
            return rSavePath;
        }
    }
}
