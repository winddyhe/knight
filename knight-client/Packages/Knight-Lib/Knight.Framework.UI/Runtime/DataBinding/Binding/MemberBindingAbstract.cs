using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public partial class MemberBindingAbstract : MonoBehaviour
    {
        public bool                         IsListTemplate;
        [Dropdown("ViewPaths")]
        [HideIf("IsHideViewPath")]
        public string                       ViewPath;

        [HideIf("IsDisplayConverter")]
        public bool                         IsDataConvert;
        [Dropdown("ModelConvertMethods")]
        [HideIf("IsNeedDataConverter")]
        public string                       DataConverterMethodPath;

        [Dropdown("ModelPaths")]
        public string                       ViewModelPath;

        public DataBindingProperty          ViewProp;
        public DataBindingProperty          ViewModelProp;

        public DataBindingPropertyWatcher   ViewModelPropertyWatcher;

        public void SyncFromViewModel()
        {
            var rValue = this.ViewModelProp?.GetValue();
            this.ViewProp?.SetValue(rValue);
        }

        public void SyncFromView()
        {
            var rValue = this.ViewProp?.GetValue();
            this.ViewModelProp?.SetValue(rValue);
        }

        public virtual void OnDestroy()
        {
        }
    }

    public partial class MemberBindingAbstract
    {
        [HideInInspector]
        public  string[]                    ViewPaths           = new string[0];
        [HideInInspector]
        public  string[]                    ModelPaths          = new string[0];
        [HideInInspector]
        public  string[]                    ModelConvertMethods = new string[0];
        
        public virtual void GetPaths()
        {
            this.ViewPaths  = DataBindingTypeResolve.GetAllViewPaths(this.gameObject).ToArray();
            this.ViewProp   = DataBindingTypeResolve.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);

            if (this.ViewProp != null)
            {
                if (!this.IsDataConvert)
                {
                    var rViewModelProps = new List<BindableMember<PropertyInfo>>(
                        DataBindingTypeResolve.GetViewModelProperties(this.gameObject, this.ViewProp.Property.PropertyType, this.IsListTemplate));
                    this.ModelPaths = DataBindingTypeResolve.GetAllViewModelPaths(rViewModelProps).ToArray();
                }
                else
                {
                    var rViewModelMethods = new List<BindableMember<MethodInfo>>(
                        DataBindingTypeResolve.GetViewModelConvertMethods(this.ViewProp.Property.PropertyType, this.IsListTemplate));
                    this.ModelConvertMethods = DataBindingTypeResolve.GetAllConvertMethodPaths(rViewModelMethods).ToArray();
                    
                    if (!string.IsNullOrEmpty(this.DataConverterMethodPath))
                    {
                        var nIndex = new List<string>(this.ModelConvertMethods).IndexOf(this.DataConverterMethodPath);
                        if (nIndex >= 0)
                        {
                            var rViewModelType = rViewModelMethods[nIndex].ViewModelType;
                            var rParamType = rViewModelMethods[nIndex].Member.GetParameters()[0].ParameterType;
                            var rViewModelProps = new List<BindableMember<PropertyInfo>>(
                                DataBindingTypeResolve.GetViewModelProperties(this.gameObject, rParamType, rViewModelType, this.IsListTemplate));
                            this.ModelPaths = DataBindingTypeResolve.GetAllViewModelPaths(rViewModelProps).ToArray();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(this.ViewPath))
            {
                this.ViewPath = this.ViewPaths.Length > 0 ? this.ViewPaths[0] : "";
            }
            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ModelPaths.Length > 0 ? this.ModelPaths[0] : "";
            }
            if (string.IsNullOrEmpty(this.DataConverterMethodPath))
            {
                this.DataConverterMethodPath = this.ModelConvertMethods.Length > 0 ? this.ModelConvertMethods[0] : "";
            }
        }

        protected virtual bool IsHideViewPath()
        {
            return false;
        }

        protected bool IsNeedDataConverter()
        {
            return !this.IsDataConvert;
        }

        protected bool IsDisplayConverter()
        {
            return this.GetType() != typeof(UnityEngine.UI.MemberBindingOneWay);
        }
    }
}
