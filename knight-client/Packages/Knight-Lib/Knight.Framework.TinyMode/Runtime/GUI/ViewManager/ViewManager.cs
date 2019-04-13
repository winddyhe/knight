//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI
{
    public class ViewManager : TSingleton<ViewManager>
    {
        /// <summary>
        /// 存放各种动态节点的地方
        /// </summary>
        public GameObject                   DynamicRoot;
        /// <summary>
        /// 存放各种弹出框节点的地方
        /// </summary>
        public GameObject                   PopupRoot;
        /// <summary>
        /// 存放各种Page页面的地方
        /// </summary>
        public GameObject                   FrameRoot;
        public GameObject                   PageRoot;

        /// <summary>
        /// 当前的UI中的Views，每个View是用GUID来作唯一标识
        /// 底层-->顶层 { 0 --> list.count }
        /// </summary>
        private IndexedDict<string, View>   mCurFrameViews;
        private IndexedDict<string, View>   mCurPageViews;
        /// <summary>
        /// 当前存在的固定View，每个View使用GUID来作唯一标识
        /// </summary>
        private IndexedDict<string, View>   mCurFixedViews;
        
        private ViewManager()
        {
        }

        public void Initialize()
        {
            // 初始化这个Page节点
            this.FrameRoot = UIRoot.Instance.DynamicRoot;
            this.PageRoot = UIRoot.Instance.DynamicRoot;

            this.DynamicRoot = UIRoot.Instance.DynamicRoot;
            this.PopupRoot = UIRoot.Instance.PopupRoot;

            this.mCurFrameViews = new IndexedDict<string, View>();
            this.mCurPageViews  = new IndexedDict<string, View>();
            this.mCurFixedViews = new IndexedDict<string, View>();
        }

        public void Update()
        {
            if (this.mCurFixedViews != null)
            {
                var rCurViewKeys = mCurFixedViews.Keys;
                for (int i = 0; i < mCurFixedViews.Count; i++)
                {
                    this.mCurFixedViews[rCurViewKeys[i]].Update();
                }
            }
            if (this.mCurFrameViews != null)
            {
                var rCurViewKeys = mCurFrameViews.Keys;
                for (int i = 0; i < rCurViewKeys.Count; i++)
                {
                    this.mCurFrameViews[rCurViewKeys[i]].Update();
                }
            }
            if (this.mCurPageViews != null)
            {
                var rCurViewKeys = mCurPageViews.Keys;
                for (int i = 0; i < rCurViewKeys.Count; i++)
                {
                    this.mCurPageViews[rCurViewKeys[i]].Update();
                }
            }
        }

        public void Show(string rViewGUID)
        {
            View rView = null;
            if (this.mCurFixedViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Show();
            }
            if (this.mCurFrameViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Show();
            }
            if (this.mCurPageViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Show();
            }
        }
        
        public void Hide(string rViewGUID)
        {
            View rView = null;
            if (this.mCurFixedViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Hide();
            }
            if (this.mCurFrameViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Hide();
            }
            if (this.mCurPageViews.TryGetValue(rViewGUID, out rView))
            {
                rView.Hide();
            }
        }

        /// <summary>
        /// 打开一个View
        /// </summary>
        public async Task<View> OpenAsync(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("-- Open UI View: " + rViewName);
            MaybeCloseTopView(rViewState);
            return await OpenViewAsync(rViewName, rViewState, rOpenCompleted);
        }

        public void Open(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
#pragma warning disable 4014
            this.OpenAsync(rViewName, rViewState, rOpenCompleted);
#pragma warning restore 4014
        }

        private async Task<View> OpenViewAsync(string rViewName, View.State rViewState, Action<View> rOpenCompleted)
        {
            var rLoaderRequest = await UIAssetLoader.Instance.LoadUI(rViewName);
            return await OpenView(rViewName, rLoaderRequest.ViewPrefabGo, rViewState, rOpenCompleted);
        }
        
        /// <summary>
        /// 移除顶层View
        /// </summary>
        public void Pop(Action rCloseComplted = null)
        {
            // 得到顶层结点
            CKeyValuePair<string, View> rTopNode = this.mCurPageViews.Last();
            if (rTopNode == null)
            {
                UtilTool.SafeExecute(rCloseComplted);
                return;
            }

            string rViewGUID = rTopNode.Key;
            View rView = rTopNode.Value;
            if (rView == null)
            {
                UtilTool.SafeExecute(rCloseComplted);
                return;
            }

            // 移除顶层结点
            this.mCurPageViews.Remove(rViewGUID);
            rView.Close();
            DestroyView(rView);
            UtilTool.SafeExecute(rCloseComplted);
        }

        public void CloseAllPageViews()
        {
            var rViewKeys = this.mCurPageViews.Keys;
            for (int i = 0; i < rViewKeys.Count; i++)
            {
                this.CloseView(rViewKeys[i]);
            }
        }

        /// <summary>
        /// 根据GUID来关闭指定的View
        /// </summary>
        public void CloseView(string rViewGUID, Action rCloseCompleted = null)
        {
            bool isFixedView = false;
            View rView = null;

            // 找到View
            if (this.mCurFixedViews.TryGetValue(rViewGUID, out rView))
            {
                isFixedView = true;
            }
            else if (this.mCurFrameViews.TryGetValue(rViewGUID, out rView))
            {
                isFixedView = false;
            }
            else if (this.mCurPageViews.TryGetValue(rViewGUID, out rView))
            {
                isFixedView = false;
            }

            // View不存在
            if (rView == null)
            {
                UtilTool.SafeExecute(rCloseCompleted);
                return;
            }

            // View存在
            if (isFixedView)
            {
                this.mCurFixedViews.Remove(rViewGUID);
            }
            else
            {
                if (rView.CurState == View.State.Frame)
                    this.mCurFrameViews.Remove(rViewGUID);
                else if (rView.CurState == View.State.PageSwitch || rView.CurState == View.State.PageSwitch)
                    this.mCurPageViews.Remove(rViewGUID);
            }

            // 移除顶层结点
            rView.Close();

            DestroyView(rView);
            UtilTool.SafeExecute(rCloseCompleted);
        }

        /// <summary>
        /// 初始化View，如果是Dispatch类型的话，只对curViews顶层View进行交换
        /// </summary>
        public async Task<View> OpenView(string rViewName, GameObject rViewPrefab, View.State rViewState, Action<View> rOpenCompleted)
        {
            if (rViewPrefab == null) return null;

            //把View的GameObject结点加到rootCanvas下
            GameObject rViewGo = null;
            switch (rViewState)
            {
                case View.State.Fixing:
                    rViewGo = this.DynamicRoot.transform.AddChildNoScale(rViewPrefab, "UI");
                    break;
                case View.State.Popup:
                    rViewGo = this.PopupRoot.transform.AddChildNoScale(rViewPrefab, "UI");
                    break;
                case View.State.Frame:
                    rViewGo = this.FrameRoot.transform.AddChildNoScale(rViewPrefab, "UI");
                    break;
                case View.State.PageSwitch:
                case View.State.PageOverlap:
                    rViewGo = this.PageRoot.transform.AddChildNoScale(rViewPrefab, "UI");
                    break;
            }

            var rView = View.CreateView(rViewGo);
            string rViewGUID = Guid.NewGuid().ToString();   //生成GUID
            if (rView == null)
            {
                Debug.LogErrorFormat("GameObject {0} is null.", rViewGo.name);
                UtilTool.SafeExecute(rOpenCompleted, null);
                return null;
            }

            //新的View的存储逻辑
            switch (rViewState)
            {
                case View.State.Fixing:
                    mCurFixedViews.Add(rViewGUID, rView);
                    break;
                case View.State.Frame:
                    mCurFrameViews.Add(rViewGUID, rView);
                    break;
                case View.State.PageSwitch:
                    if (mCurPageViews.Count == 0)
                    {
                        mCurPageViews.Add(rViewGUID, rView);
                    }
                    else
                    {
                        var rTopNode = this.mCurPageViews.Last();
                        mCurPageViews.Remove(rTopNode.Key);
                        mCurPageViews.Add(rViewGUID, rView);
                    }
                    break;
                case View.State.PageOverlap:
                    mCurPageViews.Add(rViewGUID, rView);
                    break;
                case View.State.Popup:
                    mCurPageViews.Add(rViewGUID, rView);
                    break;
                default:
                    break;
            }

            try
            {
                await rView.Initialize(rViewName, rViewGUID, rViewState);   //为View的初始化设置
                await rView.Open();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
            UtilTool.SafeExecute(rOpenCompleted, rView);
            
            return rView;
        }

        /// <summary>
        /// 企图关闭一个当前的View，当存在当前View时候，并且传入的View是需要Dispatch的。
        /// </summary>
        private void MaybeCloseTopView(View.State rViewState)
        {
            // 得到顶层结点
            CKeyValuePair<string, View> rTopNode = null;
            if (this.mCurPageViews.Count > 0)
                rTopNode = this.mCurPageViews.Last();
            if (rTopNode == null) return;

            string rViewGUID = rTopNode.Key;
            View rView = rTopNode.Value;
            if (rView == null) return;

            if (rViewState == View.State.PageSwitch)
            {
                // 移除顶层结点
                this.mCurPageViews.Remove(rViewGUID);
                rView.Close();
                DestroyView(rView);
            }
        }

        /// <summary>
        /// 等待View关闭动画播放完后开始删除一个View
        /// </summary>
        private void DestroyView(View rView)
        {
            rView.Dispose();
            UtilTool.SafeDestroy(rView.GameObject);
            rView = null;
        }
    }
}
