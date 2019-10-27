using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UIWait : MonoBehaviour
    {
        private static UIWait __instance = null;
        public  static UIWait Instance { get { return __instance; } }

        public GameObject   WaitPanelGO = null;
        public Text         TxtWaitTips = null;
        public float        MinWaitTime = 1;        // 等几秒钟开始弹Wait的UI，锁死界面
        public float        CurWaitTime = 0.0f;
        
        public List<string> WaitItems   = new List<string>();
        
        public void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
                this.gameObject.SetActive(false);

                this.WaitPanelGO.SetActive(false);
                this.WaitItems = new List<string>();
            }
        }

        public void Update()
        {
            var bActive = this.WaitItems.Count > 0;
            if (!bActive)
            {
                this.WaitPanelGO.SetActive(false);
                this.CurWaitTime = 0.0f;
                return;
            }
            if (this.CurWaitTime >= this.MinWaitTime)
            {
                this.WaitPanelGO.SetActive(true);
            }
            this.CurWaitTime += Time.deltaTime;
        }

        public void StartWait(string rWaitName, bool bImmediately = false)
        {
            if (!this.WaitItems.Contains(rWaitName))
            {
                this.WaitItems.Add(rWaitName);
            }
            if (bImmediately)
            {
                this.WaitPanelGO.SetActive(true);
            }
        }

        public void EndWait(string rWaitName)
        {
            this.WaitItems.Remove(rWaitName);
        }
    }
}
