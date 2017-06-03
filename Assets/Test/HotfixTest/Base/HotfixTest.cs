using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;
using Framework.WindUI;
using Framework.Hotfix;
using System.Collections.Generic;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        public Canvas Canvas;

        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();
            yield return HotfixApp.Instance.Load("game/knight.ab", "KnightHotfixModule");

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
    }
}