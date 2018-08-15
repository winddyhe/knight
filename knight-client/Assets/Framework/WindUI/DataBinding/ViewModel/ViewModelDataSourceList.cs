using System;
using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class ViewModelDataSourceList : MonoBehaviour
    {
        [DrawOrder(2)]
        public LoopScrollRect               ListView;
        [Dropdown("ModelPaths")]
        public string                       ViewModelPath;
        public string                       ListItemPath;

        public DataBindingProperty          ViewModelProp;
        public DataBindingPropertyWatcher   ViewModelPropertyWatcher;
    }

    public partial class ViewModelDataSourceList
    {
        [HideInInspector]
        protected string[]         ModelPaths;

        public void GetPaths()
        {
            this.ListView = this.GetComponent<LoopScrollRect>();            
            if (this.ListView != null)
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
