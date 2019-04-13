//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class ViewModelDataSourceList : ViewModelDataSourceTemplate
    {
        [DrawOrder(2)]
        public LoopScrollRect   ListView;
        
        public override void GetPaths()
        {
            base.GetPaths();

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
