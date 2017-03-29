//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.IO;

namespace Framework.Hotfix
{
    public class HotfixManager : TSingleton<HotfixManager>
    {
        private HotfixApp       mApp;
        public  HotfixApp       App { get { return mApp; } }
        
        private HotfixManager() { }

        public Coroutine Load(string rHotfixModuleName)
        {
            return CoroutineManager.Instance.Start(Load_Async(rHotfixModuleName));
        }

        private IEnumerator Load_Async(string rHotfixModuleName)
        {
            var rRequest = HotfixAssetLoader.Instance.Load(rHotfixModuleName);
            yield return rRequest.Coroutine;
            
            MemoryStream rDllMS = new MemoryStream(rRequest.dllBytes);
            MemoryStream rPDBMS = new MemoryStream(rRequest.pdbBytes);

            mApp = new HotfixApp();
            this.RegisterCrossBindingAdaptor(mApp); // 注册Adaptor
            mApp.Initialize(rDllMS, rPDBMS);

            
            rDllMS.Dispose();
            rDllMS.Dispose();
        }

        private void RegisterCrossBindingAdaptor(HotfixApp rApp)
        {
            rApp.RegisterCrossBindingAdaptorEvent = () =>
            {
                rApp.App.RegisterCrossBindingAdaptor(new MonoBehaviourProxyAdaptor());
                rApp.App.RegisterCrossBindingAdaptor(new CoroutineAdapter());
                rApp.App.RegisterCrossBindingAdaptor(new ViewControllerAdaptor());
            };
        }
    }
}
