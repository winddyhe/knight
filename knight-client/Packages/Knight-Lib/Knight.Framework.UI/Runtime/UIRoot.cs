//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UIRoot : MonoBehaviour
    {
        private static UIRoot   __instance;
        public  static UIRoot   Instance      { get { return __instance; } }

        public GameObject       DynamicRoot;
        public GameObject       GlobalsRoot;
        public GameObject       PopupRoot;

        public Camera           UICamera;
        public EventSystem      UIEventSystem;

        void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public void Initialize()
        {
            this.DynamicRoot.SetActive(true);
            this.GlobalsRoot.SetActive(true);
            this.PopupRoot.SetActive(true);
        }

        public void SetEventSystemEnable(bool bEnabled)
        {
            this.UIEventSystem.enabled = bEnabled;
        }
    }
}
