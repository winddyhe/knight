using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Knight.Framework.TypeResolve;

namespace UnityEngine.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class DataBindingAttribute : Attribute
    {
    }
    
    [System.Serializable]
    public class ModelDataItem
    {
        public string           Path;
        public DataSourceModel  DataSource;
        public string           ClassName;
        public string           VaribleName;
        
        public Type             VaribleType;
    }

    [System.Serializable]
    public class ViewDataItem
    {
        public string           Path;
        public Component        ViewComp;
        public string           ClassName;
        public string           VaribleName;

        public Type             VaribleType;
    }

    [DefaultExecutionOrder(90)]
    public class DataSourceModel : MonoBehaviour
    {
        public string           ModelClass;
        public object           ModelObject;

        public void Awake()
        {
            this.ModelObject = TypeResolveManager.Instance.Instantiate(this.ModelClass);
        }
    }
}