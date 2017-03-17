//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System.IO;

namespace Game.Knight
{
    public class HotfixLoaderRequest : BaseCoroutineRequest<HotfixAssetLoader>
    {
        public string hotfixModuleName;

        public byte[] dllBytes;
        public byte[] pdbBytes;

        public HotfixLoaderRequest(string rHotfixModuleName)
        {
            this.hotfixModuleName = rHotfixModuleName;
        }
    }

    public class HotfixAssetLoader : TSingleton<HotfixAssetLoader>
    {
        private string mHotfixDllDir = "Assets/Game/Knight/GameAsset/Hotfix/Libs/";

        private HotfixAssetLoader() { }

        public HotfixLoaderRequest Load(string rHotfixModuleName)
        {
            var rRequest = new HotfixLoaderRequest(rHotfixModuleName);
            rRequest.Start(Load_Async(rRequest));
            return rRequest;
        }

        /// <summary>
        /// @TODO: 暂时使用读取本地文件的方式加载，后期做好完整的资源管理之后再来改为
        ///        Assetbundle加载和本地AssetDataBase加载来回切换。
        /// </summary>
        public IEnumerator Load_Async(HotfixLoaderRequest rRequest)
        {
            string rDLLPath = mHotfixDllDir + rRequest.hotfixModuleName + ".bytes";
            string rPDBPath = mHotfixDllDir + rRequest.hotfixModuleName + "_PDB.bytes";

            rRequest.dllBytes = File.ReadAllBytes(Path.GetFullPath(rDLLPath));
            rRequest.pdbBytes = File.ReadAllBytes(Path.GetFullPath(rPDBPath));

            yield return 0;
        }
    }
}
