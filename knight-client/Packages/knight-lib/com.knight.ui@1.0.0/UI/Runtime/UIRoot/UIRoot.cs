using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Knight.Framework.UI
{
    public class UIRoot : MonoBehaviour
    {
        private static UIRoot __instance;
        public static UIRoot Instance => __instance;

        public Camera UICamera;
        public EventSystem EventSystem;

        public GameObject FixedRoot;
        public GameObject DynamicRoot;
        public GameObject PopupBlurRoot;
        public GameObject GlobalRoot;

        private void Awake()
        {
            __instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}
