//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Core.WindJson;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Hotfix
{
    public class HotfixApp_ILRT : HotfixApp
    {
        private AppDomain               mApp;
        
        public HotfixApp_ILRT()
        {
        }

        ~HotfixApp_ILRT()
        {
            mApp = null;
        }

        public override async Task Load(string rABPath, string rHotfixModuleName)
        {
            var rRequest = await HotfixAssetLoader.Instance.Load(rABPath, rHotfixModuleName);
            this.InitApp(rRequest.DllBytes, rRequest.PdbBytes);
        }

        public override void InitApp(byte[] rDLLBytes, byte[] rPDBBytes)
        {
            mApp = new AppDomain();

            MemoryStream rDLLMS = (rDLLBytes != null) ? new MemoryStream(rDLLBytes) : null;
            MemoryStream rPDBMS = (rPDBBytes != null) ? new MemoryStream(rPDBBytes) : null;

            mApp.LoadAssembly(rDLLMS, rPDBMS, new Mono.Cecil.Pdb.PdbReaderProvider());

            rDLLMS?.Close();
            rPDBMS?.Close();
            rDLLMS?.Dispose();
            rPDBMS?.Dispose();

            // 注册Value Type Binder
            this.RegisterValueTypeBinder();

            // 注册Adaptor
            this.RegisterCrossBindingAdaptor();
            
            // 注册重定向方法
            this.RegisterCLRMethodRedirection();

            // 注册委托
            this.RegisterDelegates();
        }

        public override void InitApp(string rDLLPath, string rPDBPath)
        {
            byte[] rDLLBytes = File.Exists(rDLLPath) ? File.ReadAllBytes(rDLLPath) : null;
            byte[] rPDBBytes = File.Exists(rPDBPath) ? File.ReadAllBytes(rPDBPath) : null;
            InitApp(rDLLBytes, rPDBBytes);
        }

        private void RegisterCrossBindingAdaptor()
        {
            this.mApp.RegisterCrossBindingAdaptor(new CoroutineAdaptor());
            this.mApp.RegisterCrossBindingAdaptor(new IEqualityComparerAdaptor());
            this.mApp.RegisterCrossBindingAdaptor(new IEnumerableAdaptor());
            this.mApp.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
        }

        private unsafe void RegisterCLRMethodRedirection()
        {
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(this.mApp);
        }

        private unsafe void RegisterValueTypeBinder()
        {
            this.mApp.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            this.mApp.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            this.mApp.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
        }

        private void RegisterDelegates()
        {
            this.mApp.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            this.mApp.DelegateManager.RegisterMethodDelegate<JsonNode, JsonNode>();
            this.mApp.DelegateManager.RegisterMethodDelegate<IEqualityComparer>();

            this.mApp.DelegateManager.RegisterFunctionDelegate<Framework.Hotfix.BaseDataObject, System.Boolean>();
            this.mApp.DelegateManager.RegisterDelegateConvertor<System.Predicate<Framework.Hotfix.BaseDataObject>>((act) =>
            {
                return new System.Predicate<Framework.Hotfix.BaseDataObject>((obj) =>
                {
                    return ((Func<Framework.Hotfix.BaseDataObject, System.Boolean>)act)(obj);
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

            this.mApp.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
            this.mApp.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });

            this.mApp.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            this.mApp.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();

            this.mApp.DelegateManager.RegisterMethodDelegate<Model.AChannel, System.Net.Sockets.SocketError>();
            this.mApp.DelegateManager.RegisterMethodDelegate<System.Object>();
            this.mApp.DelegateManager.RegisterFunctionDelegate<Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, 
                                                               Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, 
                                                               System.Int32>();
            this.mApp.DelegateManager.RegisterDelegateConvertor<System.Comparison<Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>>((act) =>
            {
                return new System.Comparison<Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>>((x, y) =>
                {
                    return ((Func<Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                                  Core.CKeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                                  System.Int32>)
                           act)(x, y);
                });
            });
        }

        public override HotfixObject Instantiate(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return null;
            var rObject = new HotfixObject(this, rTypeName);
            rObject.Object = this.mApp.Instantiate(rTypeName, rArgs);
            return rObject;
        }

        public override T Instantiate<T>(string rTypeName, params object[] rObjs)
        {
            if (mApp == null) return default(T);
            return this.mApp.Instantiate<T>(rTypeName, rObjs);
        }

        public override object Invoke(HotfixObject rHotfixObj, string rMethodName, params object[] rArgs)
        {
            if (mApp == null || rHotfixObj == null) return null;
            return this.mApp.Invoke(rHotfixObj.TypeName, rMethodName, rHotfixObj.Object, rArgs);
        }

        public override object InvokeParent(HotfixObject rHotfixObj, string rParentType, string rMethodName, params object[] rArgs)
        {
            if (mApp == null || rHotfixObj == null) return null;
            return this.mApp.Invoke(rParentType, rMethodName, rHotfixObj.Object, rArgs);
        }

        public override object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.Invoke(rTypeName, rMethodName, null, rArgs);
        }

        public override Type[] GetTypes()
        {
            if (mApp == null) return null;
            return mApp.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
        }
    }
}
