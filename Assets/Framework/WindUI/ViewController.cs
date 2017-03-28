using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.WindUI
{
    public class IViewController
    {
        public virtual void Initialize(UnityEngine.Object[] rObjs) { }

        public virtual void OnOpening() { }

        public virtual void OnOpened()  { }

        public virtual void OnShow()    { }

        public virtual void OnHide()    { }

        public virtual void OnRefresh() { }

        public virtual void OnClosing() { }

        public virtual void OnClosed()  { }
    }

    public class ViewController : IViewController
    {
        protected Object[]      mObjects;
        public bool             IsOpened;
        public bool             IsClosed;

        public override void Initialize(Object[] rObjs)
        {
            this.mObjects = rObjs;
        }

        /// <summary>
        /// 开始打开View时候要做的事情
        /// </summary>
        public override void OnOpening() { }

        /// <summary>
        /// 彻底打开View时候要做的事情
        /// </summary>
        public override void OnOpened()  { }

        /// <summary>
        /// 显示View要做的操作
        /// </summary>
        public override void OnShow()    { }

        /// <summary>
        /// 隐藏View要做的事情
        /// </summary>
        public override void OnHide()    { }

        /// <summary>
        /// 刷新页面要做的操作
        /// </summary>
        public override void OnRefresh() { }

        /// <summary>
        /// 开始关闭时候要做的操作
        /// </summary>
        public override void OnClosing() { }

        /// <summary>
        /// 彻底关闭后要做的操作
        /// </summary>
        public override void OnClosed()  { }
    }
}
