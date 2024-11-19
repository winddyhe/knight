using System;
using System.Reflection;

namespace Knight.Core
{
    public class TypeResolveAssembly_Mono : TypeResolveAssembly
    {
        private Assembly mAssembly;

        public TypeResolveAssembly_Mono(string rAssemblyName)
            : base(rAssemblyName)
        {
            this.IsHotfix = false;
        }

        public override void Load()
        {
            var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < rAllAssemblies.Length; i++)
            {
                if (rAllAssemblies[i].GetName().Name.Equals(this.AssemblyName))
                {
                    this.mAssembly = rAllAssemblies[i];
                    break;
                }
            }

            if (this.mAssembly != null)
            {
                var rAllTypes = this.mAssembly.GetTypes();
                for (int i = 0; i < rAllTypes.Length; i++)
                {
                    this.Types.Add(rAllTypes[i].FullName, rAllTypes[i]);
                }
            }
        }

        public override object Instantiate(string rTypeName, params object[] rArgs)
        {
            if (this.mAssembly == null) return null;
            return Activator.CreateInstance(this.mAssembly.GetType(rTypeName), rArgs);
        }

        public override T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            if (this.mAssembly == null) return default(T);
            return (T)Activator.CreateInstance(this.mAssembly.GetType(rTypeName), rArgs);
        }

        public override object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mAssembly == null) return null;
            if (rObj == null) return null;

            var rType = this.mAssembly.GetType(rTypeName);
            if (rType == null) return null;

            return rType.InvokeMember(rMethodName, BindingFlags.Instance | BindingFlags.InvokeMethod, null, rObj, rArgs);
        }

        public override object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mAssembly == null) return null;

            var rType = this.mAssembly.GetType(rTypeName);
            if (rType == null) return null;

            return rType.InvokeMember(rMethodName, BindingFlags.Static | BindingFlags.InvokeMethod, null, null, rArgs);
        }
    }
}
