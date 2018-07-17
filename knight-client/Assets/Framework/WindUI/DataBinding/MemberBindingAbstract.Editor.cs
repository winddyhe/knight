using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public partial class MemberBindingAbstract
    {
        [HideInInspector]
        public  string[]            ViewPaths   = new string[0];
        [HideInInspector]
        public  string[]            ModelPaths  = new string[0];

        public void GetPaths()
        {
            this.ViewPaths  = DataBindingTypeResolve.GetAllViewPaths(this.gameObject).ToArray();
            this.ViewProp   = DataBindingTypeResolve.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);

            if (this.ViewProp != null)
            {
                this.ModelPaths = DataBindingTypeResolve.GetAllViewModelPaths(this.gameObject, this.ViewProp.Property.PropertyType).ToArray();
            }
        }
    }
}
