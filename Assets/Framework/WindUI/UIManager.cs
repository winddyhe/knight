//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Core;

namespace Framework.WindUI
{
    /// <summary>
    /// UI的管理类
    /// </summary>
    public class UIManager : MonoBehaviour 
    {
        private static UIManager        __instance;
        public  static UIManager        Instance { get { return __instance; } }
    
        /// <summary>
        /// 存放各种动态节点的地方
        /// </summary>
        public GameObject               RootCanvas;
    
        /// <summary>
        /// 当前的UI中的Views，每个View是用GUID来作唯一标识
        /// 底层-->顶层 { 0 --> list.count }
        /// </summary>
        private Dict<string, View>      mCurViews;
        
        /// <summary>
        /// 当前存在的固定View，每个View使用GUID来作唯一标识
        /// </summary>
        private Dict<string, View>      mCurFixedViews;
    
        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
                //跨越场景时不销毁
                GameObject.DontDestroyOnLoad(this.gameObject);
    
                mCurViews = new Dict<string, View>();
                mCurFixedViews = new Dict<string, View>();
            }
        }
        
        /// <summary>
        /// 打开一个View
        /// </summary>
        public void Open(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);

            this.StartCoroutine(Open_Async(rViewName, rViewState, rOpenCompleted));
        }

        public Coroutine OpenAsync(string rViewName, View.State rViewState, Action<View> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);
            return this.StartCoroutine(Open_Async(rViewName, rViewState, rOpenCompleted));
        }

        private IEnumerator Open_Async(string rViewName, View.State rViewState, Action<View> rOpenCompleted)
        {
            var rLoaderRequest = UIAssetLoader.Instance.LoadUI(rViewName);
            yield return rLoaderRequest;
            
            OpenView(rLoaderRequest.ViewPrefabGo, rViewState, rOpenCompleted);
        }
    
        /// <summary>
        /// 移除顶层View
        /// </summary>
        public void Pop(Action rCloseComplted = null)
        {
            // 得到顶层结点
            KeyValuePair<string, View> rTopNode = this.mCurViews.Last();
    
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
            this.StartCoroutine(DestroyView_Async(rView, () => 
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
            this.StartCoroutine(DestroyView_Async(rView, () =>
            {
                UtilTool.SafeExecute(rCloseCompleted);
            })); 
        }
    
        /// <summary>
        /// 初始化View，如果是Dispatch类型的话，只对curViews顶层View进行交换
        /// </summary>
        private void OpenView(GameObject rViewPrefab, View.State rViewState, Action<View> rOpenCompleted)
        {
            if (rViewPrefab == null) return;
    
            //把View的GameObject结点加到rootCanvas下
            GameObject rViewGo = this.RootCanvas.transform.AddChild(rViewPrefab, "UI");
    
            View rView = rViewGo.SafeGetComponent<View>();
            if (rView == null)
            {
                Debug.LogErrorFormat("GameObject {0} has not View script.", rViewGo.name);
                UtilTool.SafeExecute(rOpenCompleted, null);
                return;
            }
    
            //生成GUID
            string rViewGUID = Guid.NewGuid().ToString();
    
            //为View的初始化设置
            rView.Initialize(rViewGUID, rViewState);
    
            //新的View的存储逻辑
            switch (rView.CurState)
            {
                case View.State.fixing:
                    mCurFixedViews.Add(rViewGUID, rView);
                    break;
                case View.State.overlap:
                    mCurViews.Add(rViewGUID, rView);
                    break;
                case View.State.dispatch:
                    if (mCurViews.Count == 0)
                        mCurViews.Add(rViewGUID, rView);
                    else 
                        mCurViews[mCurViews.LastKey()] = rView;
                    break;
                default:
                    break;
            }
    
            rView.Open(rOpenCompleted);
        }
    
        /// <summary>
        /// 企图关闭一个当前的View，当存在当前View时候，并且传入的View是需要Dispatch的。
        /// </summary>
        private void MaybeCloseTopView(View.State rViewState)
        {
            // 得到顶层结点
            KeyValuePair<string, View> rTopNode = this.mCurViews.Last();
    
            string rViewGUID = rTopNode.Key;
            View rView = rTopNode.Value;
    
            if (rView == null) return;
    
            if (rViewState == View.State.dispatch)
            {
                // 移除顶层结点
                this.mCurViews.Remove(rViewGUID);
                rView.Close();
                this.StartCoroutine(DestroyView_Async(rView));
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

            rView.Destroy();
            GameObject.DestroyObject(rView.gameObject);
    
            UtilTool.SafeExecute(rDestroyCompleted);
        }
    }
}


