using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.UI
{
    [ExecuteInEditMode]
    public partial class ViewModelDataSource : MonoBehaviour
    {
        public bool IsGlobal;
        [OnValueChanged("GetPaths")]
        public string Key;

        [Dropdown("ViewModelClasses")]
        [ValidateInput("IsPathValid")]
        [OnValueChanged("GetPaths")]
        public string ViewModelPath;
    }

#if UNITY_EDITOR
    public partial class ViewModelDataSource
    {
        private string[] ViewModelClasses;

        public void GetPaths()
        {
            this.ViewModelClasses = DataBindingTypeResolve.GetAllViewModels().ToArray();

            if (!this.ViewModelClasses.Contains(this.ViewModelPath))
            {
                this.ViewModelPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ViewModelClasses.Length > 0 ? this.ViewModelClasses[0] : string.Empty;
            }
        }

        public bool IsPathValid()
        {
            if (this.ViewModelClasses == null || this.ViewModelClasses.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewModelClasses.Any(item => item == this.ViewModelPath);
        }
    }
#endif
}
