using Knight.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class TabView : ToggleGroup
    {
        public GameObject               TabTemplateGo;
        public TabButton                CurTabBtn;
        
        public UnityEvent<EventArg>     OnTabChangedFunc;
        
        [HideInInspector]
        public List<TabButton>          TabButtons;
        
        public void OnTabChanged(TabButton rTabBtn)
        {
            this.CurTabBtn = rTabBtn;
            if (OnTabChangedFunc != null)
            {
                this.OnTabChangedFunc.Invoke(new EventArg(this.CurTabBtn.TabIndex));
            }
        }
    }
}
