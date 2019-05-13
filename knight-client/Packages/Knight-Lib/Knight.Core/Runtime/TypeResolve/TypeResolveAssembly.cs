using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Knight.Core
{
    public interface ITypeResolveAssemblyProxy
    {
        void Load();
        Type[] GetAllTypes();
        object Instantiate(string rTypeName, params object[] rArgs);
        T Instantiate<T>(string rTypeName, params object[] rArgs);
        object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs);
    }

    public abstract class TypeResolveAssembly
    {
        public string       AssemblyName;
        public bool         IsHotfix;

        public TypeResolveAssembly(string rAssemblyName)
        {
            this.AssemblyName = rAssemblyName;
            this.IsHotfix     = false;
        }

        public virtual void Load()
        {
        }

        public virtual Type[] GetAllTypes()
        {
            return null;
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
    }

    public class TypeResolveAssembly_Mono : TypeResolveAssembly
    {
        private Assembly    mAssembly;

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
        }

        public override Type[] GetAllTypes()
        {
            if (this.mAssembly == null) return new Type[0];
            return this.mAssembly.GetTypes();
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

            return rType.InvokeMember(rMethodName, ReflectionAssist.flags_method_inst, null, rObj, rArgs);
        }
    }

    public class TypeResolveAssembly_Hotfix : TypeResolveAssembly
    {
        public ITypeResolveAssemblyProxy Proxy;

        public TypeResolveAssembly_Hotfix(string rAssemblyName)
            : base(rAssemblyName)
        {
            this.IsHotfix = true;
            var rProxyType = System.AppDomain.CurrentDomain.GetAssemblies()
                             .Single(rAssembly => rAssembly.GetName().Name.Equals("Framework.Hotfix"))?.GetTypes()?
                             .SingleOrDefault(rType => rType.FullName.Equals("Knight.Framework.Hotfix.HotfixTypeResolveAssemblyProxy"));

            if (rProxyType != null)
            {
                this.Proxy = ReflectionAssist.Construct(rProxyType, new Type[] { typeof(string) }, rAssemblyName) as ITypeResolveAssemblyProxy;
            }
        }

        public override void Load()
        {
            this.Proxy?.Load();
        }

        public override Type[] GetAllTypes()
        {
            return this.Proxy?.GetAllTypes();
        }

        public override object Instantiate(string rTypeName, params object[] rArgs)
        {
            return this.Proxy?.Instantiate(rTypeName, rArgs);
        }

        public override T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            if (this.Proxy == null) return default(T);
            return this.Proxy.Instantiate<T>(rTypeName, rArgs);
        }

        public override object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.Proxy == null) return null;
            return this.Proxy.Invoke(rObj, rTypeName, rMethodName, rArgs);
        }
    }
}
