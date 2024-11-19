using System;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HybridCLR;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.Hotfix
{
    /// <summary>
    /// Library DLL加载
    /// </summary>
    public class HotfixApp_HybridCLR : HotfixApp
    {
        private Assembly mApp;

        public override void Dispose()
        {
            this.mApp = null;
        }

        public override async Task Load(string rABPath, string rHotfixModuleName)
        {
            if (AssetLoader.Instance.IsHotfixDebugMode)
            {
                // 直接反射出DLL的Assembly
                var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < rAllAssemblies.Length; i++)
                {
                    if (rAllAssemblies[i].GetName().Name.Equals(rHotfixModuleName))
                    {
                        this.mApp = rAllAssemblies[i];
                        break;
                    }
                }
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
                LogManager.LogRelease("HotfixApp_HybridCLR, Load dll use library dll reflect, for debug..");
            }
            else
            {
                // AB包加载Assembly
                var rRequest = AssetLoader.Instance.LoadAssetAsync<TextAsset>(rABPath, rHotfixModuleName, AssetLoader.Instance.IsSimulate(ABSimuluateType.Script));
                await rRequest.Task();
                if (rRequest.Asset != null)
                {
                    var rDLLBytes = rRequest.Asset.bytes;
                    this.mApp = Assembly.Load(rDLLBytes);
                    LogManager.LogRelease("HotfixApp_HybridCLR, Load dll use hybridclr..");
                }
                AssetLoader.Instance.Unload(rRequest);
            }
        }

        public override void LoadDLL(string rHotfixModuleName)
        {
            // 直接反射出DLL的Assembly
            var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < rAllAssemblies.Length; i++)
            {
                if (rAllAssemblies[i].GetName().Name.Equals(rHotfixModuleName))
                {
                    this.mApp = rAllAssemblies[i];
                    break;
                }
            }
            LogManager.LogRelease($"HotfixApp_HybridCLR, Load dll {rHotfixModuleName} use library dll reflect, for debug..");
        }

        public override object Instantiate(string rTypeName, params object[] rArgs)
        {
            if (this.mApp == null) return null;
            return Activator.CreateInstance(this.mApp.GetType(rTypeName), rArgs);
        }

        public override T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            if (this.mApp == null) return default(T);
            return (T)Activator.CreateInstance(this.mApp.GetType(rTypeName), rArgs);
        }

        public override object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mApp == null) return null;
            return ReflectTool.MethodMember(rObj, rMethodName, ReflectTool.flags_method_inst, rArgs);
        }

        public override object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mApp == null) return null;
            Type rObjType = this.mApp.GetType(rTypeName);
            return rObjType.InvokeMember(rMethodName, ReflectTool.flags_method_static, null, null, rArgs);
        }

        public override Type[] GetTypes()
        {
            if (this.mApp == null) return null;
            return this.mApp.GetTypes();
        }
    }
}
