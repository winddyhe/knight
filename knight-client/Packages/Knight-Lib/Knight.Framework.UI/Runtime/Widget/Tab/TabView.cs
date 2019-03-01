using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NaughtyAttributes;

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
        
        [ReorderableList]
        public List<TabButton>  TabButtons;
        
        /// <summary>
        /// @TODO: 这里有坑，CurTabIndex只能去设置它，只能用于OneWay
        ///        如果是TwoWay的话，逻辑会出问题。
        /// </summary>
        public  int             CurTabIndex
        {
            get
            {
                for (int i = 0; i < this.TabButtons.Count; i++)
                {
                    if (this.TabButtons[i].isOn)
                        return i;
                }
                return 0;
            }
            set
            {
                for (int i = 0; i < this.TabButtons.Count; i++)
                {
                    this.TabButtons[i].isOn = value == i;
                }
            }
        }
        
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
