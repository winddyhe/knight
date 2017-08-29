using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Core;
using Framework.WindUI;

namespace WindHotfix.GUI
{
    public class UIViewManager : THotfixSingleton<UIViewManager>
    {
        /// <summary>
        /// 存放各种动态节点的地方
        /// </summary>
        public GameObject               RootCanvas;
        /// <summary>
        /// 当前的UI中的Views，每个View是用GUID来作唯一标识
        /// 底层-->顶层 { 0 --> list.count }
        /// </summary>
        private Dict<string, UIView>    mCurViews;
        /// <summary>
        /// 当前存在的固定View，每个View使用GUID来作唯一标识
        /// </summary>
        private Dict<string, UIView>    mCurFixedViews;
        /// <summary>
        /// 用来存储需要删除的View，当一个View加载完之后，要删除当前需要的不再使用的View资源
        /// </summary>
        private List<string>            mUnusedViews;

        private UIViewManager()
        {
        }

        public void Initialize()
        {
            this.RootCanvas = GameObject.Find("UIRoot/RootCanvas/__dynamicRoot");
            this.mCurViews = new Dict<string, UIView>();
            this.mCurFixedViews = new Dict<string, UIView>();
            this.mUnusedViews = new List<string>();
        }

        public void Update()
        {

        }

        /// <summary>
        /// 打开一个View
        /// </summary>
        public void Open(string rViewName, UIView.State rViewState, Action<UIView> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);

            CoroutineManager.Instance.Start(Open_Async(rViewName, rViewState, rOpenCompleted));
        }

        public Coroutine OpenAsync(string rViewName, UIView.State rViewState, Action<UIView> rOpenCompleted = null)
        {
            // 企图关闭当前的View
            Debug.Log("Open " + rViewName);
            MaybeCloseTopView(rViewState);
            return CoroutineManager.Instance.Start(Open_Async(rViewName, rViewState, rOpenCompleted));
        }

        private IEnumerator Open_Async(string rViewName, UIView.State rViewState, Action<UIView> rOpenCompleted)
        {
            var rLoaderRequest = Framework.WindUI.UIAssetLoader.Instance.LoadUI(rViewName);
            yield return rLoaderRequest;

            OpenView(rViewName, rLoaderRequest.ViewPrefabGo, rViewState, rOpenCompleted);
        }

        /// <summary>
        /// 移除顶层View
        /// </summary>
        public void Pop(Action rCloseComplted = null)
        {
            // 得到顶层结点
            KeyValuePair<string, UIView> rTopNode = this.mCurViews.Last();

            string rViewGUID = rTopNode.Key;
            UIView rView = rTopNode.Value;

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
            UIView rView = null;

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
        public void OpenView(string rViewName, GameObject rViewPrefab, UIView.State rViewState, Action<UIView> rOpenCompleted)
        {
            if (rViewPrefab == null) return;

            //把View的GameObject结点加到rootCanvas下
            GameObject rViewGo = this.RootCanvas.transform.AddChild(rViewPrefab, "UI");

            UIView rView = UIView.CreateView(rViewGo);
            if (rView == null)
            {
                Debug.LogErrorFormat("GameObject {0} has not View script.", rViewGo.name);
                UtilTool.SafeExecute(rOpenCompleted, null);
                return;
            }

            //生成GUID
            string rViewGUID = Guid.NewGuid().ToString();
            //为View的初始化设置
            rView.Initialize(rViewName, rViewGUID, rViewState);

            //新的View的存储逻辑
            switch (rView.CurState)
            {
                case UIView.State.fixing:
                    mCurFixedViews.Add(rViewGUID, rView);
                    break;
                case UIView.State.overlap:
                    mCurViews.Add(rViewGUID, rView);
                    break;
                case UIView.State.dispatch:
                    if (mCurViews.Count == 0)
                        mCurViews.Add(rViewGUID, rView);
                    else
                        mCurViews[mCurViews.LastKey()] = rView;
                    break;
                default:
                    break;
            }

            rView.Open((rNewView) =>
            {
                UnloadUnusedViewAssets();
                UtilTool.SafeExecute(rOpenCompleted, rNewView);
            });
        }

        private void UnloadUnusedViewAssets()
        {
            if (this.mUnusedViews == null) return;
            for (int i = 0; i < this.mUnusedViews.Count; i++)
            {
                UIAssetLoader.Instance.UnloadUI(this.mUnusedViews[i]);
            }
            this.mUnusedViews.Clear();
        }

        /// <summary>
        /// 企图关闭一个当前的View，当存在当前View时候，并且传入的View是需要Dispatch的。
        /// </summary>
        private void MaybeCloseTopView(UIView.State rViewState)
        {
            // 得到顶层结点
            KeyValuePair<string, UIView> rTopNode = this.mCurViews.Last();

            string rViewGUID = rTopNode.Key;
            UIView rView = rTopNode.Value;

            if (rView == null) return;

            if (rViewState == UIView.State.dispatch)
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
        private IEnumerator DestroyView_Async(UIView rView, Action rDestroyCompleted = null)
        {
            while (!rView.IsClosed)
            {
                yield return 0;
            }

            mUnusedViews.Add(rView.ViewName);
            rView.Destroy();
            GameObject.DestroyObject(rView.gameObject);
            rView = null;
            UtilTool.SafeExecute(rDestroyCompleted);
        }
    }
}
