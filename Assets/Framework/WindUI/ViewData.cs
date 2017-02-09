using UnityEngine;
using System.Collections;

namespace Framework.WindUI
{
    /// <summary>
    /// View的数据，所有属性相关的东西都放在里面
    /// </summary>
    [System.Serializable]
    public class ViewData
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
        /// View的根节点对象
        /// </summary>
        public GameObject       GameObject;

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
        public bool             IsActived
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); }
        }
    }
}
