using System;
using System.Collections.Generic;

namespace Knight.Core
{
    public interface ITypeResolveAssemblyProxy
    {
        void Load();
        Type[] GetAllTypes();
        object Instantiate(string rTypeName, params object[] rArgs);
        T Instantiate<T>(string rTypeName, params object[] rArgs);
        object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs);
        object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs);
    }

    public abstract class TypeResolveAssembly
    {
        public bool IsHotfix;
        public string AssemblyName;
        public Dictionary<string, Type> Types;

        public TypeResolveAssembly(string rAssemblyName)
        {
            this.AssemblyName = rAssemblyName;
            this.Types = new Dictionary<string, Type>();
        }

        public virtual void Load()
        {
        }

        public virtual object Instantiate(string rTypeName, params object[] rArgs)
        {
            return null;
        }

        public virtual T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            return default(T);
        }

        public virtual object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return null;
        }

        public virtual object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            return null;
        }
    }
}
