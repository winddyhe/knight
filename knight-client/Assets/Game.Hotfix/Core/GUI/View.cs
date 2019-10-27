using Knight.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;

namespace Knight.Hotfix.Core
{
    public class View
    {
        public enum State
        {
            Fixing,
            Frame,
            Popup,
            PageSwitch,
            PageOverlap,
        }
        
        public string                   GUID                = "";
        public string                   ViewName            = "";
        public State                    CurState            = State.Fixing;
        public bool                     IsBackCache         = false;

        public GameObject               GameObject;
        
        public ViewControllerContainer  ViewModelContainer;
        public ViewController           ViewController;
        public ViewModel                DefaultViewModel;

        public bool                     IsActived
        {
            get { return this.GameObject.activeSelf;        }
            set { this.GameObject.SetActive(value);         }
        }

        public static View CreateView(GameObject rViewGo)
        {
            if (rViewGo == null) return null;
            View rUIView = new View();
            rUIView.GameObject = rViewGo;
            return rUIView;
        }
        
        public async Task Initialize(string rViewName, string rViewGUID, State rViewState)
        {
            this.ViewName = rViewName;
            this.GUID     = rViewGUID;
            this.CurState = rViewState;
            
            // 初始化ViewController
            await this.InitializeViewModel();
        }
        
        /// <summary>
        /// 初始化ViewController
        /// </summary>
        private async Task InitializeViewModel()
        {
            this.ViewModelContainer = this.GameObject.GetComponent<ViewControllerContainer>();
            if (this.ViewModelContainer == null)
            {
                Debug.LogErrorFormat("Prefab {0} has not ViewContainer Component..", this.ViewName);
                return;
            }

            var rType = Type.GetType(this.ViewModelContainer.ViewControllerClass);
            if (rType == null)
            {
                Debug.LogErrorFormat("Can not find ViewModel Type: {0}", rType);
                return;
            }

            // 构建ViewController
            this.ViewController = HotfixReflectAssists.Construct(rType) as ViewController;
            this.ViewController.View = this;
            this.ViewController.BindingViewModels(this.ViewModelContainer);
            await this.ViewController.Initialize();

            this.ViewController.DataBindingConnect(this.ViewModelContainer);
        }

        /// <summary>
        /// 打开View, 此时View对应的GameObject已经加载出来了, 用于做View的初始化。
        /// </summary>
        public async Task Open()
        {
            await this.ViewController?.Open();
        }
        
        /// <summary>
        /// 显示View
        /// </summary>
        public void Show()
        {
            this.GameObject.SetActive(true);
            this.ViewController?.Show();
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        public void Hide()
        {
            this.GameObject.SetActive(false);
            this.ViewController?.Hide();
        }

        public void Update(float fDeltaTime)
        {
            this.ViewController?.Update(fDeltaTime);
        }
        
        public void Dispose()
        {
            this.ViewController?.DataBindingDisconnect(this.ViewModelContainer);
            this.ViewController?.Dispose();
        }

        /// <summary>
        /// 关闭View
        /// </summary>
        public void Close()
        {
            this.ViewController?.Closing();
        }
    }
}
