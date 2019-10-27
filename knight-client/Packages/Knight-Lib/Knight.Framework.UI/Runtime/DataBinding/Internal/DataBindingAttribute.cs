using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class DataBindingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DataBindingConvertAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataBindingRelatedAttribute : Attribute
    {
        public string RelatedProps;

        public DataBindingRelatedAttribute(string rRelatedProps)
        {
            this.RelatedProps = rRelatedProps;
        }
    }
}
