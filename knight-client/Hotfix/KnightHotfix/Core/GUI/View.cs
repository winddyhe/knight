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
            Overlap,
            Dispatch,
        }
        
        public string               GUID                = "";
        public string               ViewName            = "";
        public State                CurState            = State.Fixing;
        public GameObject           GameObject;
        
        public ViewModelContainer   ViewModelContainer;
        public ViewController       ViewController;
        
        public bool                 IsOpened
        {
            get { return this.ViewController.IsOpened;      }
            set { this.ViewController.IsOpened = value;     }
        }
        
        public bool                 IsClosed
        {
            get { return this.ViewController.IsClosed;      }
            set { this.ViewController.IsClosed = value;     }
        }

        public bool                 IsActived
        {
            get { return this.GameObject.activeSelf;        }
            set { this.GameObject.SetActive(value);         }
        }

        public static View CreateView(GameObject rViewGo)
        {
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
            this.ViewModelContainer = this.GameObject.GetComponent<ViewModelContainer>();
            if (this.ViewModelContainer == null)
            {
                Debug.LogErrorFormat("Prefab {0} has not ViewContainer Component..", this.ViewName);
                return;
            }

            var rType = Type.GetType(this.ViewModelContainer.ViewModelClass);
            if (rType == null)
            {
                Debug.LogErrorFormat("Can not find ViewModel Type: {0}", rType);
                return;
            }

            // 构建ViewController
            this.ViewController = HotfixReflectAssists.Construct(rType) as ViewController;
            // 把ViewModel绑定到ViewController里面
            for (int i = 0; i < this.ViewModelContainer.ViewModels.Count; i++)
            {
                var rViewModelDataSource = this.ViewModelContainer.ViewModels[i];
                ViewModel rViewModel = null;
                Type rViewModelType = Type.GetType(rViewModelDataSource.ViewModelPath);
                if (rViewModelType != null)
                {
                    rViewModel = HotfixReflectAssists.Construct(rViewModelType) as ViewModel;
                }
                if (rViewModel != null)
                {
                    this.ViewController.AddViewModel(rViewModelDataSource.Key, rViewModel);
                }
                else
                {
                    Debug.LogErrorFormat("Can not find ViewModel {0}.", rViewModelDataSource.ViewModelPath);
                }
            }
            await this.ViewController.Initialize();

            // 把Event绑定到ViewController里面
            for (int i = 0; i < this.ViewModelContainer.EventBindings.Count; i++)
            {
                var rEventBinding = this.ViewModelContainer.EventBindings[i];
                var bResult = HotfixDataBindingTypeResolve.MakeViewModelDataBindingEvent(this.ViewController, rEventBinding);
                if (!bResult)
                {
                    Debug.LogErrorFormat("Make view model binding event {0} failed..", rEventBinding.ViewModelMethod);
                }
            }

            // ViewModel和View之间的数据绑定
            this.DataBindingConnect();
        }
        
        private void DataBindingConnect()
        {
            var rAllMemberBindings = this.GameObject.GetComponentsInChildren<MemberBindingAbstract>(true);
            for (int i = 0; i < rAllMemberBindings.Length; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];

                rMemberBinding.ViewProp = DataBindingTypeResolve.MakeViewDataBindingProperty(rMemberBinding.gameObject, rMemberBinding.ViewPath);
                if (rMemberBinding.ViewProp == null)
                {
                    Debug.LogErrorFormat("View Path: {0} error..", rMemberBinding.ViewPath);
                    return;
                }

                ViewModel rViewModel = null;
                rMemberBinding.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rMemberBinding.ViewModelPath, this.ViewController, out rViewModel);
                if (rMemberBinding.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rMemberBinding.ViewModelPath);
                    return;
                }
                rMemberBinding.SyncFromViewModel();
                
                if (rViewModel != null)
                {
                    // ViewModel绑定View
                    rMemberBinding.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rMemberBinding.ViewModelProp.PropertyName, () =>
                    {
                        rMemberBinding.SyncFromViewModel();
                    });
                    rViewModel.PropertyChanged += rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                }

                // View绑定ViewModel
                var rMemberBindingTwoWay = rMemberBinding as MemberBindingTwoWay;
                if (rMemberBindingTwoWay != null)
                {
                    rMemberBindingTwoWay.InitEventWatcher();
                }
            }
        }

        private void DataBindingDisconnect()
        {
            var rAllMemberBindings = this.GameObject.GetComponentsInChildren<MemberBindingAbstract>(true);
            for (int i = 0; i < rAllMemberBindings.Length; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];
                if (rMemberBinding.ViewModelProp == null) continue;

                ViewModel rViewModel = rMemberBinding.ViewModelProp.PropertyOwner as ViewModel;
                if (rViewModel != null)
                {
                    rViewModel.PropertyChanged -= rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                }
                rMemberBinding.OnDestroy();
            }

            var rAllEventBindings = this.GameObject.GetComponentsInChildren<EventBinding>(true);
            for (int i = 0; i < rAllEventBindings.Length; i++)
            {
                rAllEventBindings[i].OnDestroy();
            }
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

        public void Update()
        {
            this.ViewController?.Update();
        }
        
        public void Dispose()
        {
            this.DataBindingDisconnect();
            this.ViewController?.Dispose();
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
