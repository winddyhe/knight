using System;
using System.Collections.Generic;

namespace Framework.WindUI
{
    public class IViewController
    {
        public virtual void OnOpening() { }

        public virtual void OnOpened()  { }

        public virtual void OnShow()    { }

        public virtual void OnHide()    { }

        public virtual void OnRefresh() { }

        public virtual void OnClosing() { }

        public virtual void OnClosed()  { }
    }

    public class ViewController<T> : IViewController where T : View
    {
        protected T         mView = null;

        public ViewController(T rView)
        {
            this.mView = rView;
        }

        /// <summary>
        /// 开始打开View时候要做的事情
        /// </summary>
        public override void OnOpening()
        {
            this.mView.IsOpened = true;
        }

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
        public override void OnClosing()
        {
            this.mView.IsClosed = true;
        }

        /// <summary>
        /// 彻底关闭后要做的操作
        /// </summary>
        public override void OnClosed()  { }
    }
}
