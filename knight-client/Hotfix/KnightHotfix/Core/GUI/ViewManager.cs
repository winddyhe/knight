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

namespace Knight.Hotfix.Core
{
    public class ViewManager : THotfixSingleton<ViewManager>
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
        public GameObject                   PageRoot;

        /// <summary>
        /// 当前的UI中的Views，每个View是用GUID来作唯一标识
        /// 底层-->顶层 { 0 --> list.count }
        /// </summary>
        private IndexedDict<string, View>   mCurViews;
        /// <summary>
        /// 当前存在的固定View，每个View使用GUID来作唯一标识
        /// </summary>
        private Dict<string, View>          mCurFixedViews;
        
        private ViewManager()
        {
        }

        public void Initialize()
        {
            this.DynamicRoot = UIRoot.Instance.DynamicRoot;
            this.PopupRoot = UIRoot.Instance.PopupRoot;

            this.mCurViews = new IndexedDict<string, View>();
            this.mCurFixedViews = new Dict<string, View>();
        }

        public void Update()
        {
            if (this.mCurViews == null) return;

            var rCurViewKeys = mCurViews.Keys;
            for (int i = 0; i < rCurViewKeys.Count; i++)
            {
                this.mCurViews[rCurViewKeys[i]].Update();
            }
        }

        /// <summary>
        /// 打开一个View
        /// </summary>
        public async Task<View> OpenAsync(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);
            return await Open_Async(rViewName, rViewState, rOpenCompleted);
        }

        public void Open(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
#pragma warning disable 4014
            this.OpenAsync(rViewName, rViewState, rOpenCompleted);
#pragma warning restore 4014
        }

        private async Task<View> Open_Async(string rViewName, View.State rViewState, Action<View> rOpenCompleted)
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
            CKeyValuePair<string, View> rTopNode = this.mCurViews.Last();

            string rViewGUID = rTopNode.Key;
            View rView = rTopNode.Value;

            if (rView == null)
            {
                UtilTool.SafeExecute(rCloseComplted);
                return;
            }

            // 移除顶层结点
            this.mCurViews.Remove(rViewGUID);
            rView.Close();
            CoroutineManager.Instance.Start(DestroyView_Async(rView, () =>
            {
                UtilTool.SafeExecute(rCloseComplted);
            }));
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
            else if (this.mCurViews.TryGetValue(rViewGUID, out rView))
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
                this.mCurViews.Remove(rViewGUID);
            }

            // 移除顶层结点
            rView.Close();
            CoroutineManager.Instance.Start(DestroyView_Async(rView, () =>
            {
                UtilTool.SafeExecute(rCloseCompleted);
            }));
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
                case View.State.Dispatch:
                    rViewGo = this.DynamicRoot.transform.AddChild(rViewPrefab, "UI");
                    break;
                case View.State.Popup:
                    rViewGo = this.PopupRoot.transform.AddChild(rViewPrefab, "UI");
                    break;
                case View.State.Page:
                    rViewGo = this.PageRoot.transform.AddChild(rViewPrefab, "UI");
                    break;
            }

            var rView = View.CreateView(rViewGo);
            if (rView == null)
            {
                Debug.LogErrorFormat("GameObject {0} is null.", rViewGo.name);
                UtilTool.SafeExecute(rOpenCompleted, null);
                return null;
            }

            string rViewGUID = Guid.NewGuid().ToString();               //生成GUID
            await rView.Initialize(rViewName, rViewGUID, rViewState);   //为View的初始化设置

            //新的View的存储逻辑
            switch (rView.CurState)
            {
                case View.State.Fixing:
                    mCurFixedViews.Add(rViewGUID, rView);
                    break;
                case View.State.Dispatch:
                case View.State.Page:
                    if (mCurViews.Count == 0)
                        mCurViews.Add(rViewGUID, rView);
                    else
                        mCurViews[mCurViews.Last().Key] = rView;
                    break;
                case View.State.Popup:
                    mCurViews.Add(rViewGUID, rView);
                    break;
                default:
                    break;
            }

            rView.Open((rNewView) =>
            {
                UtilTool.SafeExecute(rOpenCompleted, rNewView);
            });

            return rView;
        }

        /// <summary>
        /// 企图关闭一个当前的View，当存在当前View时候，并且传入的View是需要Dispatch的。
        /// </summary>
        private void MaybeCloseTopView(View.State rViewState)
        {
            // 得到顶层结点
            CKeyValuePair<string, View> rTopNode = null;
            if (this.mCurViews.Count > 0)
                rTopNode = this.mCurViews.Last();

            if (rTopNode == null) return;

            string rViewGUID = rTopNode.Key;
            View rView = rTopNode.Value;

            if (rView == null) return;

            if (rViewState == View.State.Dispatch)
            {
                // 移除顶层结点
                this.mCurViews.Remove(rViewGUID);
                rView.Close();
                CoroutineManager.Instance.Start(DestroyView_Async(rView));
            }
        }

        /// <summary>
        /// 等待View关闭动画播放完后开始删除一个View
        /// </summary>
        private IEnumerator DestroyView_Async(View rView, Action rDestroyCompleted = null)
        {
            while (!rView.IsClosed)
            {
                yield return 0;
            }

            rView.Dispose();
            UtilTool.SafeDestroy(rView.GameObject);
            rView = null;
            UtilTool.SafeExecute(rDestroyCompleted);
        }
    }
}
