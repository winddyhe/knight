using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace Knight.Framework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public partial class DataBindingTwoWay : DataBindingAbstract
    {
        [Dropdown("EventPaths")]
        [ValidateInput("IsPathValid")]
        [OnValueChanged("GetPaths")]
        public string EventPath;
    }

    public partial class DataBindingTwoWay
    {
        private string[] EventPaths = new string[0];

        public override void GetPaths()
        { 
            base.GetPaths();
            
            this.EventPaths = DataBindingTypeResolve.GetBindableEventPaths(this.gameObject);
            if (!this.EventPaths.Contains(this.EventPath))
            {
                this.EventPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.EventPath))
            {
                this.EventPath = this.EventPaths.Length > 0 ? this.EventPaths[0] : "";
            }
        }

        public bool IsPathValid()
        {
            if (this.EventPaths == null || this.EventPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.EventPaths.Contains(this.EventPath);
        }
    }
#endif
}
