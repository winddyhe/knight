//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.IO;
using Framework.Hotfix;

namespace Game.Knight
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
            mApp.Initialize(rDllMS, rPDBMS);
            
            rDllMS.Dispose();
            rDllMS.Dispose();
        }

        private string mHotfixDllDir = "Assets/Game/Knight/GameAsset/Hotfix/Libs/";
        public void Load()
        {
            string rHotfixModuleName = "KnightHotfixModule";
            string rDLLPath = mHotfixDllDir + rHotfixModuleName + ".bytes";
            string rPDBPath = mHotfixDllDir + rHotfixModuleName + "_PDB.bytes";

            var dllBytes = File.ReadAllBytes(Path.GetFullPath(rDLLPath));
            var pdbBytes = File.ReadAllBytes(Path.GetFullPath(rPDBPath));
            MemoryStream rDllMS = new MemoryStream(dllBytes);
            MemoryStream rPDBMS = new MemoryStream(pdbBytes);

            mApp = new HotfixApp();
            mApp.Initialize(rDllMS, rPDBMS);

            rDllMS.Dispose();
            rDllMS.Dispose();
        }
    }
}
