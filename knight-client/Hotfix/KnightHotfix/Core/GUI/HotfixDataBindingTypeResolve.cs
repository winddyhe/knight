using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class HotfixDataBindingTypeResolve
    {
        public static DataBindingProperty MakeViewModelDataBindingProperty(ViewModel rViewModel, string rViewModelPath)
        {
            if (string.IsNullOrEmpty(rViewModelPath)) return null;

            var rViewModelPathStrs = rViewModelPath.Split('/');
            if (rViewModelPathStrs.Length < 2) return null;

            var rViewModelClassName = rViewModelPathStrs[0].Trim();
            var rViewModelProp = rViewModelPathStrs[1].Trim();

            var rViewModelPropStrs = rViewModelProp.Split(':');
            if (rViewModelPropStrs.Length < 1) return null;

            var rViewModelPropName = rViewModelPropStrs[0].Trim();

            Type rViewModelType = Type.GetType(rViewModelClassName);
            if (rViewModelType == null || !rViewModel.GetType().Equals(rViewModelType)) return null;
            
            var rViewModelProperty = new DataBindingProperty(rViewModel, rViewModelPropName);
            rViewModelProperty.Property = rViewModel.GetType().GetProperty(rViewModelPropName);
            return rViewModelProperty; 
        }
    }
}
