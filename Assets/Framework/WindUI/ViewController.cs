using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.WindUI
{
    public class ViewController
    {
        public HotfixObject HotfixObj;
        public string       ParentType  = "WindHotfix.GUI.TViewController`1";
        
        public ViewController(string rHotfixName)
        {
            this.HotfixObj = HotfixApp.Instance.Instantiate(rHotfixName);
            this.ParentType = string.Format("WindHotfix.GUI.TViewController`1<{0}>", rHotfixName);
        }

        public virtual void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Initialize", rObjs, rBaseDatas);
        }

        /// <summary>
        /// 该View是否被打开
        /// </summary>
        public bool IsOpened
        {
            get
            {
                if (this.HotfixObj == null) return false;
                return (bool)this.HotfixObj.InvokeParent(this.ParentType, "get_IsOpened");
            }
            set
            {
                if (this.HotfixObj == null) return;
                this.HotfixObj.InvokeParent(this.ParentType, "set_IsOpened", value);
            }
        }

        /// <summary>
        /// 该View是否被关掉
        /// </summary>
        public bool IsClosed
        {
            get
            {
                if (this.HotfixObj == null) return false;
                return (bool)this.HotfixObj.InvokeParent(this.ParentType, "get_IsClosed");
            }
            set
            {
                if (this.HotfixObj == null) return;
                this.HotfixObj.InvokeParent(this.ParentType, "set_IsClosed", value);
            }
        }

        /// <summary>
        /// 开始打开View时候要做的事情
        /// </summary>
        public virtual void OnOpening()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Opening");
        }

        /// <summary>
        /// 彻底打开View时候要做的事情
        /// </summary>
        public virtual void OnOpened()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnOpened");
        }

        /// <summary>
        /// 显示View要做的操作
        /// </summary>
        public virtual void OnShow()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnShow");
        }

        /// <summary>
        /// 隐藏View要做的事情
        /// </summary>
        public virtual void OnHide()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnHide");
        }

        /// <summary>
        /// 刷新页面要做的操作
        /// </summary>
        public virtual void OnRefresh()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.Invoke("OnRefresh");
        }

        /// <summary>
        /// 开始关闭时候要做的操作
        /// </summary>
        public virtual void OnClosing()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Closing");
        }

        /// <summary>
        /// 彻底关闭后要做的操作
        /// </summary>
        public virtual void OnClosed()
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "Closed");
        }

        /// <summary>
        /// 事件传递
        /// </summary>
        public virtual void OnUnityEvent(Object rTarget)
        {
            if (this.HotfixObj == null) return;
            this.HotfixObj.InvokeParent(this.ParentType, "OnUnityEvent", rTarget);
        }
    }
}
