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
        [Dropdown("ViewPaths")]
        public string                       ViewPath;
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
        public  string[]                    ViewPaths   = new string[0];
        [HideInInspector]
        public  string[]                    ModelPaths  = new string[0];
        
        public void GetPaths()
        {
            this.ViewPaths  = DataBindingTypeResolve.GetAllViewPaths(this.gameObject).ToArray();
            this.ViewProp   = DataBindingTypeResolve.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);

            if (this.ViewProp != null)
            {
                var rViewModelProps = new List<BindableMember<PropertyInfo>>(
                    DataBindingTypeResolve.GetViewModelProperties(this.gameObject, this.ViewProp.Property.PropertyType));

                this.ModelPaths = DataBindingTypeResolve.GetAllViewModelPaths(rViewModelProps).ToArray();
            }

            if (string.IsNullOrEmpty(this.ViewPath))
            {
                this.ViewPath = this.ViewPaths.Length > 0 ? this.ViewPaths[0] : "";
            }
            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ModelPaths.Length > 0 ? this.ModelPaths[0] : "";
            }
        }
    }
}
