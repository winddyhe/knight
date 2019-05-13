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
        /// <summary>
        /// 这个Key是ViewModelClassName
        /// </summary>
        public string           PropertyOwnerKey;   
        public object           PropertyOwner;

        public string           PropertyName;
        public PropertyInfo     Property;

        public MethodInfo       ConvertMethod;
        
        public DataBindingProperty(object rPropOwner, string rPropName)
            : this(rPropOwner, "", rPropName)
        {
        }

        public DataBindingProperty(object rPropOwner, string rPropOwnerKey, string rPropName)
        {
            this.PropertyOwnerKey   = rPropOwnerKey;
            this.PropertyOwner      = rPropOwner;
            this.PropertyName       = rPropName;
            this.Property           = rPropOwner?.GetType()?.GetProperty(rPropName);
        }

        public object GetValue()
        {
            var rValue = this.Property?.GetValue(this.PropertyOwner);
            if (this.ConvertMethod != null)
            {
                rValue = this.ConvertMethod.Invoke(null, new object[] { rValue });
            }
            return rValue;
        }

        public void SetValue(object rValue)
        {
            this.Property?.SetValue(this.PropertyOwner, rValue);
        }
    }
}
