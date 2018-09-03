using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knight.Hotfix.Core;
using UnityEngine;

namespace Game
{
    public class FrameManager : THotfixMB<FrameManager>
    {
        private static FrameManager     __instance;
        public  static FrameManager     Instance { get { return __instance; } }

        [HotfixBinding("MiddlePanel")]
        public RectTransform            ContentMiddlePanel;

        public override void Awake()
        {
            __instance = this;
            
            // 初始化这个Page节点
            ViewManager.Instance.PageRoot = this.ContentMiddlePanel.gameObject;
        }

        public async Task<View> OpenPageUIAsync(string rViewName, Action<View> rOpenCompleted = null)
        {
            return await ViewManager.Instance.OpenAsync(rViewName, View.State.Page, rOpenCompleted);
        }

        public void OpenPageUI(string rViewName, Action<View> rOpenCompleted = null)
        {
            ViewManager.Instance.Open(rViewName, View.State.Page, rOpenCompleted);
        }

        public async Task<View> OpenPopUIAsync(string rViewName, Action<View> rOpenCompleted = null)
        {
            return await ViewManager.Instance.OpenAsync(rViewName, View.State.Popup, rOpenCompleted);
        }

        public void OpenPopUI(string rViewName, Action<View> rOpenCompleted = null)
        {
            ViewManager.Instance.Open(rViewName, View.State.Popup, rOpenCompleted);
        }

        public void SetActive(string rViewGUID, bool bIsActive)
        {
            if (bIsActive)
                ViewManager.Instance.Show(rViewGUID);
            else
                ViewManager.Instance.Hide(rViewGUID);
        }
    }
}
