using Knight.Core;
using Knight.Hotfix.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Hotfix.DataBinding
{
    public class View : HotfixKnightObject
    {
        public enum State
        {
            fixing,
            overlap,
            dispatch,
        }
        
        public string               GUID        = "";
        public string               ViewName    = "";

        public State                CurState    = State.fixing;
        public GameObject           gameObject;

        public bool                 IsOpened;
        public bool                 IsClosed;

        public List<ViewModelData>  ViewModels;
        
        public bool                 IsActived
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value);  }
        }

        public static View CreateView(GameObject rViewGo)
        {
            View rUIView = new View();
            rUIView.gameObject = rViewGo;
            return rUIView;
        }
        
        public void InitializeView(string rViewName, string rViewGUID, State rViewState)
        {
            this.ViewName = rViewName;
            this.GUID     = rViewGUID;
            this.CurState = rViewState;
            
            // 初始化ViewModel
            this.Initialize_ViewModelDatas();
        }

        /// <summary>
        /// 初始化ViewModel
        /// </summary>
        private void Initialize_ViewModelDatas()
        {
            this.ViewModels = new List<ViewModelData>();
            var rAllDataSources = this.gameObject.GetComponentsInChildren<DataSourceModel>(true);
            for (int i = 0; i < rAllDataSources.Length; i++)
            {
                var rClassName = rAllDataSources[i].ViewModelClass;
                if (string.IsNullOrEmpty(rClassName)) continue;

                var rType = Type.GetType(rClassName);
                if (rType == null) continue;

                var rViewModel = HotfixReflectAssists.Construct(rType) as ViewModel;
                this.ViewModels.Add(new ViewModelData() { DataSource = rAllDataSources[i], ViewModel = rViewModel });
            }
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted)
        {
            this.IsOpened = false;
            this.Opening();
            CoroutineManager.Instance.Start(Open_WaitforCompleted(rOpenCompleted));
        }

        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while (!this.IsOpened)
            {
                yield return 0;
            }
            this.OnOpened();
            UtilTool.SafeExecute(rOpenCompleted, this);
        }

        private void Opening()
        {
            this.IsOpened = true;
            this.OnOpening();
        }

        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            this.OnShow();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            this.OnHide();
        }
        
        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close()
        {
            this.IsClosed = false;
            this.Closing();
            CoroutineManager.Instance.Start(Close_WaitForCompleted());
        }

        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.IsClosed)
            {
                yield return 0;
            }
            this.OnClosed();
        }

        private void Closing()
        {
            this.IsClosed = true;
            this.OnClosing();
        }

        protected virtual void OnOpening()
        {
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected override void OnUpdate()
        {
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void OnClosed()
        {
        }

        protected override void OnDispose()
        {
            this.gameObject = null;
        }
    }
}
