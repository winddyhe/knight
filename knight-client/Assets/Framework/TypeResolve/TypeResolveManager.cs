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

        public void Initialize()
        {
            this.mAssemblies.Clear();
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
                var rTypes = rPair.Value.GetAllTypes();
                if (rTypes == null) continue;

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

        public Type GetType(string rTypeName, out bool bIsHotfix)
        {
            bIsHotfix = false;
            foreach (var rPair in this.mAssemblies)
            {
                var rTypes = rPair.Value.GetAllTypes();
                for (int i = 0; i < rTypes.Length; i++)
                {
                    if (rTypes[i].FullName.Equals(rTypeName))
                    {
                        bIsHotfix = rPair.Value.IsHotfix;
                        return rTypes[i];
                    }
                }
            }
            return null;
        }

        public List<Type> GetAllTypes(bool bIsHotfix)
        {
            var rTypes = new List<Type>();
            foreach (var rPair in this.mAssemblies)
            {
                if (rPair.Value.IsHotfix == bIsHotfix)
                    rTypes.AddRange(rPair.Value.GetAllTypes());
            }
            return rTypes;
        }

        public object Instantiate(string rTypeName, params object[] rArgs)
        {
            var rType = this.GetType(rTypeName);
            if (rType == null) return null;

            var rAssembly = this.GetAssembly(rType);
            if (rAssembly == null) return null;

            return rAssembly.Instantiate(rTypeName, rArgs);
        }

        public T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            var rType = this.GetType(rTypeName);
            if (rType == null) return default(T);

            var rAssembly = this.GetAssembly(rType);
            if (rAssembly == null) return default(T);

            return rAssembly.Instantiate<T>(rTypeName, rArgs);
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ScriptsReloaded()
        {
            if (!Application.isPlaying)
            {
                TypeResolveManager.Instance.Initialize();
                TypeResolveManager.Instance.AddAssembly("Game");
                TypeResolveManager.Instance.AddAssembly("Tests");
                TypeResolveManager.Instance.AddAssembly("KnightHotfix", true);
            }
        }
#endif
    }
}
