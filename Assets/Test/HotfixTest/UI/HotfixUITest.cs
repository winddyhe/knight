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

            MemoryStream rDllMS = new MemoryStream(File.ReadAllBytes(rDLLPath));
            MemoryStream rPDBMS = new MemoryStream(File.ReadAllBytes(rPDBPath));

            HotfixApp.Instance.Initialize(rDllMS, rPDBMS);

            rDllMS.Close();
            rPDBMS.Close();
            rDllMS.Dispose();
            rPDBMS.Dispose();
        }
    }
}