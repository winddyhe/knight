//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class ViewModelDataSourceTemplate : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public string                       ViewModelPath;
        [Dropdown("TemplateViewModels")]
        public string                       TemplatePath;

        public DataBindingProperty          ViewModelProp;
        public DataBindingPropertyWatcher   ViewModelPropertyWatcher;
        
        [HideInInspector]
        protected string[]                  ModelPaths = new string[0];
        [HideInInspector]
        protected string[]                  TemplateViewModels = new string[0];                  

        public virtual void GetPaths()
        {
            this.TemplateViewModels = DataBindingTypeResolve.GetAllViewModels().ToArray();
        }
    }
}