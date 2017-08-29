using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;
using Framework.WindUI;
using Framework.Hotfix;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        public Canvas Canvas;

        void Start()
        {
            CoroutineManager.Instance.Initialize();
            
            LoadHotfixDLL();
            string rPrefabPath = "Assets/Test/HotfixTest/Base/HotfixTest4.prefab";

            GameObject rTestPrefab = null;
#if UNITY_EDITOR
            rTestPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(rPrefabPath, typeof(GameObject)) as GameObject;
#endif
            //this.Canvas.transform.AddChild(rTestPrefab, "UI");
            GameObject.Instantiate(rTestPrefab); 

            //HotfixObject rHotfixObj = HotfixApp.Instance.Instantiate("WindHotfix.Test.Class3");
            //rHotfixObj.Invoke("Test");
            //rHotfixObj.InvokeParent("WindHotfix.Test1.TClass3`1", "Test");
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