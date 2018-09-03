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
        [Serializable]
        public class ToggleEvent : UnityEvent<int>
        {
        }

        public GameObject       TabTemplateGo;
        public TabButton        CurTabBtn;
        
        public ToggleEvent      OnTabChangedFunc = new ToggleEvent();
        
        [HideInInspector]
        public List<TabButton>  TabButtons;
        
        public void OnTabChanged(TabButton rTabBtn)
        {
            this.CurTabBtn = rTabBtn;
            if (OnTabChangedFunc != null)
            {
                this.OnTabChangedFunc.Invoke(this.CurTabBtn.TabIndex);
            }
        }
    }
}
