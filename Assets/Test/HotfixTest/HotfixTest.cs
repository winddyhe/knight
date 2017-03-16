using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();
            yield return HotfixManager.Instance.Load("KnightHotfixModule");

            string rPrefabPath = "Assets/Test/HotfixTest/HotfixTest.prefab";
            
            GameObject rTestPrefab = null;
#if UNITY_EDITOR
            rTestPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(rPrefabPath, typeof(GameObject)) as GameObject;
#endif
            GameObject.Instantiate(rTestPrefab);
        }
    }
}