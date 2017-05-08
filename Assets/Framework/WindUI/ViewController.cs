using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.WindUI
{
    public class ViewController
    {
        public string HotfixName;
        public string ParentType = "WindHotfix.GUI.THotfixViewController`1";

        public ViewController()
        {
        }

        public void SetHotfixName(string rHotfixName)
        {
            this.HotfixName = rHotfixName;
            this.ParentType = string.Format("WindHotfix.GUI.THotfixViewController`1<{0}>", rHotfixName);
        }

        public virtual void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
        }

        /// <summary>
        /// 该View是否被打开
        /// </summary>
        public virtual bool IsOpened
        {
            get;
            set;
        }

        /// <summary>
        /// 该View是否被关掉
        /// </summary>
        public virtual bool IsClosed
        {
            get;
            set;
        }

        /// <summary>
        /// 开始打开View时候要做的事情
        /// </summary>
        public virtual void Opening()
        {
        }

        /// <summary>
        /// 彻底打开View时候要做的事情
        /// </summary>
        public virtual void OnOpened()
        {
        }

        /// <summary>
        /// 显示View要做的操作
        /// </summary>
        public virtual void OnShow()
        {
        }

        /// <summary>
        /// 隐藏View要做的事情
        /// </summary>
        public virtual void OnHide()
        {
        }

        /// <summary>
        /// 刷新页面要做的操作
        /// </summary>
        public virtual void OnRefresh()
        {
        }

        /// <summary>
        /// 开始关闭时候要做的操作
        /// </summary>
        public virtual void Closing()
        {
        }

        /// <summary>
        /// 彻底关闭后要做的操作
        /// </summary>
        public virtual void Closed()
        {
        }

        /// <summary>
        /// 事件传递
        /// </summary>
        public virtual void OnUnityEvent(Object rTarget)
        {
        }
    }
}
