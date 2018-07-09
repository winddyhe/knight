using System;
using System.Collections.Generic;
using System.Reflection;
using Knight.Framework.Hotfix;
using UnityEngine;

namespace Knight.Framework.TypeResolve
{
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
    }

    public class TypeResolveAssembly_Hotfix : TypeResolveAssembly
    {
        public TypeResolveAssembly_Hotfix(string rAssemblyName)
            : base(rAssemblyName)
        {
            this.IsHotfix = true;
        }

        public override void Load()
        {
#if UNITY_EDITOR
            // 编辑器下初始化
            if (!Application.isPlaying)
            {
                string rDLLPath = HotfixManager.HotfixDLLDir + "/" + this.AssemblyName + ".dll";
                string rPDBPath = HotfixManager.HotfixDLLDir + "/" + this.AssemblyName + ".pdb";
                HotfixManager.Instance.InitApp(rDLLPath, rPDBPath);
            }
#else
#endif
        }

        public override Type[] GetAllTypes()
        {
            return HotfixManager.Instance.GetTypes();
        }
    }
}
