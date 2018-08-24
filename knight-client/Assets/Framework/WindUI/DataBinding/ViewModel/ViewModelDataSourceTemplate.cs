using NaughtyAttributes;
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class ViewModelDataSourceTemplate : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public string                       ViewModelPath;
        public string                       TemplatePath;

        public DataBindingProperty          ViewModelProp;
        public DataBindingPropertyWatcher   ViewModelPropertyWatcher;
        
        [HideInInspector]
        protected string[]                  ModelPaths = new string[0];

        public virtual void GetPaths()
        {
        }
    }
}