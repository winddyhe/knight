using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Knight.Core;
using Knight.Framework.TypeResolve;
using Knight.Framework.Hotfix;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public    string            CurModelPath;
        [Dropdown("ViewPaths")]
        [ShowIf("IsShowCurViewPath")]
        public    string            CurViewPath;
        
        [HideInInspector]
        public    ModelDataItem     CurModelData;
        [HideInInspector]
        public    ViewDataItem      CurViewData;

        protected HotfixObject      mHotfixBindingObj;
        public    Action<string>    ModelPropertyChanged;

        private void Awake()
        {
        }

        //public void BindingPropertyChangedEvent(Action<string> rModelPropertyChanged)
        //{
        //    this.ModelPropertyChanged += rModelPropertyChanged;
        //}

        //public void UnbindingPropertyChangedEvent(Action<string> rModelPropertyChanged)
        //{
        //    this.ModelPropertyChanged -= rModelPropertyChanged;
        //}

        //public void ModelPropertyChanged(string rPropName)
        //{
        //    var rModelData = this.CurModelData;

        //    var rModelProp = rModelType.GetProperty(rModelData.VaribleName, HotfixReflectAssists.flags_public);
        //    object rModelValue = null;
        //    if (rModelProp != null)
        //    {
        //        rModelValue = rModelProp.GetValue(this.ViewModel);
        //    }
        //    rAllDataBindings[i].SetViewData(rModelValue);
        //}

        public void SetViewData(object rModelValue)
        {
            var rViewType = this.CurViewData.ViewComp.GetType();
            var rViewField = rViewType.GetField(this.CurViewData.VaribleName, ReflectionAssist.flags_public);
            if (rViewField != null)
            {
                rViewField.SetValue(this.CurViewData.ViewComp, rModelValue);
            }
            else
            {
                var rViewProp = rViewType.GetProperty(this.CurViewData.VaribleName, ReflectionAssist.flags_public);
                if (rViewProp != null)
                {
                    rViewProp.SetValue(this.CurViewData.ViewComp, rModelValue);
                }
            }
        }
    }
}