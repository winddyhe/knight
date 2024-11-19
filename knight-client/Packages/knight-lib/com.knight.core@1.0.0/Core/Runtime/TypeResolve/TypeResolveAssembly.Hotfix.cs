using System;
using System.Linq;

namespace Knight.Core
{
    public class TypeResolveAssembly_Hotfix : TypeResolveAssembly
    {
        public ITypeResolveAssemblyProxy Proxy;

        public TypeResolveAssembly_Hotfix(string rAssemblyName)
            : base(rAssemblyName)
        {
            this.IsHotfix = true;
            var rProxyType = System.AppDomain.CurrentDomain.GetAssemblies()
                             .Single(rAssembly => rAssembly.GetName().Name.Equals("Knight.Hotfix"))?.GetTypes()?
                             .SingleOrDefault(rType => rType.FullName.Equals("Knight.Framework.Hotfix.HotfixTypeResolveAssemblyProxy"));

            if (rProxyType != null)
            {
                this.Proxy = ReflectTool.Construct(rProxyType, new Type[] { typeof(string) },  new object[] { rAssemblyName }) as ITypeResolveAssemblyProxy;
            }
        }

        public override void Load()
        {
            this.Proxy?.Load();

            // 初始化Type
            var rAllTypes = this.Proxy?.GetAllTypes();
            if (rAllTypes != null)
            {
                for (int i = 0; i < rAllTypes.Length; i++)
                {
                    var rFullName = rAllTypes[i].FullName.Replace('/', '+');
                    if (!this.Types.ContainsKey(rFullName))
                    {
                        this.Types.Add(rFullName, rAllTypes[i]);
                    }
                }
            }
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

        public override object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.Proxy == null) return null;
            return this.Proxy.InvokeStatic(rTypeName, rMethodName, rArgs);
        }
    }
}
