using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;
using Framework.WindUI;
using Framework.Hotfix;
using System.Collections.Generic;
using System.IO;
using System;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        public Canvas Canvas;

        public void Test(UnityEngine.Object rObj)
        {
            Debug.Log("Test");
        }

        void Start()
        {
            var rMethodInfos = this.GetType().GetMethods();
            for (int i = 0; i < rMethodInfos.Length; i++)
            {
                if (rMethodInfos[i].Name == "Test")
                {
                    Delegate rDelegate = Delegate.CreateDelegate(typeof(Action<UnityEngine.Object>), this, rMethodInfos[i]);
                    Action<UnityEngine.Object> rActionDelegate = rDelegate as Action<UnityEngine.Object>;

                    Debug.LogError("Te1");
                    rActionDelegate(null);
                }
            }

            CoroutineManager.Instance.Initialize();

            LoadHotfixDLL();
            string rPrefabPath = "Assets/Test/HotfixTest/Base/HotfixTest4.prefab";

            GameObject rTestPrefab = null;
#if UNITY_EDITOR
            rTestPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(rPrefabPath, typeof(GameObject)) as GameObject;
#endif
            //this.Canvas.transform.AddChild(rTestPrefab, "UI");
            GameObject.Instantiate(rTestPrefab);

            HotfixObject rHotfixObj = HotfixManager.Instance.Instantiate("WindHotfix.Test.Class3");
            rHotfixObj.Invoke("Test");
            rHotfixObj.InvokeParent("WindHotfix.Test1.TClass3`1", "Test");
        }

        private void LoadHotfixDLL()
        {
            string rDLLPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes";
            string rPDBPath = "Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule_PDB.bytes";
            
            HotfixManager.Instance.InitApp(File.ReadAllBytes(rDLLPath), File.ReadAllBytes(rPDBPath));
        }
    }
}