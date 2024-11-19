using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Core
{
    public static class UtilUnityTool
    {
        public static string GetTransformPathByTrans(Transform rTargetTrans, Transform rTrans)
        {
            string rPath = "";
            GetTransformPathByTrans(rTargetTrans, rTrans, ref rPath);
            return rPath;
        }

        public static void GetTransformPathByTrans(Transform rTargetTrans, Transform rTrans, ref string rPath)
        {
            if (rTrans == null || rTrans.parent == null || rTargetTrans == rTrans)
            {
                if (rTrans != null && rTargetTrans == rTrans)
                {
                    rPath = rTrans.name + (string.IsNullOrEmpty(rPath) ? rPath : "_" + rPath);
                }
                return;
            }
            rPath = rTrans.name + (string.IsNullOrEmpty(rPath) ? rPath : "_" + rPath);
            GetTransformPathByTrans(rTargetTrans, rTrans.parent, ref rPath);
        }

        public static void SetLayer(this GameObject rGo, string rLayerName)
        {
            if (rGo == null) return;

            SetLayer(rGo.transform, rLayerName);
        }

        public static void SetLayer(Transform rParent, string rLayerName)
        {
            if (rParent == null) return;

            rParent.gameObject.layer = LayerMask.NameToLayer(rLayerName);

            for (int i = 0; i < rParent.transform.childCount; i++)
            {
                var rTrans = rParent.transform.GetChild(i);
                SetLayer(rTrans, rLayerName);
            }
        }

        public static void SafeDestroy(UnityEngine.Object rObj)
        {
            SafeDestroy(rObj, false);
        }

        public static void SafeDestroy(UnityEngine.Object rObj, bool bAllowDestroyingAssets)
        {
            if (rObj)
                GameObject.DestroyImmediate(rObj, bAllowDestroyingAssets);
            rObj = null;
        }

        public static T SafeExecute<T>(Func<T> rActionAlloc) where T : new()
        {
            if (rActionAlloc == null) return default(T);
            return rActionAlloc();
        }

        public static void SafeExecute<T>(Action<T> rActionDestroy, T rItem) where T : new()
        {
            if (rActionDestroy == null) return;
            rActionDestroy(rItem);
        }

        public static GameObject CreateGameObject(string rName)
        {
            return new GameObject(rName);
        }

        public static T ReceiveComponent<T>(this GameObject rGo) where T : Component
        {
            var rComp = rGo.GetComponent<T>();
            if (rComp == null) rComp = rGo.AddComponent<T>();
            return rComp;
        }

        public static void SetActiveSafe(this GameObject rGo, bool bActive)
        {
            if (rGo == null) return;
            rGo.SetActive(bActive);
        }

        public static string GetAndroidHideDataRoot()
        {
            // 获取当前应用程序的包名
            string rPackageName = Application.identifier;
            // 获取Android的Context对象
            AndroidJavaClass rUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject rContext = rUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // 获取/data/data/packagename目录的路径
            AndroidJavaObject rFile = rContext.Call<AndroidJavaObject>("getFilesDir");
            string rPath = rFile.Call<string>("getAbsolutePath");
            LogManager.LogRelease(rPath);
            return rPath;
        }
    }
}
