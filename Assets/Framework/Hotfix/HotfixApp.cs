using Core.WindJson;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using System.IO;

namespace Framework.Hotfix
{
    public class HotfixApp
    {
        private AppDomain                   mApp;
        public  AppDomain                   App { get { return mApp; } }

        public  System.Action               RegisterCrossBindingAdaptorEvent;

        public void Initialize(Stream rDLLStream, Stream rPDBStream)
        {
            mApp = new AppDomain();
            mApp.LoadAssembly(rDLLStream, rPDBStream, new Mono.Cecil.Pdb.PdbReaderProvider());

            // 注册代理类
            if (this.RegisterCrossBindingAdaptorEvent != null)
                this.RegisterCrossBindingAdaptorEvent();

            // 注册重定向方法
            this.RegisterCLRMethodRedirection();

            this.mApp.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            this.mApp.DelegateManager.RegisterMethodDelegate<JsonNode, JsonNode>();
            this.mApp.DelegateManager.RegisterFunctionDelegate<Framework.GameMode>();
        }

        public HotfixObject CreateInstance(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return null;
            HotfixObject rObject = new HotfixObject(mApp, rTypeName);
            if (rObject == null) return rObject;

            rObject.CreateInstance(rArgs);
            return rObject;
        }

        public T CreateInstance<T>(string rTypeName, params object[] rObjs)
        {
            if (mApp == null) return default(T);
            return this.mApp.Instantiate<T>(rTypeName, rObjs);
        }

        public object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.Invoke(rTypeName, rMethodName, null, rArgs);
        }

        public unsafe void RegisterCLRMethodRedirection()
        {
            //ILRuntime.Runtime.Generated.CLRBindings.Initialize(this.mApp);
        }
    }
}
