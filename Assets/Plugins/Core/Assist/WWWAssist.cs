//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Core
{
    /// <summary>
    /// WWW的辅助类
    /// </summary> 
    public class WWWAssist
    {
        public static WWW Load(string rURL)
        {
            WWW www = new WWW(rURL);
            return www;
        }

        public static void Destroy(ref WWW www)
        {
            if (www == null) return;

            www.Dispose();
            www = null;
        }

        public static void Dispose(ref WWW www, bool isUnloadAllLoadedObjects = false)
        {
            if (www == null) return;

            if (www.assetBundle != null)
                www.assetBundle.Unload(isUnloadAllLoadedObjects);

            www.Dispose();
            www = null;
        }
    }
}
