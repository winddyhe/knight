using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace Knight.Framework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public partial class DataBindingEvent : MonoBehaviour
    {
        [OnValueChanged("GetPaths")]
        public bool IsListTemlate;

        [Dropdown("ViewEventPaths")]
        [ValidateInput("IsViewEventPathValid")]
        [OnValueChanged("GetPaths")]
        public string ViewEventPath;

        [Dropdown("ViewModelEventPaths")]
        [ValidateInput("IsViewModelEventPathValid")]
        [OnValueChanged("GetPaths")]
        public string ViewModelEventPath;
    }

    public partial class DataBindingEvent
    {
        private string[] ViewEventPaths = new string[0];
        private string[] ViewModelEventPaths = new string[0];

        public void GetPaths()
        {
            this.ViewEventPaths = DataBindingTypeResolve.GetBindableEventPaths(this.gameObject);
            this.ViewModelEventPaths = DataBindingTypeResolve.GetViewModelBindingEvents(this.gameObject, this.IsListTemlate);

            if (!this.ViewEventPaths.Contains(this.ViewEventPath))
            {
                this.ViewEventPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewEventPath))
            {
                this.ViewEventPath = this.ViewEventPaths.Length > 0? this.ViewEventPaths[0] : "";
            }

            if (!this.ViewModelEventPaths.Contains(this.ViewModelEventPath))
            {
                this.ViewModelEventPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewModelEventPath))
            {
                this.ViewModelEventPath = this.ViewModelEventPaths.Length > 0? this.ViewModelEventPaths[0] : "";
            }
        }

        public Component GetViewComponent()
        {
            return DataBindingTypeResolve.GetViewEventComponent(this.gameObject, this.ViewEventPath);
        }

        public bool IsViewEventPathValid()
        {
            if (this.ViewEventPaths == null || this.ViewEventPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewEventPaths.Contains(this.ViewEventPath);
        }

        public bool IsViewModelEventPathValid()
        {
            if (this.ViewModelEventPaths == null || this.ViewModelEventPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewModelEventPaths.Contains(this.ViewModelEventPath);
        }
    }
#endif
}
