//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public class MainUpdate : MonoBehaviour
    {
        private static MainUpdate __instance;
        public  static MainUpdate Instance { get { return __instance; } }

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;

                GameObject.DontDestroyOnLoad(this);
            }
        }
    }
}
