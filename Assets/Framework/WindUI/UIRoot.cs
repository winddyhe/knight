//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.WindUI
{
    public class UIRoot : MonoBehaviour
    {
        private static UIRoot   __instance;
        public  static UIRoot   Instance      { get { return __instance; } }

        public GameObject       DynamicRoot;
        public GameObject       GlobalsRoot;

        void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public void Initialize()
        {
            this.DynamicRoot.SetActive(true);
            this.GlobalsRoot.SetActive(true);
        }
    }
}
