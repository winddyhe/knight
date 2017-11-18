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
        public string ModuleName;

        public byte[] DllBytes;
        public byte[] PdbBytes;

        public string DLLPath;
        public string PDBPath;

        public HotfixLoaderRequest(string rABName, string rModuleName)
        {
            this.ABPath     = rABName;
            this.ModuleName = rModuleName;
        }
    }

    public class HotfixAssetLoader : TSingleton<HotfixAssetLoader>
    {
        private string mHotfixDllDir        = "Assets/Game/Knight/GameAsset/Hotfix/Libs/";
        private string mHotfixDebugDllDir   = "Assets/StreamingAssets/Temp/Libs/";

        private HotfixAssetLoader() { }

        public async Task<HotfixLoaderRequest> Load(string rABPath, string rHotfixModuleName)
        {
            var rRequest = new HotfixLoaderRequest(rABPath, rHotfixModuleName);

            string rDLLPath = mHotfixDllDir + rRequest.ModuleName + ".bytes";
            string rPDBPath = mHotfixDllDir + rRequest.ModuleName + "_PDB.bytes";

#if UNITY_EDITOR
            // 编辑器下的HotfixDebug模式直接加载DLL文件，方便做断点调试
            if (HotfixManager.Instance.IsHotfixDebugMode())
            {
                rDLLPath = mHotfixDebugDllDir + rRequest.ModuleName + ".dll";
                rPDBPath = mHotfixDebugDllDir + rRequest.ModuleName + ".pdb";

                Debug.Log("---Simulate load hotfix dll: " + rDLLPath);
                rRequest.DLLPath = rDLLPath;
                rRequest.PDBPath = rPDBPath;

                return rRequest;
            }
#endif

            if (ABPlatform.Instance.IsSumilateMode_Script())
            {
                Debug.Log("---Simulate load ab: " + rRequest.ABPath);
                rRequest.DllBytes = File.ReadAllBytes(rDLLPath);
                rRequest.PdbBytes = File.ReadAllBytes(rPDBPath);
            }
            else
            {
                var rDLLAssetRequest = await ABLoader.Instance.LoadAsset(rRequest.ABPath, rDLLPath, false);
                if (rDLLAssetRequest.Asset != null)
                {
                    var rTextAsset = rDLLAssetRequest.Asset as TextAsset;
                    if (rTextAsset != null)
                        rRequest.DllBytes = rTextAsset.bytes;
                }
                ABLoader.Instance.UnloadAsset(rRequest.ABPath);

                var rPDBAssetRequest = await ABLoader.Instance.LoadAsset(rRequest.ABPath, rPDBPath, false);
                if (rPDBAssetRequest.Asset != null)
                {
                    var rTextAsset = rPDBAssetRequest.Asset as TextAsset;
                    if (rTextAsset != null)
                        rRequest.PdbBytes = rTextAsset.bytes;
                }
                ABLoader.Instance.UnloadAsset(rRequest.ABPath);
            }
            return rRequest;
        }
    }
}
