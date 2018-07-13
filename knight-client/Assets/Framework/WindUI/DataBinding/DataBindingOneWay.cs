using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Knight.Core;
using Knight.Framework.TypeResolve;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public  string          CurModelPath;
        [Dropdown("ViewPaths")]
        [ShowIf("IsShowCurViewPath")]
        public  string          CurViewPath;
        
        [HideInInspector]
        public  ModelDataItem   CurModelData;
        [HideInInspector]
        public  ViewDataItem    CurViewData;

        private void Awake()
        {
            this.DataBinding();
        }

        protected void DataBinding()
        {
            if (this.CurModelData == null || this.CurViewData == null) return;

            var rModelType = TypeResolveManager.Instance.GetType(this.CurModelData.ClassName);
            if (rModelType == null) return;

            object rModelValue = null;
            var rModelFiled = rModelType.GetField(this.CurModelData.VaribleName, ReflectionAssist.flags_public);
            if (rModelFiled != null)
            {
                rModelValue = rModelFiled.GetValue(null);
            }
            else
            {
                var rModelProp = rModelType.GetProperty(this.CurModelData.VaribleName, ReflectionAssist.flags_public);
                if (rModelProp != null)
                {
                    rModelValue = rModelProp.GetValue(this.CurModelData.DataSource);
                }
            }

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