using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class TabView : ToggleGroup
    {
        public TabButton            CurTabBtn;
        public bool                 NeedHotfix;

        public Action<TabButton>    OnTabChangedFunc;
        
        public void OnTabChanged(TabButton rTabBtn)
        {
            this.CurTabBtn = rTabBtn;

            if (NeedHotfix)
            {
                HotfixEventManager.Instance.Handle(this, Framework.HEventTriggerType.TabChanged);
            }
            Core.UtilTool.SafeExecute(OnTabChangedFunc, rTabBtn);
        }
    }
}
