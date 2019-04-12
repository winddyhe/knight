using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knight.Core;
using UnityEngine;

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
#if UNITY_EDITOR
            // 编辑器下初始化
            if (!Application.isPlaying)
            {
                string rDLLPath = HotfixManager.HotfixDllDir + this.mAssemblyName + ".bytes";
                string rPDBPath = HotfixManager.HotfixDllDir + this.mAssemblyName + "_PDB.bytes";

                var rDLLBytes = File.ReadAllBytes(rDLLPath);
                var rPDBBytes = File.ReadAllBytes(rPDBPath);

                HotfixManager.Instance.Initialize();
                HotfixManager.Instance.InitApp(rDLLBytes, rPDBBytes);
            }
#endif
        }

        public Type[] GetAllTypes()
        {
            return HotfixManager.Instance.GetTypes();
        }

        public object Instantiate(string rTypeName, params object[] rArgs)
        {
            return HotfixManager.Instance.Instantiate(rTypeName, rArgs);
        }

        public T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            return HotfixManager.Instance.Instantiate<T>(rTypeName, rArgs);
        }

        public object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return HotfixManager.Instance.Invoke(rObj, rTypeName, rMethodName, rArgs);
        }
    }
}
