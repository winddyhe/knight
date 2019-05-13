using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Knight.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class ViewController : HotfixKnightObject
    {
        public    View                      View;
        protected Dict<string, ViewModel>   ViewModels;

        public ViewController()
        {
            this.ViewModels = new Dict<string, ViewModel>();
        }

        public string GUID
        {
            get
            {
                if (this.View == null) return "";
                return this.View.GUID;
            }
        }

        public void DataBindingConnect(ViewControllerContainer rViewControllerContainer)
        {
            if (rViewControllerContainer == null) return;
            
            // 把Event绑定到ViewController里面
            this.BindingEvents(rViewControllerContainer);
            
            // ViewModel和View之间的数据绑定
            this.BindingViewAndViewModels(rViewControllerContainer);
            
            // ListViewModel和View之间的数据绑定
            this.BindingListViewAndViewModels(rViewControllerContainer);
            
            // TabViewModel和View之间的数据绑定
            this.BindingTabViewAndViewModels(rViewControllerContainer);
            
            // ArrayViewModel和View之间的数据绑定
            this.BindingArrayViewAndViewModels(rViewControllerContainer);
        }

        public void DataBindingDisconnect(ViewControllerContainer rViewControllerContainer)
        {
            if (!rViewControllerContainer) return;

            var rAllMemberBindings = rViewControllerContainer.gameObject.GetComponentsInChildren<MemberBindingAbstract>(true);
            for (int i = 0; i < rAllMemberBindings.Length; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];
                if (rMemberBinding.ViewModelProp == null) continue;

                ViewModel rViewModel = rMemberBinding.ViewModelProp.PropertyOwner as ViewModel;
                if (rViewModel != null)
                {
                    rViewModel.PropChangedHandler -= rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                }
                rMemberBinding.OnDestroy();
            }

            var rAllEventBindings = rViewControllerContainer.gameObject.GetComponentsInChildren<EventBinding>(true);
            for (int i = 0; i < rAllEventBindings.Length; i++)
            {
                rAllEventBindings[i].OnDestroy();
            }
        }

        /// <summary>
        /// 把ViewModel绑定到ViewController里面
        /// </summary>
        public void BindingViewModels(ViewControllerContainer rViewControllerContainer)
        {
            for (int i = 0; i < rViewControllerContainer.ViewModels.Count; i++)
            {
                var rViewModelDataSource = rViewControllerContainer.ViewModels[i];
                ViewModel rViewModel = ViewModelManager.Instance.ReceiveViewModel(rViewModelDataSource.ViewModelPath);
                
                // 匹配DefaultViewModel
                if (this.View.DefaultViewModel != null && this.View.DefaultViewModel.GetType().FullName.Equals(rViewModelDataSource.ViewModelPath))
                {
                    rViewModel = this.View.DefaultViewModel;
                }

                if (rViewModel != null)
                {
                    this.AddViewModel(rViewModelDataSource.ViewModelPath, rViewModel);
                }
                else
                {
                    Debug.LogErrorFormat("Can not find ViewModel {0}.", rViewModelDataSource.ViewModelPath);
                }
            }
        }

        /// <summary>
        /// 把Event绑定到ViewController里面
        /// </summary>
        private void BindingEvents(ViewControllerContainer rViewControllerContainer)
        {
            for (int i = 0; i < rViewControllerContainer.EventBindings.Count; i++)
            {
                var rEventBinding = rViewControllerContainer.EventBindings[i];
                if (rEventBinding.IsListTemplate) continue;
                
                var bResult = HotfixDataBindingTypeResolve.MakeViewModelDataBindingEvent(this, rEventBinding);
                if (!bResult)
                {
                    Debug.LogErrorFormat("Make view model binding event {0} failed..", rEventBinding.ViewModelMethod);
                }
            }
        }

        /// <summary>
        /// ViewModel和View之间的数据绑定
        /// </summary>
        private void BindingViewAndViewModels(ViewControllerContainer rViewControllerContainer)
        {
            var rAllMemberBindings = UtilTool.GetComponentsInChildrenUtilOrigin<MemberBindingAbstract>(rViewControllerContainer);
            for (int i = 0; i < rAllMemberBindings.Count; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];
                if (rMemberBinding.IsListTemplate) continue;    // 过滤掉ListTemplate标记得Binding Script

                rMemberBinding.ViewProp = DataBindingTypeResolve.MakeViewDataBindingProperty(rMemberBinding.gameObject, rMemberBinding.ViewPath);
                if (rMemberBinding.ViewProp == null)
                {
                    Debug.LogErrorFormat("View Path: {0} error..", rMemberBinding.ViewPath);
                    return;
                }
                
                rMemberBinding.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rMemberBinding.ViewModelPath);
                if (rMemberBinding.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rMemberBinding.ViewModelPath);
                    return;
                }
                ViewModel rViewModel = this.GetViewModel(rMemberBinding.ViewModelProp.PropertyOwnerKey);
                if (rViewModel == null)
                {
                    Debug.LogErrorFormat("View Model: {0} error..", rMemberBinding.ViewModelPath);
                    return;
                }

                // 设置类型转换器
                if (rMemberBinding.IsDataConvert)
                {
                    rMemberBinding.ViewModelProp.ConvertMethod = HotfixDataBindingTypeResolve.MakeViewModelDataBindingConvertMethod(rMemberBinding.DataConverterMethodPath);
                    if (rMemberBinding.IsDataConvert && rMemberBinding.ViewModelProp.ConvertMethod == null)
                    {
                        Debug.LogError("Cannot find convert method: {0}.." + rMemberBinding.DataConverterMethodPath);
                        return;
                    }
                }
                else
                {
                    rMemberBinding.ViewModelProp.ConvertMethod = null;
                }
                
                rMemberBinding.ViewModelProp.PropertyOwner = rViewModel;
                rMemberBinding.SyncFromViewModel();

                // ViewModel绑定View
                rMemberBinding.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rMemberBinding.ViewModelProp.PropertyName, () =>
                {
                    rMemberBinding.SyncFromViewModel();
                });
                rViewModel.PropChangedHandler += rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                
                // View绑定ViewModel
                var rMemberBindingTwoWay = rMemberBinding as MemberBindingTwoWay;
                if (rMemberBindingTwoWay != null)
                {
                    rMemberBindingTwoWay.InitEventWatcher();
                }
            }
        }

        /// <summary>
        /// ListViewModel和View之间的数据绑定
        /// </summary>
        private void BindingListViewAndViewModels(ViewControllerContainer rViewControllerContainer)
        {
            var rViewModelDataSources = UtilTool.GetComponentsInChildrenUtilOrigin<ViewModelDataSourceList>(rViewControllerContainer);
            for (int i = 0; i < rViewModelDataSources.Count; i++)
            {
                var rViewModelDataSource = rViewModelDataSources[i];
                rViewModelDataSource.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rViewModelDataSource.ViewModelPath);
                if (rViewModelDataSource.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }
                ViewModel rViewModel = this.GetViewModel(rViewModelDataSource.ViewModelProp.PropertyOwnerKey);
                if (rViewModel == null)
                {
                    Debug.LogErrorFormat("View Model: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }
                rViewModelDataSource.ViewModelProp.PropertyOwner = rViewModel;

                // 绑定Watcher
                rViewModelDataSource.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rViewModelDataSource.ViewModelProp.PropertyName, () =>
                {
                    // 重新设置List数据时候，改变个数
                    this.BindingList(rViewModelDataSource);
                });
                rViewModel.PropChangedHandler += rViewModelDataSource.ViewModelPropertyWatcher.PropertyChanged;

                // 初始化list
                this.BindingList(rViewModelDataSource);
            }
        }

        private void BindingList(ViewModelDataSourceList rViewModelDataSource)
        {
            var rViewModelObj = rViewModelDataSource.ViewModelProp.GetValue();
            if (rViewModelObj != null)
            {
                var rListObservableObj = rViewModelObj as IObservableEvent;
                rListObservableObj.ChangedHandler += () =>
                {
                    var rListObj2 = (IList)rViewModelDataSource.ViewModelProp.GetValue();
                    var nListCount2 = rListObj2 != null ? rListObj2.Count : 0;

                    var nOldCount = rViewModelDataSource.ListView.totalCount;
                    rViewModelDataSource.ListView.totalCount = nListCount2;
                    if (nListCount2 == nOldCount)
                        rViewModelDataSource.ListView.RefreshCells();
                    else
                        rViewModelDataSource.ListView.RefillCells();
                };

                var rListObj = rViewModelObj as IList;
                var nListCount = rListObj != null ? rListObj.Count : 0;

                rViewModelDataSource.ListView.OnFillCellFunc = (rTrans, nIndex) =>
                {
                    this.OnListViewFillCellFunc(rTrans, nIndex, rListObj);
                };
                rViewModelDataSource.ListView.totalCount = nListCount;
                rViewModelDataSource.ListView.RefillCells();
            }
            else
            {
                Debug.LogError($"ViewModel {rViewModelDataSource.ViewModelPath} getValue is null..");
            }
        }

        private void OnListViewFillCellFunc(Transform rTrans, int nIndex, IList rListObj)
        {
            if (rListObj == null || nIndex >= rListObj.Count) return;
            
            var rListItem = rListObj[nIndex] as ViewModel;
            if (rListItem == null) return;
            
            var rAllEventBindings = rTrans.GetComponentsInChildren<EventBinding>(true);
            for (int i = 0; i < rAllEventBindings.Length; i++)
            {
                var rEventBinding = rAllEventBindings[i];
                if (!rEventBinding.IsListTemplate) continue;
                rEventBinding.OnDestroy();
                var bResult = HotfixDataBindingTypeResolve.MakeListViewModelDataBindingEvent(this, rEventBinding, nIndex);
                if (!bResult)
                {
                    Debug.LogErrorFormat("Make view model binding event {0} failed..", rEventBinding.ViewModelMethod);
                }
            }
            
            // 清除已有的事件监听
            rListItem.PropChangedHandler = null;

            var rAllMemberBindings = rTrans.GetComponentsInChildren<MemberBindingAbstract>(true);
            for (int i = 0; i < rAllMemberBindings.Length; i++)
            {
                var rMemberBinding = rAllMemberBindings[i];
                if (!rMemberBinding.IsListTemplate) continue;    // 过滤掉非ListTemplate标记的Binding Script

                if (rMemberBinding.ViewProp == null)
                {
                    rMemberBinding.ViewProp = DataBindingTypeResolve.MakeViewDataBindingProperty(rMemberBinding.gameObject, rMemberBinding.ViewPath);
                }
                if (rMemberBinding.ViewProp == null)
                {
                    Debug.LogErrorFormat("List template View Path: {0} error..", rMemberBinding.ViewPath);
                    return;
                }

                if (rMemberBinding.ViewModelProp == null)
                {
                    rMemberBinding.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rMemberBinding.ViewModelPath);
                    if (rMemberBinding.ViewModelProp == null)
                    {
                        Debug.LogErrorFormat("View Model Path: {0} error..", rMemberBinding.ViewModelPath);
                        return;
                    }
                }

                // 设置类型转换器
                if (rMemberBinding.IsDataConvert)
                {
                    rMemberBinding.ViewModelProp.ConvertMethod = HotfixDataBindingTypeResolve.MakeViewModelDataBindingConvertMethod(rMemberBinding.DataConverterMethodPath);
                    if (rMemberBinding.IsDataConvert && rMemberBinding.ViewModelProp.ConvertMethod == null)
                    {
                        Debug.LogError("Cannot find convert method: {0}.." + rMemberBinding.DataConverterMethodPath);
                        return;
                    }
                }
                else
                {
                    rMemberBinding.ViewModelProp.ConvertMethod = null;
                }

                rMemberBinding.ViewModelProp.PropertyOwner = rListItem;
                rMemberBinding.SyncFromViewModel();

                if (rListItem != null)
                {
                    // ViewModel绑定View
                    rMemberBinding.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rListItem, rMemberBinding.ViewModelProp.PropertyName, () =>
                    {
                        rMemberBinding.SyncFromViewModel();
                    });
                    rListItem.PropChangedHandler += rMemberBinding.ViewModelPropertyWatcher.PropertyChanged;
                }
            }
        }

        private void BindingTabViewAndViewModels(ViewControllerContainer rViewControllerContainer)
        {
            var rViewModelDataSources = rViewControllerContainer.gameObject.GetComponentsInChildren<ViewModelDataSourceTab>(true);
            for (int i = 0; i < rViewModelDataSources.Length; i++)
            {
                var rViewModelDataSource = rViewModelDataSources[i];
                rViewModelDataSource.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rViewModelDataSource.ViewModelPath);
                if (rViewModelDataSource.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }
                ViewModel rViewModel = this.GetViewModel(rViewModelDataSource.ViewModelProp.PropertyOwnerKey);
                if (rViewModel == null)
                {
                    Debug.LogErrorFormat("View Model: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }

                rViewModelDataSource.ViewModelProp.PropertyOwner = rViewModel;

                // 绑定Watcher
                rViewModelDataSource.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rViewModelDataSource.ViewModelProp.PropertyName, () =>
                {
                    this.FillTabItems(rViewModelDataSource);
                });
                rViewModel.PropChangedHandler += rViewModelDataSource.ViewModelPropertyWatcher.PropertyChanged;

                this.FillTabItems(rViewModelDataSource);
            }
        }

        private void BindingArrayViewAndViewModels(ViewControllerContainer rViewModelContainer)
        {
            var rViewModelDataSources = rViewModelContainer.gameObject.GetComponentsInChildren<ViewModelDataSourceArray>(true);
            for (int i = 0; i < rViewModelDataSources.Length; i++)
            {
                var rViewModelDataSource = rViewModelDataSources[i];
                rViewModelDataSource.ViewModelProp = HotfixDataBindingTypeResolve.MakeViewModelDataBindingProperty(rViewModelDataSource.ViewModelPath);
                if (rViewModelDataSource.ViewModelProp == null)
                {
                    Debug.LogErrorFormat("View Model Path: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }
                ViewModel rViewModel = this.GetViewModel(rViewModelDataSource.ViewModelProp.PropertyOwnerKey);
                if (rViewModel == null)
                {
                    Debug.LogErrorFormat("View Model: {0} error..", rViewModelDataSource.ViewModelPath);
                    return;
                }

                rViewModelDataSource.ViewModelProp.PropertyOwner = rViewModel;

                // 绑定watcher
                rViewModelDataSource.ViewModelPropertyWatcher = new DataBindingPropertyWatcher(rViewModel, rViewModelDataSource.ViewModelProp.PropertyName, () => 
                {
                    this.FillArrayItems(rViewModelDataSource);
                });
                rViewModel.PropChangedHandler += rViewModelDataSource.ViewModelPropertyWatcher.PropertyChanged;

                this.FillArrayItems(rViewModelDataSource);
            }
        }

        private void FillArrayItems(ViewModelDataSourceArray rViewModelDataSource)
        {
            var rListObj = (IList)rViewModelDataSource.ViewModelProp.GetValue();
            var nListCount = rListObj != null ? rListObj.Count : 0;

            if (!rViewModelDataSource.HasInitData)
            {
                // 删除节点下的所有数据
                rViewModelDataSource.transform.DeleteChildren(true);

                for (int i = 0; i < nListCount; i++)
                {
                    GameObject rItemInstGo = GameObject.Instantiate(rViewModelDataSource.ItemTemplateGo);
                    rItemInstGo.SetActive(true);
                    rItemInstGo.name = "list_" + i;
                    rItemInstGo.transform.SetParent(rViewModelDataSource.transform, false);
                    this.OnListViewFillCellFunc(rItemInstGo.transform, i, rListObj);
                }
            }
            else
            {
                int k = 0;
                for (int i = 0; i < rViewModelDataSource.transform.childCount; i++)
                {
                    var rTrans = rViewModelDataSource.transform.GetChild(i);
                    if (rTrans.gameObject.activeSelf)
                    {
                        rTrans.name = "list_" + k;
                        this.OnListViewFillCellFunc(rTrans, k, rListObj);
                        k++;
                    }
                }
            }
        }
        
        private void FillTabItems(ViewModelDataSourceTab rViewModelDataSource)
        {                    
            // 重新设置Tab数据时候，改变个数
            var rListObj = (IList)rViewModelDataSource.ViewModelProp.GetValue();
            var nListCount = rListObj != null ? rListObj.Count : 0;

            rViewModelDataSource.TabView.transform.DeleteChildren(true);
            rViewModelDataSource.TabView.TabButtons = new List<TabButton>();

            for (int k = 0; k < nListCount; k++)
            {
                GameObject rTabInstGo = GameObject.Instantiate(rViewModelDataSource.TabView.TabTemplateGo);
                rTabInstGo.SetActive(true);
                rTabInstGo.name = "tab_" + k;
                rTabInstGo.transform.SetParent(rViewModelDataSource.TabView.transform, false);
                this.OnListViewFillCellFunc(rTabInstGo.transform, k, rListObj);

                var rTabButton = rTabInstGo.ReceiveComponent<TabButton>();
                rTabButton.group = rViewModelDataSource.TabView;
                rTabButton.TabIndex = k;
                rTabButton.isOn = k == 0;
                rViewModelDataSource.TabView.TabButtons.Add(rTabButton);
            }
        }

        public ViewModel GetViewModel(string rKey)
        {
            ViewModel rViewModel = null;
            this.ViewModels.TryGetValue(rKey, out rViewModel);
            return rViewModel;
        }

        protected void AddViewModel(string rKey, ViewModel rViewModel)
        {
            this.ViewModels.Add(rKey, rViewModel);
        }

        public async Task Open()
        {
            await this.OnOpen();
        }
        
        public void Show()
        {
            this.OnShow();
        }

        public void Hide()
        {
            this.OnHide();
        }
        
        public void Closing()
        {
            this.OnClose();
        }

        /// <summary>
        /// 数据绑定前
        /// </summary>
        #region Virtual Function
        protected override async Task OnInitialize()
        {
            await base.OnInitialize();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDispose()
        {
            foreach (var rPair in this.ViewModels)
            {
                rPair.Value.PropChangedHandler = null;
                rPair.Value = null;
            }
            this.ViewModels.Clear();
        }

        /// <summary>
        /// 数据绑定之后
        /// </summary>
#pragma warning disable 1998
        protected virtual async Task OnOpen()
#pragma warning restore 1998
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
        
        protected virtual void OnClose()
        {
        }
        #endregion
    }
}
