//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Core.WindJson;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Hotfix
{
    public class HotfixApp : TSingleton<HotfixApp>
    {
        private AppDomain                   mApp;
        public  AppDomain                   App { get { return mApp; } }

        public  System.Action               RegisterCrossBindingAdaptorEvent;

        private HotfixApp()
        {
        }

        ~HotfixApp()
        {
            mApp = null;
        }

        public Coroutine Load(string rABPath, string rHotfixModuleName)
        {
            return CoroutineManager.Instance.Start(Load_Async(rABPath, rHotfixModuleName));
        }

        private IEnumerator Load_Async(string rABPath, string rHotfixModuleName)
        {
            var rRequest = HotfixAssetLoader.Instance.Load(rABPath, rHotfixModuleName);
            yield return rRequest;

            MemoryStream rDllMS = new MemoryStream(rRequest.dllBytes);
            MemoryStream rPDBMS = new MemoryStream(rRequest.pdbBytes);
            
            this.Initialize(rDllMS, rPDBMS);
            
            rDllMS.Close();
            rPDBMS.Close();
            rDllMS.Dispose();
            rPDBMS.Dispose();
        }

        public void Initialize(Stream rDLLStream, Stream rPDBStream)
        {
            mApp = new AppDomain();
            mApp.LoadAssembly(rDLLStream, rPDBStream, new Mono.Cecil.Pdb.PdbReaderProvider());

            // 注册Adaptor
            this.RegisterCrossBindingAdaptor();

            // 注册重定向方法
            this.RegisterCLRMethodRedirection();
            
            // 注册委托
            this.RegisterDelegates();
        }

        private void RegisterCrossBindingAdaptor()
        {
            this.mApp.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            this.mApp.RegisterCrossBindingAdaptor(new IEqualityComparerAdaptor());
            this.mApp.RegisterCrossBindingAdaptor(new IEnumerableAdaptor());
            this.mApp.RegisterCrossBindingAdaptor(new HotfixMBAdaptor());

            UtilTool.SafeExecute(this.RegisterCrossBindingAdaptorEvent);
        }

        public unsafe void RegisterCLRMethodRedirection()
        {
            //ILRuntime.Runtime.Generated.CLRBindings.Initialize(this.mApp);
        }
        
        public void RegisterDelegates()
        {
            this.mApp.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            this.mApp.DelegateManager.RegisterMethodDelegate<JsonNode, JsonNode>();
            this.mApp.DelegateManager.RegisterMethodDelegate<IEqualityComparer>();

            this.mApp.DelegateManager.RegisterFunctionDelegate<Framework.Hotfix.BaseDataObject, bool>();
            this.mApp.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Hotfix.BaseDataObject>>((act) =>
            {
                return new System.Predicate<Framework.Hotfix.BaseDataObject>((obj) =>
                {
                    return ((Func<Framework.Hotfix.BaseDataObject, bool>)act)(obj);
                });
            });

            this.mApp.DelegateManager.RegisterFunctionDelegate<                System.Collections.Generic.KeyValuePair<int, ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                System.Collections.Generic.KeyValuePair<int, ILRuntime.Runtime.Intepreter.ILTypeInstance>, 
                int>();
            this.mApp.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>>((act) =>
            {
                return new System.Comparison<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>((x, y) =>
                {
                    return ((Func<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, 
                                  System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, 
                                  int>)
                           act)(x, y);
                });
            });

            this.mApp.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        }

        public HotfixObject Instantiate(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return null;
            HotfixObject rObject = new HotfixObject(this, rTypeName);
            rObject.ILObject = this.mApp.Instantiate(rTypeName, rArgs);

            return rObject;
        }

        public T Instantiate<T>(string rTypeName, params object[] rObjs)
        {
            if (mApp == null) return default(T);
            return this.mApp.Instantiate<T>(rTypeName, rObjs);
        }

        public object Invoke(HotfixObject rHotfixObj, string rMethodName, params object[] rArgs)
        {
            if (mApp == null || rHotfixObj == null) return null;
            return this.mApp.Invoke(rHotfixObj.TypeName, rMethodName, rHotfixObj.ILObject, rArgs);
        }

        public object InvokeParent(HotfixObject rHotfixObj, string rParentType, string rMethodName, params object[] rArgs)
        {
            if (mApp == null || rHotfixObj == null) return null;
            return this.mApp.Invoke(rParentType, rMethodName, rHotfixObj.ILObject, rArgs);
        }

        public object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.Invoke(rTypeName, rMethodName, null, rArgs);
        }
    }
}
