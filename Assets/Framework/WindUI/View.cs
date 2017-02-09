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
        /// View的数据
        /// </summary>
        public ViewData         ViewData;

        /// <summary>
        /// View的控制器
        /// </summary>
        public ViewController   ViewController;

        public void Initialize(string rViewGUID, ViewData.State rViewState)
        {
            Type rViewDataType = Type.GetType();
            this.ViewData = ReflectionAssist.Construct();

            this.ViewData.GUID = rViewGUID;
            this.ViewData.CurState = rViewState;

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