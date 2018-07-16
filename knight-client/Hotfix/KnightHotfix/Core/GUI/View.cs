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
            Fixing,
            Overlap,
            Dispatch,
        }
        
        public string               GUID                = "";
        public string               ViewName            = "";
        public State                CurState            = State.Fixing;
        public GameObject           GameObject;
        
        public ViewModel            ViewModel;
        
        public bool                 IsOpened
        {
            get { return this.ViewModel.IsOpened;       }
            set { this.ViewModel.IsOpened = value;      }
        }
        
        public bool                 IsClosed
        {
            get { return this.ViewModel.IsClosed;       }
            set { this.ViewModel.IsClosed = value;      }
        }

        public bool                 IsActived
        {
            get { return this.GameObject.activeSelf;    }
            set { this.GameObject.SetActive(value);     }
        }

        public static View CreateView(GameObject rViewGo)
        {
            View rUIView = new View();
            rUIView.GameObject = rViewGo;
            return rUIView;
        }
        
        public async Task Initialize(string rViewName, string rViewControllerName, string rViewGUID, State rViewState)
        {
            this.ViewName = rViewName;
            this.GUID     = rViewGUID;
            this.CurState = rViewState;
            
            // 初始化ViewController
            await this.InitializeViewModel(rViewControllerName);

            // DataBinding
            this.DataBinding();
        }
        
        /// <summary>
        /// 初始化ViewController
        /// </summary>
        private async Task InitializeViewModel(string rViewControllerName)
        {
            var rType = Type.GetType(rViewControllerName);
            if (rType == null)
            {
                Debug.LogErrorFormat("Can not find ViewModel Type: {0}", rType);
                return;
            }
            this.ViewModel = HotfixReflectAssists.Construct(rType) as ViewModel;
            this.ViewModel.SetView(this);
            await this.ViewModel.Initialize();
        }

        private void DataBinding()
        {
            var rAllDataBindings = this.GameObject.GetComponentsInChildren<DataBindingOneWay>(true);
            for (int i = 0; i < rAllDataBindings.Length; i++)
            {
                var rDataBinding = rAllDataBindings[i];

                var rModelData = rDataBinding.CurModelData;
                this.ModelPropertyChanged(rModelData.VaribleName, rDataBinding);
                
                rAllDataBindings[i].ModelPropertyChanged = (rPropName) => { this.ModelPropertyChanged(rPropName, rDataBinding); };
                this.ViewModel.PropertyChanged += rDataBinding.ModelPropertyChanged;
            }
        }

        private void ModelPropertyChanged(string rPropName, DataBindingOneWay rDataBindingOneWay)
        {
            var rModelType = this.ViewModel.GetType();
            var rModelProp = rModelType.GetProperty(rPropName, HotfixReflectAssists.flags_public);
            object rModelValue = null;
            if (rModelProp != null)
            {
                rModelValue = rModelProp.GetValue(this.ViewModel);
            }
            rDataBindingOneWay.SetViewData(rModelValue);
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public void Open(Action<View> rOpenCompleted)
        {
            this.IsOpened = false;
            this.ViewModel?.Opening();
            CoroutineManager.Instance.Start(Open_WaitforCompleted(rOpenCompleted));
        }

        private IEnumerator Open_WaitforCompleted(Action<View> rOpenCompleted)
        {
            while (!this.IsOpened)
            {
                yield return 0;
            }
            this.ViewModel?.Opened();
            UtilTool.SafeExecute(rOpenCompleted, this);
        }
        
        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            this.GameObject.SetActive(true);
            this.ViewModel?.Show();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            this.GameObject.SetActive(false);
            this.ViewModel?.Hide();
        }

        public void Update()
        {
            this.ViewModel?.Update();
        }
        
        public void Dispose()
        {
            var rAllDataBindings = this.GameObject.GetComponentsInChildren<DataBindingOneWay>(true);
            for (int i = 0; i < rAllDataBindings.Length; i++)
            {
                this.ViewModel.PropertyChanged -= rAllDataBindings[i].ModelPropertyChanged;
            }
            this.ViewModel?.Dispose();
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close()
        {
            this.IsClosed = false;
            this.ViewModel?.Closing();
            CoroutineManager.Instance.Start(Close_WaitForCompleted());
        }

        private IEnumerator Close_WaitForCompleted()
        {
            while (!this.IsClosed)
            {
                yield return 0;
            }
            this.ViewModel?.Closed();
        }
    }
}
