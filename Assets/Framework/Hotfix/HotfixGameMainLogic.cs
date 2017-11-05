using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task Initialize()
        {
            // 加载Hotfix端的代码
            this.MainLogicHotfixObj = HotfixApp.Instance.Instantiate(this.MainLogicScript);

            // 加载Hotfix端的代码
            await (HotfixApp.Instance.Invoke(this.MainLogicHotfixObj, "Initialize") as Task);
        }

        void Update()
        {
            if (this.MainLogicHotfixObj == null) return;
            HotfixApp.Instance.Invoke(this.MainLogicHotfixObj, "Update");
        }
    }
}
