using System;
using System.Collections.Generic;

namespace Knight.Core
{
    public class TypeResolveManager : TSingleton<TypeResolveManager>
    {
        private Dictionary<string, TypeResolveAssembly> mAssemblies;

        private TypeResolveManager()
        {
            this.mAssemblies = new Dictionary<string, TypeResolveAssembly>();
        }

        public void Initialize()
        {
            this.mAssemblies.Clear();
        }

        public object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            TypeResolveAssembly rAssembly = null;
            var rType = this.GetType(rTypeName, out rAssembly);
            if (rType == null) return null;

            return rAssembly.Invoke(rObj, rTypeName, rMethodName, rArgs);
        }

        public void AddAssembly(string rAssemblyName, bool bIsHotfix = false)
        {
            TypeResolveAssembly rTypeResolveAsssembly = null;
            if (!bIsHotfix)
                rTypeResolveAsssembly = new TypeResolveAssembly_Mono(rAssemblyName);
            else
                rTypeResolveAsssembly = new TypeResolveAssembly_Hotfix(rAssemblyName);
            rTypeResolveAsssembly.Load();

            if (!this.mAssemblies.ContainsKey(rAssemblyName))
            {
                this.mAssemblies.Add(rAssemblyName, rTypeResolveAsssembly);
            }
        }

        public Type[] GetTypes(string rAssemblyName)
        {
            TypeResolveAssembly rAssembly = null;
            if (this.mAssemblies.TryGetValue(rAssemblyName, out rAssembly))
            {
                return new List<Type>(rAssembly.Types.Values).ToArray();
            }
            return new Type[0];
        }

        public TypeResolveAssembly GetAssembly(Type rType)
        {
            var rTypeAssemblyName = rType.Assembly.GetName().Name;

            TypeResolveAssembly rAssembly = null;
            this.mAssemblies.TryGetValue(rTypeAssemblyName, out rAssembly);
            return rAssembly;
        }

        public Type GetType(string rTypeName)
        {
            foreach (var rPair in this.mAssemblies)
            {
                var rTypes = rPair.Value.Types;
                if (rTypes == null) continue;
                if (rTypes.TryGetValue(rTypeName, out var rType))
                {
                    return rType;
                }
            }
            return null;
        }

        public Type GetType(string rTypeName, out TypeResolveAssembly rTypeResolveAssembly)
        {
            rTypeResolveAssembly = null;
            foreach (var rPair in this.mAssemblies)
            {
                var rTypes = rPair.Value.Types;
                if (rTypes == null) continue;
                if (rTypes.TryGetValue(rTypeName, out var rType))
                {
                    rTypeResolveAssembly = rPair.Value;
                    return rType;
                }
            }
            return null;
        }

        public object Instantiate(string rTypeName, params object[] rArgs)
        {
            TypeResolveAssembly rAssembly = null;
            var rType = this.GetType(rTypeName, out rAssembly);
            if (rType == null) return null;
            if (rAssembly == null) return null;

            return rAssembly.Instantiate(rTypeName, rArgs);
        }

        public T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            TypeResolveAssembly rAssembly = null;
            var rType = this.GetType(rTypeName, out rAssembly);
            if (rType == null) return default(T);

            return rAssembly.Instantiate<T>(rTypeName, rArgs);
        }

        public object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            TypeResolveAssembly rAssembly = null;
            var rType = this.GetType(rTypeName, out rAssembly);
            if (rType == null) return null;

            return rAssembly.InvokeStatic(rTypeName, rMethodName, rArgs);
        }
    }
}
