using System;

namespace Knight.Framework.UI
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class DataBindingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class DataBindingEventAttribute : Attribute
    {
        public bool IsListEvent { get; set; }

        public DataBindingEventAttribute(bool bIsListEvent)
        {
            this.IsListEvent = bIsListEvent;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataBindingRelatedAttribute : Attribute
    {
        public string PropName { get; set; }

        public DataBindingRelatedAttribute(string rPropName)
        {
            this.PropName = rPropName;
        }
    }
}
