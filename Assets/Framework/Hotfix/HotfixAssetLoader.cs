//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.IO;
using UnityEngine.AssetBundles;
using System.Threading.Tasks;

namespace Framework.Hotfix
{
    public class HotfixLoaderRequest
    {
        public string ABPath;
        public string hotfixModuleName;

        public byte[] dllBytes;
        public byte[] pdbBytes;

        public HotfixLoaderRequest(string rABName, string rHotfixModuleName)
        {
            this.ABPath             = rABName;
            this.hotfixModuleName   = rHotfixModuleName;
        }
    }

    public class HotfixAssetLoader : TSingleton<HotfixAssetLoader>
    {
        private string mHotfixDllDir = "Assets/Game/Knight/GameAsset/Hotfix/Libs/";

        private HotfixAssetLoader() { }

        public async Task<HotfixLoaderRequest> Load(string rABPath, string rHotfixModuleName)
        {
            var rRequest = new HotfixLoaderRequest(rABPath, rHotfixModuleName);
        
            string rDLLPath = mHotfixDllDir + rRequest.hotfixModuleName + ".bytes";
            string rPDBPath = mHotfixDllDir + rRequest.hotfixModuleName + "_PDB.bytes";

            if (ABPlatform.Instance.IsSumilateMode_Script())
            {
                Debug.Log("---Simulate load ab: " + rRequest.ABPath);
                rRequest.dllBytes = File.ReadAllBytes(rDLLPath);
                rRequest.pdbBytes = File.ReadAllBytes(rPDBPath);
            }
            else
            {
                var rDLLAssetRequest = await ABLoader.Instance.LoadAsset(rRequest.ABPath, rDLLPath, false);
                if (rDLLAssetRequest.Asset != null)
                {
                    var rTextAsset = rDLLAssetRequest.Asset as TextAsset;
                    if (rTextAsset != null)
                        rRequest.dllBytes = rTextAsset.bytes;
                }
                ABLoader.Instance.UnloadAsset(rRequest.ABPath);

                var rPDBAssetRequest = await ABLoader.Instance.LoadAsset(rRequest.ABPath, rPDBPath, false);
                if (rPDBAssetRequest.Asset != null)
                {
                    var rTextAsset = rPDBAssetRequest.Asset as TextAsset;
                    if (rTextAsset != null)
                        rRequest.pdbBytes = rTextAsset.bytes;
                }
                ABLoader.Instance.UnloadAsset(rRequest.ABPath);
            }
            return rRequest;
        }
    }
}
