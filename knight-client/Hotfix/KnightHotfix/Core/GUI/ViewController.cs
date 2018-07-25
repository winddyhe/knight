using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knight.Core;
using UnityEngine;

namespace Knight.Hotfix.Core
{
    public class ViewController : HotfixKnightObject
    {
        public    bool                      IsOpened;
        public    bool                      IsClosed;

        protected Dict<string, ViewModel>   ViewModels;

        public ViewController()
        {
            this.ViewModels = new Dict<string, ViewModel>();
        }

        public void AddViewModel(string rKey, ViewModel rViewModel)
        {
            this.ViewModels.Add(rKey, rViewModel);
        }

        public ViewModel GetViewModel(string rKey)
        {
            ViewModel rViewModel = null;
            this.ViewModels.TryGetValue(rKey, out rViewModel);
            return rViewModel;
        }
        
        public void Opening()
        {
            this.IsOpened = true;
            this.OnOpening();
        }

        public void Opened()
        {
            this.OnOpened();
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
            this.IsClosed = true;
            this.OnClosing();
        }

        public void Closed()
        {
            this.OnClosed();
        }
        
        #region Virtual Function
        protected override async Task OnInitialize()
        {
            await base.OnInitialize();

            foreach (var rPair in this.ViewModels)
            {
                ViewModel rViewModel = rPair.Value;

                var rViewModelProp = this.GetType().GetFields(HotfixReflectAssists.flags_public)
                    .Where(prop =>
                    {
                        var rAttrObjs = prop.GetCustomAttributes(typeof(HotfixBindingAttribute), false);
                        if (rAttrObjs == null || rAttrObjs.Length == 0) return false;
                        var rBindingAttr = rAttrObjs[0] as HotfixBindingAttribute;

                        return prop.FieldType.IsSubclassOf(typeof(ViewModel)) &&
                                                           rBindingAttr != null &&
                                                           rBindingAttr.Name.Equals(rPair.Key);
                    }).FirstOrDefault();

                if (rViewModelProp != null)
                {
                    rViewModelProp.SetValue(this, rViewModel);
                }
                else
                {
                    Debug.LogErrorFormat("ViewModel {0} is not define in ViewController({1})", rViewModel.GetType(), this.GetType());
                }
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDispose()
        {
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

        protected virtual void OnClosing()
        {
        }

        protected virtual void OnClosed()
        {
        }
        #endregion
    }
}
