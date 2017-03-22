using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        public Canvas Canvas;

        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();
            yield return HotfixManager.Instance.Load("KnightHotfixModule");

            string rPrefabPath = "Assets/Test/HotfixTest/HotfixTest1.prefab";
            
            GameObject rTestPrefab = null;
#if UNITY_EDITOR
            rTestPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(rPrefabPath, typeof(GameObject)) as GameObject;
#endif
            var rTestGo = UtilTool.CreateGameObject(rTestPrefab);
            rTestGo.transform.SetParent(this.Canvas.transform, false);
            //GameObject.Instantiate(rTestPrefab);
        }
    }
}