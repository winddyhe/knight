using Knight.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
            await this.ViewController.Initialize();
        
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
                rMemberBinding.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rMemberBinding.ViewModelPath, out rViewModel);
                if (rMemberBinding.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rMemberBinding.ViewModelPath);
                    return;
                }
                rMemberBinding.SyncFromViewModel();
                
                if (rViewModel != null)
                {
                    // 把ViewModel绑定到ViewController里面
                    var rViewModelProp = this.ViewController.GetType().GetProperties()
                        .Where(prop =>
                        {
                            var rAttrObjs = prop.GetCustomAttributes(typeof(HotfixBindingAttribute), false);
                            if (rAttrObjs == null || rAttrObjs.Length == 0) return false;
                            var rBindingAttr = rAttrObjs[0] as HotfixBindingAttribute;

                            return prop.PropertyType.IsSubclassOf(typeof(ViewModel)) && rBindingAttr != null;
                        }).FirstOrDefault();

                    if (rViewModelProp != null)
                    {
                        rViewModelProp.SetValue(this.ViewController, rViewModel);
                    }
                    else
                    {
                        Debug.LogErrorFormat("ViewModel {0} is not define in ViewController({1})", rViewModel.GetType(), this.ViewController.GetType());
                    }

                    // ViewModel绑定View
                    rMemberBinding.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rMemberBinding.ViewModelProp.PropertyName, () =>
                    {
                        rMemberBinding.SyncFromViewModel();
                    });
                    rViewModel.PropertyChanged += rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
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
