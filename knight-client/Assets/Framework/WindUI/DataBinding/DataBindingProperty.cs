using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public class DataBindingProperty
    {
        public object           PropertyOwner;

        public string           PropertyName;
        public PropertyInfo     Property;
        
        public DataBindingProperty(object rPropOwner, string rPropName)
        {
            this.PropertyOwner  = rPropOwner;
            this.PropertyName   = rPropName;
            this.Property       = rPropOwner.GetType().GetProperty(rPropName);
        }

        public object GetValue()
        {
            return this.Property?.GetValue(this.PropertyOwner);
        }

        public void SetValue(object rValue)
        {
            this.Property?.SetValue(this.PropertyOwner, rValue);
        }
    }
}
