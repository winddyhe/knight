using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityObject = Framework.Hotfix.MonoBehaviourContainer.UnityObject;
using BaseDataObject = Framework.Hotfix.MonoBehaviourContainer.BaseDataObject;

namespace Framework.WindUI
{
    public class ViewController
    {
        protected List<UnityObject>         mObjects;
        protected List<BaseDataObject>      mBaseDatas;
        protected bool                      mIsOpened = false;
        protected bool                      mIsClosed = false;

        public virtual void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.mObjects = rObjs;
            this.mBaseDatas = rBaseDatas;
        }

        /// <summary>
        /// 该View是否被打开
        /// </summary>
        public bool IsOpened
        {
            get { return this.mIsOpened;  }
            set { this.mIsOpened = value; }
        }

        /// <summary>
        /// 该View是否被关掉
        /// </summary>
        public bool IsClosed
        {
            get { return this.mIsClosed;  }
            set { this.mIsClosed = value; }
        }

        /// <summary>
        /// 开始打开View时候要做的事情
        /// </summary>
        public virtual void OnOpening()
        {
            this.IsOpened = true;
        }

        /// <summary>
        /// 彻底打开View时候要做的事情
        /// </summary>
        public virtual void OnOpened()     { }

        /// <summary>
        /// 显示View要做的操作
        /// </summary>
        public virtual void OnShow()       { }

        /// <summary>
        /// 隐藏View要做的事情
        /// </summary>
        public virtual void OnHide()       { }

        /// <summary>
        /// 刷新页面要做的操作
        /// </summary>
        public virtual void OnRefresh()    { }

        /// <summary>
        /// 开始关闭时候要做的操作
        /// </summary>
        public virtual void OnClosing()
        {
            this.IsClosed = true;
        }

        /// <summary>
        /// 彻底关闭后要做的操作
        /// </summary>
        public virtual void OnClosed()     { }

        /// <summary>
        /// 事件传递
        /// </summary>
        public virtual void OnUnityEvent(Object rTarget)
        {
        }
    }
}
