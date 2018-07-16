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
        public GameObject                   RootCanvas;
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
            this.RootCanvas = UIRoot.Instance.DynamicRoot;
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
        public async Task Open(string rViewName, string rViewControllerName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);
            await Open_Async(rViewName, rViewControllerName, rViewState, rOpenCompleted);
        }

        private async Task Open_Async(string rViewName, string rViewControllerName, View.State rViewState, Action<View> rOpenCompleted)
        {
            var rLoaderRequest = await UIAssetLoader.Instance.LoadUI(rViewName);
            await OpenView(rViewName, rViewControllerName, rLoaderRequest.ViewPrefabGo, rViewState, rOpenCompleted);
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
        public async Task OpenView(string rViewName, string rViewControllerName, GameObject rViewPrefab, View.State rViewState, Action<View> rOpenCompleted)
        {
            if (rViewPrefab == null) return;

            //把View的GameObject结点加到rootCanvas下
            GameObject rViewGo = this.RootCanvas.transform.AddChild(rViewPrefab, "UI");

            var rView = View.CreateView(rViewGo);
            if (rView == null)
            {
                Debug.LogErrorFormat("GameObject {0} has not View script.", rViewGo.name);
                UtilTool.SafeExecute(rOpenCompleted, null);
                return;
            }

            string rViewGUID = Guid.NewGuid().ToString();               //生成GUID
            await rView.Initialize(rViewName, rViewControllerName, rViewGUID, rViewState);   //为View的初始化设置

            //新的View的存储逻辑
            switch (rView.CurState)
            {
                case View.State.Fixing:
                    mCurFixedViews.Add(rViewGUID, rView);
                    break;
                case View.State.Overlap:
                    mCurViews.Add(rViewGUID, rView);
                    break;
                case View.State.Dispatch:
                    if (mCurViews.Count == 0)
                        mCurViews.Add(rViewGUID, rView);
                    else
                        mCurViews[mCurViews.Last().Key] = rView;
                    break;
                default:
                    break;
            }

            rView.Open((rNewView) =>
            {
                UtilTool.SafeExecute(rOpenCompleted, rNewView);
            });
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
