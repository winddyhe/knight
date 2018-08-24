using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public class ViewModelDataSourceTab : ViewModelDataSourceTemplate
    {
        [DrawOrder(2)]
        public TabView  TabView;

        public override void GetPaths()
        {
            this.TabView = this.GetComponent<TabView>();
            if (this.TabView != null)
            {
                var rViewModelProps = new List<BindableMember<PropertyInfo>>(
                    DataBindingTypeResolve.GetListViewModelProperties(this.gameObject));
                this.ModelPaths = DataBindingTypeResolve.GetAllViewModelPaths(rViewModelProps).ToArray();

                if (string.IsNullOrEmpty(this.ViewModelPath))
                {
                    this.ViewModelPath = this.ModelPaths.Length > 0 ? this.ModelPaths[0] : "";
                }
            }
        }
    }
}
