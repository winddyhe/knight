using System;
using System.Collections.Generic;
using System.Reflection;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.TypeResolve
{
    public class TypeResolveManager : TSingleton<TypeResolveManager>
    {
        private Dict<string, TypeResolveAssembly> mAssemblies;

        private TypeResolveManager()
        {
            this.mAssemblies = new Dict<string, TypeResolveAssembly>();
        }

        public void AddAssembly(string rAssemblyName, bool bIsHotfix = false)
        {
            TypeResolveAssembly rTypeResolveAsssembly = null;
            if (bIsHotfix)
            {
                rTypeResolveAsssembly = new TypeResolveAssembly_Hotfix(rAssemblyName);
            }
            else
            {
                rTypeResolveAsssembly = new TypeResolveAssembly_Mono(rAssemblyName);
            }
            rTypeResolveAsssembly.Load();

            if (!this.mAssemblies.ContainsKey(rAssemblyName))
            {
                this.mAssemblies.Add(rAssemblyName, rTypeResolveAsssembly);
            }
        }

        public Type GetType(string rTypeName)
        {
            foreach (var rPair in this.mAssemblies)
            {
                var rTypes = rPair.Value.GetAllTypes();
                for (int i = 0; i < rTypes.Length; i++)
                {
                    if (rTypes[i].FullName.Equals(rTypeName))
                    {
                        return rTypes[i];
                    }
                }
            }
            return null;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ScriptsReloaded()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TypeResolveManager.Instance.AddAssembly("Game");
                TypeResolveManager.Instance.AddAssembly("Tests");
            }
#endif
        }
    }
}
