using System;
using Knight.Core;

namespace Knight.Framework.Hotfix
{
    public class HotfixTypeResolveAssemblyProxy : ITypeResolveAssemblyProxy
    {
        private string mAssemblyName;

        public HotfixTypeResolveAssemblyProxy(string rAssemblyName)
        {
            this.mAssemblyName = rAssemblyName;
        }

        public void Load()
        {
        }

        public Type[] GetAllTypes()
        {
            return HotfixManager.Instance.GetTypes(this.mAssemblyName);
        }

        public object Instantiate(string rTypeName, params object[] rArgs)
        {
            return HotfixManager.Instance.Instantiate(this.mAssemblyName, rTypeName, rArgs);
        }

        public T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            return HotfixManager.Instance.Instantiate<T>(this.mAssemblyName, rTypeName, rArgs);
        }

        public object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return HotfixManager.Instance.Invoke(this.mAssemblyName, rObj, rTypeName, rMethodName, rArgs);
        }

        public object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            return HotfixManager.Instance.InvokeStatic(this.mAssemblyName, rTypeName, rMethodName, rArgs);
        }
    }
}
