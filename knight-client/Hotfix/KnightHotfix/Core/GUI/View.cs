using Knight.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class View
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

        public ViewController       ViewController;
        public List<ViewModelData>  ViewModels;
        
        public bool                 IsOpened
        {
            get { return this.ViewController.IsOpened;  }
            set { this.ViewController.IsOpened = value; }
        }
        
        public bool                 IsClosed
        {
            get { return this.ViewController.IsClosed;  }
            set { this.ViewController.IsClosed = value; }
        }

        public bool                 IsActived
        {
            get { return this.gameObject.activeSelf;    }
            set { this.gameObject.SetActive(value);     }
        }

        public static View CreateView(GameObject rViewGo)
        {
            View rUIView = new View();
            rUIView.gameObject = rViewGo;
            return rUIView;
        }
        
        public async Task Initialize(string rViewName, string rViewControllerName, string rViewGUID, State rViewState)
        {
            this.ViewName = rViewName;
            this.GUID     = rViewGUID;
            this.CurState = rViewState;
            
            // 初始化ViewModel
            this.Initialize_ViewModelDatas();

            // 初始化ViewController
            await this.Initialize_ViewController(rViewControllerName);
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
        /// 初始化ViewController
        /// </summary>
        private async Task Initialize_ViewController(string rViewControllerName)
        {
            var rType = Type.GetType(rViewControllerName);
            if (rType == null)
            {
                Debug.LogErrorFormat("Can not find ViewController Type: {0}", rType);
                return;
            }
            this.ViewController = HotfixReflectAssists.Construct(rType) as ViewController;
            this.ViewController.SetView(this);
            await this.ViewController.Initialize();
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted)
        {
            this.IsOpened = false;
            this.ViewController?.Opening();
            CoroutineManager.Instance.Start(Open_WaitforCompleted(rOpenCompleted));
        }

        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while (!this.IsOpened)
            {
                yield return 0;
            }
            this.ViewController?.Opened();
            UtilTool.SafeExecute(rOpenCompleted, this);
        }
        
        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            this.gameObject.SetActive(true);
            this.ViewController?.Show();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            this.gameObject.SetActive(false);
            this.ViewController?.Hide();
        }

        public void Update()
        {
            this.ViewController?.Update();
        }
        
        public void Dispose()
        {
            this.ViewController?.Dispose();
            this.gameObject = null;
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close()
        {
            this.IsClosed = false;
            this.ViewController?.Closing();
            CoroutineManager.Instance.Start(Close_WaitForCompleted());
        }

        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.IsClosed)
            {
                yield return 0;
            }
            this.ViewController?.Closed();
        }
    }
}
