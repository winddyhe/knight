using System;

namespace Knight.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonPathAttribute : Attribute
    {
        public string Path;

        public JsonPathAttribute(string rPath) 
        {
            this.Path = rPath;
        }
    }
}

