//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;
using Core;
using Framework.Hotfix;

namespace Framework.WindUI
{
    /// <summary>
    /// 页面，由众多的系统控件以及Widget组成。
    /// PS: 限定View只绑定到Prefab的根节点上。
    /// @TODO: 将变量 GUID CurState IsMultiView 定义到ViewController中，同时又显示到Inspector中去
    /// </summary>
    public class ViewContainer : HotfixMBContainer
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
        /// View Name
        /// </summary>
        public string           ViewName        = "";
        /// <summary>
        /// 该页面当前的状态
        /// </summary>
        public State            CurState        = State.fixing;
        /// <summary>
        /// 是否为Multi的页面
        /// </summary>
        public bool             IsMultiView     = false;

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