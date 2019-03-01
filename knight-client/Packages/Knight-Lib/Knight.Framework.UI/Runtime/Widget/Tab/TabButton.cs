using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class TabButton : Toggle
    {
        public int              TabIndex;
        public List<GameObject> TabContents;
        public List<GameObject> DeactiveTabContents;

        protected override void Awake()
        {
            this.onValueChanged.AddListener(OnTabValueChanged);
            this.OnTabValueChanged(this.isOn);
        }

        protected override void OnDestroy()
        {
            this.onValueChanged.RemoveAllListeners();
            
            if (this.TabContents != null)
                this.TabContents.Clear();

            if (this.DeactiveTabContents != null)
                this.DeactiveTabContents.Clear();
        }

        private void OnTabValueChanged(bool bValue)
        {
            if (this.TabContents != null)
            {
                for (int i = 0; i < this.TabContents.Count; i++)
                {
                    this.TabContents[i].SetActive(bValue);
                }
            }
            if (this.DeactiveTabContents != null)
            {
                for (int i = 0; i < this.DeactiveTabContents.Count; i++)
                {
                    this.DeactiveTabContents[i].SetActive(!bValue);
                }
            }
            TabView rTabView = this.group as TabView;
            if (rTabView != null && bValue)
            {
                rTabView.OnTabChanged(this);
            }
        }
    }
}
