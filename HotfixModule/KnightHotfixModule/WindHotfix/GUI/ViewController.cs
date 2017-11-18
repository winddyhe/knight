//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindHotfix.GUI
{
    public class ViewController
    {
        public string HotfixName;
        public string GUID;
        public string ParentType = "WindHotfix.GUI.THotfixViewController`1";

        public ViewController()
        {
        }

        public void SetHotfix(string rHotfixName, string rGUID)
        {
            this.HotfixName = rHotfixName;
            this.GUID = rGUID;
            this.ParentType = string.Format("WindHotfix.GUI.THotfixViewController`1<{0}>", rHotfixName);
        }

        public virtual void Initialize(List<UnityObject> rObjs)
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

        public virtual void OnUpdate()
        {
        }
    }
}
