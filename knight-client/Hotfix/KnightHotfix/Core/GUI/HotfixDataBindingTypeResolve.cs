using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class HotfixDataBindingTypeResolve
    {
        public static DataBindingProperty MakeViewModelDataBindingProperty(string rViewModelPath, ViewController rViewController, out ViewModel rViewModel)
        {
            rViewModel = null;
            if (string.IsNullOrEmpty(rViewModelPath)) return null;

            var rViewModelPathStrs = rViewModelPath.Split('/');
            if (rViewModelPathStrs.Length < 2) return null;

            var rViewModelClass = rViewModelPathStrs[0].Trim();
            var rViewModelClassStrs = rViewModelClass.Split('@');
            if (rViewModelClassStrs.Length < 1) return null;

            var rViewModelClassKey = rViewModelClassStrs[0].Trim();
            var rViewModelClassName = rViewModelClassStrs[1].Trim();
            
            var rViewModelProp = rViewModelPathStrs[1].Trim();

            var rViewModelPropStrs = rViewModelProp.Split(':');
            if (rViewModelPropStrs.Length < 1) return null;

            var rViewModelPropName = rViewModelPropStrs[0].Trim();

            // 取到ViewModel
            rViewModel = rViewController.GetViewModel(rViewModelClassKey);

            var rViewModelProperty = new DataBindingProperty(rViewModel, rViewModelClassKey, rViewModelPropName);
            rViewModelProperty.Property = rViewModel.GetType().GetProperty(rViewModelPropName);
            return rViewModelProperty; 
        }
    }
}
