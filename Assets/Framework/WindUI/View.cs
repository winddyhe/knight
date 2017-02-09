//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using Core;

namespace Framework.WindUI
{
    /// <summary>
    /// 页面，由众多的系统控件以及Widget组成。
    /// PS: 限定View只绑定到Prefab的根节点上。
    /// </summary>
    public class View : MonoBehaviour
    {
        /// <summary>
        /// View的状态，有三种状态
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 固定的
            /// </summary>
            fixing,
            /// <summary>
            /// 叠层
            /// </summary>
            overlap,
            /// <summary>
            /// 可切换的
            /// </summary>
            dispatch,
        }

        /// <summary>
        /// View的实例化GUID，用来唯一标识该View
        /// </summary>
        public string           GUID            = "";

        /// <summary>
        /// 该页面当前的状态
        /// </summary>
        public State            curState        = State.fixing;

        /// <summary>
        /// 是否为Multi的页面
        /// </summary>
        public bool             isMultiView     = false;

        /// <summary>
        /// View的控制器
        /// </summary>
        public IViewController  viewController;

        /// <summary>
        /// 该View是否被打开？
        /// </summary>
        protected bool          isOpened        = false;
        public bool             IsOpened        { get { return isOpened; } set { isOpened = value; } }

        /// <summary>
        /// 该View是否被关掉
        /// </summary>
        protected bool          isClosed        = false;
        public bool             IsClosed        { get { return isClosed; } set { isClosed = value; } }

        /// <summary>
        /// 是否被激活？
        /// </summary>
        public bool             isActived
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); }
        }

        public void Initialize(string rViewGUID, State rViewState)
        {
            this.GUID = rViewGUID;
            this.curState = rViewState;

            // 初始化View controller
            this.InitializeViewController();
        }

        /// <summary>
        /// 初始化View controller
        /// </summary>
        protected virtual void InitializeViewController()
        {
            this.viewController = new ViewController<View>(this);
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted) 
        {
            this.isOpened = false;
    
            this.viewController.OnOpening();
            UIManager.Instance.StartCoroutine(Open_WaitforCompleted(rOpenCompleted));
        }
    
        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while(!this.isOpened)
            {
                yield return 0;
            }
            this.viewController.OnOpened();
    
            UtilTool.SafeExecute(rOpenCompleted, this);
        }

        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            this.viewController.OnShow();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            this.viewController.OnHide();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void Refresh()
        {
            this.viewController.OnRefresh();
        }
    
        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close() 
        {
            this.isClosed = false;
            
            this.viewController.OnClosing();
            UIManager.Instance.StartCoroutine(Close_WaitForCompleted());
        }
    
        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.isClosed)
            {
                yield return 0;
            }
            this.viewController.OnClosed();
        }
    }
}