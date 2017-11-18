//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;
using Core;
using Framework.Hotfix;
using System.IO;
using UnityEngine.EventSystems;

namespace Test
{
    public class HotfixUITest : MonoBehaviour
    {
        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();

            // 加载热更新DLL
            //this.LoadHotfixDLL();

            //// 加载界面
            //var rRequest = Resources.LoadAsync("HotfixLogin");
            //yield return rRequest;
            //UIManager.Instance.OpenView("HotfixLogin", rRequest.asset as GameObject, View.State.dispatch, null);

            yield break;
        }

        private void LoadHotfixDLL()
        {
            string rDLLPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";
            string rPDBPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule_PDB.bytes";

            HotfixManager.Instance.InitApp(File.ReadAllBytes(rDLLPath), File.ReadAllBytes(rPDBPath));
        }
    }
}