using Cysharp.Threading.Tasks;
using Knight.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI
{
    public enum ViewState
    {
        Fixed,
        PageSwitch,
        PageOverlap
    }

    public enum ViewLayer
    {
        Fixed = 1000,
        Page = 2000,
        PopupBlur = 3000,
        Global = 4000
    }

    public class ViewConfig
    {
        public string Name;
        public ViewLayer Layer;
    }

    public class View : MonoBehaviour
    {
        public Guid GUID;
        public string ViewModuleName;
        public ViewLayer ViewLayer;
        public ViewConfig ViewConfig;

        public GameObject ViewGo;
        public Canvas Canvas;
        public CanvasGroup CanvasGroup;
        public Animator Animator;
        public Dictionary<string, ViewModel> ViewModels;

        public virtual ViewController ViewController { get; }

        public virtual void Initialize(ViewController rViewController)
        {
            this.BindViewModels();
        }

        public virtual void Bind()
        {
        }

        public virtual void UnBind()
        {
        }

        public void SetCanvasOrder(ViewLayer rViewLayer, int nOrder)
        {
            this.Canvas.overrideSorting = true;
            this.Canvas.sortingOrder = (int)rViewLayer + nOrder;
        }

        public void SetActive(bool bActive)
        {
            if (bActive)
            {
                this.transform.localPosition = Vector3.zero;
                this.CanvasGroup.alpha = 1.0f;
            }
            else
            {
                this.transform.localPosition = new Vector3(0, 50000, 0);
                this.CanvasGroup.alpha = 0.0f;
            }
        }

        public void SetViewLayer(ViewLayer rViewLayer)
        {
            this.ViewLayer = rViewLayer;

            switch (this.ViewLayer)
            {
                case ViewLayer.Fixed:
                    this.ViewGo.transform.SetParent(UIRoot.Instance.FixedRoot.transform, false);
                    this.ViewGo.SetLayer("UI");
                    break;
                case ViewLayer.Page:
                    this.ViewGo.transform.SetParent(UIRoot.Instance.DynamicRoot.transform, false);
                    this.ViewGo.SetLayer("UI");
                    break;
                case ViewLayer.PopupBlur:
                    this.ViewGo.transform.SetParent(UIRoot.Instance.PopupBlurRoot.transform, false);
                    this.ViewGo.SetLayer("UIBlur");
                    break;
                case ViewLayer.Global:
                    this.ViewGo.transform.SetParent(UIRoot.Instance.GlobalRoot.transform, false);
                    this.ViewGo.SetLayer("UI");
                    break;
            }
        }

        protected void BindViewModels()
        {
            this.ViewModels = new Dictionary<string, ViewModel>();

            var rViewModelDataSources = this.GetComponents<ViewModelDataSource>();
            foreach (var rViewModelDataSource in rViewModelDataSources)
            {
                ViewModel rViewModel = null;
                if (rViewModelDataSource.IsGlobal)
                    rViewModel = ViewModelManager.Instance.ReceiveGlobalViewModel(rViewModelDataSource.ViewModelPath);
                else
                    rViewModel = ViewModelManager.Instance.CreateViewModel(rViewModelDataSource.ViewModelPath);
                if (rViewModel == null) continue;

                this.ViewModels.Add(rViewModelDataSource.Key, rViewModel);
            }

            var rAllFields = this.ViewController.GetType().GetFields(ReflectTool.flags_all);
            for (int i = 0; i < rAllFields.Length; i++)
            {
                var rFieldInfo = rAllFields[i];
                var rAttributes = rFieldInfo.GetCustomAttributes(typeof(ViewModelKeyAttribute), true);
                if (rAttributes.Length == 0) continue;
                var rViewModelKeyAttr = rAttributes[0] as ViewModelKeyAttribute;
                if (rViewModelKeyAttr == null) continue;

                if (this.ViewModels.TryGetValue(rViewModelKeyAttr.Key, out var rViewModel))
                {
                    rFieldInfo.SetValue(this.ViewController, rViewModel);
                }
            }
        }

        public async UniTask Open()
        {
            if (this.ViewController == null)
            {
                LogManager.LogErrorFormat("ViewController is null, ViewName = {0}", this.ViewConfig.Name);
                return;
            }
            await this.ViewController.Open();
        }

        public void Close()
        {
            this.ViewController?.Close();
        }
    }
}