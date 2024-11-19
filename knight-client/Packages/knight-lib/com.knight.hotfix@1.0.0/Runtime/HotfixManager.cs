using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HybridCLR;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.Hotfix
{
    public class HotfixManager : TSingleton<HotfixManager>
    {
        private List<string> mDLLNames;
        private Dictionary<string, HotfixApp> mAppDict;

        private HotfixManager()
        {
        }

        public void Initialize(string[] rDLLNames)
        {
            Knight.Core.LogManager.LogFormat("IsHofixHybridCLRMode: {0}", this.IsHotfixHybridCLRMode());

            this.mDLLNames = new List<string>(rDLLNames);
            this.mAppDict = new Dictionary<string, HotfixApp>();
            for (int i = 0; i < this.mDLLNames.Count; i++)
            {
                if (this.IsHotfixHybridCLRMode())
                {
                    var rApp = new HotfixApp_HybridCLR();
                    this.mAppDict.Add(rDLLNames[i], rApp);
                }
                else
                {
                    var rApp = new HotfixApp_DLL();
                    this.mAppDict.Add(rDLLNames[i], rApp);
                }
            }
        }

        public async Task Load(string rABPath)
        {
            if (this.mAppDict == null) return;

            for (int i = 0; i < this.mDLLNames.Count; i++)
            {
                if (this.mAppDict.TryGetValue(this.mDLLNames[i], out var rApp))
                {
                    await rApp.Load(rABPath, this.mDLLNames[i]);
                }
            }
        }

        public void LoadDLL()
        {
            if (this.mAppDict == null) return;

            for (int i = 0; i < this.mDLLNames.Count; i++)
            {
                if (this.mAppDict.TryGetValue(this.mDLLNames[i], out var rApp))
                {
                    rApp.LoadDLL(this.mDLLNames[i]);
                }
            }
        }

        public async Task LoadAOTAsms(string rABPath)
        {
            if (this.IsHotfixHybridCLRMode())
            {
                var rRequest = AssetLoader.Instance.LoadAllAssetAsync<TextAsset>(rABPath, AssetLoader.Instance.IsSimulate(ABSimuluateType.Script));
                await rRequest.Task();
                if (rRequest != null)
                {
                    foreach (var rAOTDLLAsset in rRequest.AllAssets)
                    {
                        var rErrorCode = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(rAOTDLLAsset.bytes, HomologousImageMode.SuperSet);
                    }
                    LogManager.LogRelease($"LoadMetadataForAOTAssembly {rABPath} success..");
                }
            }
        }

        public void Dispose()
        {
            foreach (var rAppPair in this.mAppDict)
            {
                rAppPair.Value?.Dispose();
            }
        }

        public object Instantiate(string rDLLModuleName, string rTypeName, params object[] rArgs)
        {
            if (this.mAppDict == null) return null;

            if (this.mAppDict.TryGetValue(rDLLModuleName, out var rApp))
            {
                return rApp.Instantiate(rTypeName, rArgs);
            }
            return null;
        }

        public T Instantiate<T>(string rDLLModuleName, string rTypeName, params object[] rArgs)
        {
            if (this.mAppDict == null) return default(T);

            if (this.mAppDict.TryGetValue(rDLLModuleName, out var rApp))
            {
                return rApp.Instantiate<T>(rTypeName, rArgs);
            }
            return default(T);
        }

        public object Invoke(string rDLLModuleName, object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mAppDict == null) return null;

            if (this.mAppDict.TryGetValue(rDLLModuleName, out var rApp))
            {
                return rApp.Invoke(rObj, rTypeName, rMethodName, rArgs);
            }
            return null;
        }

        public object InvokeStatic(string rDLLModuleName, string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (this.mAppDict == null) return null;

            if (this.mAppDict.TryGetValue(rDLLModuleName, out var rApp))
            {
                return rApp.InvokeStatic(rTypeName, rMethodName, rArgs);
            }
            return null;
        }

        public Type[] GetTypes(string rDLLModuleName)
        {
            if (this.mAppDict == null) return null;

            if (this.mAppDict.TryGetValue(rDLLModuleName, out var rApp))
            {
                return rApp.GetTypes();
            }
            return null;
        }

        public bool IsHotfixHybridCLRMode()
        {
#if HOTFIX_HYBRIDCLR_MODE
            return true;
#else
            return false;
#endif
        }
    }
}
