//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using Core;
using Framework.Hotfix;
using Game.Knight;

namespace Framework.WindUI
{
    /// <summary>
    /// 页面，由众多的系统控件以及Widget组成。
    /// PS: 限定View只绑定到Prefab的根节点上。
    /// @TODO: 将变量 GUID CurState IsMultiView 定义到ViewController中，同时又显示到Inspector中去
    /// </summary>
    public class View : HotfixMBContainer
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
        public State            CurState        = State.fixing;
        /// <summary>
        /// 是否为Multi的页面
        /// </summary>
        public bool             IsMultiView     = false;
        /// <summary>
        /// View控制器
        /// </summary>
        private ViewController  mViewController;
        public  ViewController  ViewController { get { return mViewController; } }
        
        /// <summary>
        /// 该View是否被打开？
        /// </summary>
        public bool             IsOpened
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
        public bool             IsClosed
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
        public bool             IsActived
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value);  }
        }

        public void Initialize(string rViewGUID, State rViewState)
        {
            this.GUID = rViewGUID;
            this.CurState = rViewState;

            // 初始化View controller
            this.InitializeViewController();
        }

        /// <summary>
        /// 初始化View controller
        /// </summary>
        protected virtual void InitializeViewController()
        {
            this.mViewController = HotfixApp.Instance.Instantiate<ViewController>(this.mHotfixName);
            if (this.mViewController == null)
            {
                Debug.LogErrorFormat("Create View controller <color=red>{0}</color> failed..", this.mHotfixName);
            }
            else
            {
                this.mViewController.Initialize(this.mObjects, this.ToBaseDataObjects(this.mBaseDatas));
            }
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted) 
        {
            this.IsOpened = false;

            if (mViewController != null)
                mViewController.OnOpening();
            
            UIManager.Instance.StartCoroutine(Open_WaitforCompleted(rOpenCompleted));
        }
    
        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while(!this.IsOpened)
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
                mViewController.OnClosing();

            UIManager.Instance.StartCoroutine(Close_WaitForCompleted());
        }
    
        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.IsClosed)
            {
                yield return 0;
            }

            if (mViewController != null)
                mViewController.OnClosed();
        }

        public void Destroy()
        {
            // 销毁引用
            base.OnDestroy();
            mViewController = null;
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mViewController != null)
                mViewController.OnUnityEvent(rTarget);
        }

        /// <summary>
        /// 在View中不推荐使用MonoBehaviour本身的生命周期，还是自己控制比较好
        /// 能够避免隔帧调用顺序、Deactive GameObject之后方法不响应等问题
        /// </summary>
        #region __MonoBehaviourContainer__
        protected override void Awake()
        {
        }
        protected override void Start()
        {
        }
        protected override void OnDestroy()
        {
        }
        protected override void OnEnable()
        {
        }
        protected override void OnDisable()
        {
        }
        protected override void Update()
        {
        }
        #endregion
    }
}