using System;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Knight.Framework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public partial class DataBindingAbstract : MonoBehaviour
    {
        [OnValueChanged("GetPaths")]
        public bool IsListTemlate;

        [Dropdown("ViewPaths")]
        [HideIf("IsHideViewPath")]
        [ValidateInput("IsViewPathVaild")]
        [OnValueChanged("GetPaths")]
        public string ViewPath;

        [Dropdown("ViewModelPaths")]
        [ValidateInput("IsViewModelPathVaild")]
        [OnValueChanged("GetPaths")]
        public string ViewModelPath;

        public BindableProperty ViewProperty;
    }

    public partial class DataBindingAbstract
    {
        private string[] ViewPaths = new string[0];
        private string[] ViewModelPaths = new string[0];

        public virtual void GetPaths()
        {
            this.ViewPaths = DataBindingTypeResolve.GetViewPaths(this.gameObject).ToArray();
            
            if (!this.ViewPaths.Contains(this.ViewPath))
            {
                this.ViewPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewPath))
            {
                this.ViewPath = this.ViewPaths.Length > 0 ? this.ViewPaths[0] : string.Empty;
            }
            this.ViewProperty = DataBindingTypeResolve.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);
            
            if (this.ViewProperty != null)
            {
                var rViewModelProps = DataBindingTypeResolve.GetViewModelProperties(this.gameObject, this.ViewProperty.PropertyType, this.IsListTemlate);
                this.ViewModelPaths = DataBindingTypeResolve.GetViewModelPaths(rViewModelProps).ToArray();
            }
            if (!this.ViewModelPaths.Contains(this.ViewModelPath))
            {
                this.ViewModelPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ViewModelPaths.Length > 0 ? this.ViewModelPaths[0] : string.Empty;
            }
        }

        public Component GetViewComponent()
        {
            if (string.IsNullOrEmpty(this.ViewPath)) return null;

            this.ViewProperty = DataBindingTypeResolve.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);
            if (this.ViewProperty == null) return null;

            return this.ViewProperty.PropertyOwner as Component;
        }

        protected virtual bool IsHideViewPath()
        {
            return false;
        }

        public bool IsViewPathVaild()
        {
            if (this.ViewPaths.Length == 0 ||
                this.ViewModelPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewPaths.Contains(this.ViewPath);
        }

        public bool IsViewModelPathVaild()
        {
            if (this.ViewPaths.Length == 0 ||
                this.ViewModelPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewModelPaths.Contains(this.ViewModelPath);
        }

        [Button("Refresh")]
        public void Refresh()
        {
            this.GetPaths();
        }
    }
#endif
}
