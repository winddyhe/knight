using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core
{
    public class TypeResolveManager : TSingleton<TypeResolveManager>
    {
        private Dict<string, TypeResolveAssembly>   mAssemblies;

        private TypeResolveManager()
        {
            this.mAssemblies = new Dict<string, TypeResolveAssembly>();
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

        public Type[] GetTypes(string rAssemblyName)
        {
            TypeResolveAssembly rAssembly = null;
            if (this.mAssemblies.TryGetValue(rAssemblyName, out rAssembly))
            {
                return rAssembly.GetAllTypes();
            }
            return null;
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

        public Type GetType(string rTypeName, out TypeResolveAssembly rTypeResolveAssembly)
        {
            rTypeResolveAssembly = null;
            foreach (var rPair in this.mAssemblies)
            {
                var rTypes = rPair.Value.GetAllTypes();
                for (int i = 0; i < rTypes.Length; i++)
                {
                    if (rTypes[i].FullName.Equals(rTypeName))
                    {
                        rTypeResolveAssembly = rPair.Value;
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
