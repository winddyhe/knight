using System;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Knight.Core;

namespace Knight.Framework.Hotfix
{
    /// <summary>
    /// Library DLL加载
    /// </summary>
    public class HotfixApp_DLL : HotfixApp
    {
        private Assembly mApp;

        public override void Dispose()
        {
            this.mApp = null;
        }

        public override async Task Load(string rABPath, string rHotfixModuleName)
        {
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
            LogManager.LogRelease("HotfixApp_DLL, Load dll use dll path..");
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
            LogManager.LogRelease("HotfixApp_DLL, Load dll use library dll reflect, for debug..");
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
