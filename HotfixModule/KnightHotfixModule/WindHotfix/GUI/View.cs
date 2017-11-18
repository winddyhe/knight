//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.WindUI;
using UnityEngine;
using WindHotfix.Core;

namespace WindHotfix.GUI
{
    public class View
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
        public string               GUID = "";
        /// <summary>
        /// View Name
        /// </summary>
        public string               ViewName = "";
        /// <summary>
        /// 该页面当前的状态
        /// </summary>
        public State                CurState = State.fixing;
        /// <summary>
        /// View的GameObject
        /// </summary>
        public GameObject           gameObject;
        /// <summary>
        /// View脚本
        /// </summary>
        public ViewContainer        ViewMB;
        /// <summary>
        /// View控制器
        /// </summary>
        private ViewController      mViewController;

        public  ViewController      ViewController { get { return mViewController; } }

        /// <summary>
        /// 该View是否被打开？
        /// </summary>
        public bool                 IsOpened
        {
            get
            {
                if (mViewController == null) return false;
                return mViewController.IsOpened;
            }
            set
            {
                if (mViewController == null) return;
                mViewController.IsOpened = value;
            }
        }

        /// <summary>
        /// 该View是否被关掉？
        /// </summary>
        public bool                 IsClosed
        {
            get
            {
                if (mViewController == null) return false;
                return mViewController.IsClosed;
            }
            set
            {
                if (mViewController == null) return;
                mViewController.IsClosed = value;
            }
        }

        /// <summary>
        /// 是否被激活？
        /// </summary>
        public bool                 IsActived
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); }
        }

        public static View CreateView(GameObject rViewGo)
        {
            ViewContainer rViewDataMB = rViewGo.GetComponent<ViewContainer>();
            if (rViewDataMB == null) return null;

            View rUIView = new View();
            rUIView.gameObject = rViewGo;
            rUIView.ViewMB = rViewDataMB;
            return rUIView;
        }

        public void Initialize(string rViewName, string rViewGUID, State rViewState)
        {
            this.ViewName = rViewName;
            this.GUID = rViewGUID;
            this.CurState = rViewState;

            this.ViewMB.GUID = rViewGUID;
            this.ViewMB.ViewName = rViewName;
            this.ViewMB.CurState = (ViewContainer.State)rViewState;

            // 初始化View controller
            this.InitializeViewController();
        }

        /// <summary>
        /// 初始化View controller
        /// </summary>
        protected virtual void InitializeViewController()
        {
            this.mViewController = HotfixReflectAssists.Construct(Type.GetType(this.ViewMB.HotfixName)) as ViewController;
            if (this.mViewController == null)
            {
                Debug.LogErrorFormat("Create View controller <color=blue>{0}</color> failed..", this.ViewMB.HotfixName);
            }
            else
            {
                this.mViewController.SetHotfix(this.ViewMB.HotfixName, this.GUID);
                this.mViewController.Initialize(this.ViewMB.Objects);
            }
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted)
        {
            this.IsOpened = false;

            if (mViewController != null)
                mViewController.Opening();

            CoroutineManager.Instance.Start(Open_WaitforCompleted(rOpenCompleted));
        }

        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while (!this.IsOpened)
            {
                yield return 0;
            }

            if (mViewController != null)
                mViewController.OnOpened();

            UtilTool.SafeExecute(rOpenCompleted, this);
        }

        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            if (mViewController != null)
                mViewController.OnShow();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            if (mViewController != null)
                mViewController.OnHide();
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (mViewController != null)
                mViewController.OnUpdate();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void Refresh()
        {
            if (mViewController != null)
                mViewController.OnRefresh();
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close()
        {
            this.IsClosed = false;

            if (mViewController != null)
                mViewController.Closing();

            CoroutineManager.Instance.Start(Close_WaitForCompleted());
        }

        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.IsClosed)
            {
                yield return 0;
            }

            if (mViewController != null)
                mViewController.Closed();
        }

        public void Destroy()
        {
            // 销毁引用
            mViewController = null;
        }
    }
}
