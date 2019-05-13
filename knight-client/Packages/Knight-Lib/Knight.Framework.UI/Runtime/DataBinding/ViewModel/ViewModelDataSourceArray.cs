using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public class ViewModelDataSourceArray : ViewModelDataSourceTemplate
    {
        [DrawOrder(2)]
        public GameObject   ItemTemplateGo;
        public bool         HasInitData;

        public override void GetPaths()
        {
            base.GetPaths();
            
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
