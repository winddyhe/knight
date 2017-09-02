using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Hotfix
{
    public class HotfixGameMainLogic : MonoBehaviour
    {
        private static HotfixGameMainLogic  __instance;
        public  static HotfixGameMainLogic  Instance { get { return __instance; } }

        public  string                      MainLogicScript;

        public  HotfixObject                MainLogicHotfixObj;

        void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public IEnumerator Initialize()
        {
            // 加载Hotfix端的代码
            this.MainLogicHotfixObj = HotfixApp.Instance.Instantiate(this.MainLogicScript);
            
            // 加载Hotfix端的代码
            yield return HotfixApp.Instance.Invoke(this.MainLogicHotfixObj, "Initialize") as IEnumerator;

            // 初始化事件模块
            HotfixEventManager.Instance.Initialize();
        }

        void Update()
        {
            if (this.MainLogicHotfixObj == null) return;
            HotfixApp.Instance.Invoke(this.MainLogicHotfixObj, "Update");
        }
    }
}
