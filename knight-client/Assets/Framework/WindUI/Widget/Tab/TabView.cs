using Knight.Framework.Hotfix;
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
                HotfixEventManager.Instance.Handle(this, Knight.Framework.HEventTriggerType.TabChanged);
            }
            Knight.Core.UtilTool.SafeExecute(OnTabChangedFunc, rTabBtn);
        }
    }
}
