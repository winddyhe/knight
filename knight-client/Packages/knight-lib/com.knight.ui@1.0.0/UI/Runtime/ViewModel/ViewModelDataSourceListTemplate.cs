using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;

namespace Knight.Framework.UI
{
    [ExecuteInEditMode]
    public partial class ViewModelDataSourceListTemplate : MonoBehaviour
    {
        public FancyScrollRectView ScrollView;

        [Dropdown("ViewModelPaths")]
        [ValidateInput("IsViewModelPathValid")]
        public string ViewModelPath;

        [ReadOnly]
        public string TemplatePath;
    }

    public partial class ViewModelDataSourceListTemplate
    {
        private string[] ViewModelPaths = new string[0];

        public void GetPaths()
        {
            var rViewModelProps = DataBindingTypeResolve.GetViewModelListTemplateProperties(this.gameObject);
            this.ViewModelPaths = DataBindingTypeResolve.GetViewModelPaths(rViewModelProps).ToArray();
            if (!this.ViewModelPaths.Contains(this.ViewModelPath))
            {
                this.ViewModelPath = string.Empty;
            }
            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ViewModelPaths.Length > 0 ? this.ViewModelPaths[0] : string.Empty;
            }

            int nIndex = -1;
            for (int i = 0; i < this.ViewModelPaths.Length; i++)
            {
                var rViewModelPath = this.ViewModelPaths[i];
                if (rViewModelPath == this.ViewModelPath)
                {
                    nIndex = i;
                    break;
                }
            }
            if (nIndex >= 0)
            {
                var rViewModelProp = rViewModelProps.ElementAt(nIndex);
                this.TemplatePath = rViewModelProp.Member.PropertyType.GenericTypeArguments[0].FullName;
            }
        }

        public bool IsViewModelPathValid()
        {
            if (this.ViewModelPaths.Length == 0)
            {
                this.GetPaths();
            }
            return this.ViewModelPaths.Contains(this.ViewModelPath);
        }
    }
}
