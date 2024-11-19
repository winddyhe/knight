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
    public partial class ViewControllerDataSource : MonoBehaviour
    {
        [Dropdown("ViewControllerClasses")]
        [ValidateInput("IsViewControllerClassValid")]
        [OnValueChanged("GetPaths")]
        public string ViewControllerClass;

        [HideInInspector]
        public string ViewControllerName;
        [HideInInspector]
        public string ViewName;
    }

    public partial class ViewControllerDataSource
    {
        private string[] ViewControllerClasses = new string[0];

        public void GetPaths()
        {
            this.ViewControllerClasses = DataBindingTypeResolve.GetAllViews().ToArray();
            if (!this.ViewControllerClasses.Contains(this.ViewControllerClass))
            {
                this.ViewControllerClass = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewControllerClass))
            {
                this.ViewControllerClass = this.ViewControllerClasses.Length > 0 ? this.ViewControllerClasses[0] : string.Empty;
            }
            if (!string.IsNullOrEmpty(this.ViewControllerClass))
            {
                this.ViewControllerName = this.ViewControllerClass.Split('.').Last();
                this.ViewName = this.ViewControllerName.Replace("ViewController", "View");
            }
        }

        public bool IsViewControllerClassValid()
        {
            if (this.ViewControllerClasses == null || this.ViewControllerClasses.Length == 0)
            {
                this.GetPaths();
            } 
            return this.ViewControllerClasses.Contains(this.ViewControllerClass);
        }
    }
}
