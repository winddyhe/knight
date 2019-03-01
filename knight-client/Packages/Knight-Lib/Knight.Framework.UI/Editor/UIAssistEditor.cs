using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Callbacks;
using Knight.Core;
using System.Collections.Generic;

namespace UnityEditor.UI
{
    public class UIAssistEditor
    {
        private static readonly string  mUIRootPath         = "Assets/Game/GameAsset/GUI/UIRoot.prefab";
        private static readonly string  mUIPrefabAssetsDir  = "Assets/Game/GameAsset/GUI/Prefabs";

        [MenuItem("Assets/Select GameObject Active &a")]
        public static void SelectGameObjectAcitve()
        {
            List<GameObject> rSelectGos = new List<GameObject>();
            if (Selection.activeGameObject != null)
                rSelectGos.Add(Selection.activeGameObject);
            if (Selection.gameObjects != null)
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    var rTempGo = Selection.gameObjects[i];
                    if (rTempGo != null && !rSelectGos.Contains(rTempGo))
                        rSelectGos.Add(rTempGo);
                }
            }

            if (rSelectGos.Count == 0) return;

            for (int i = 0; i < rSelectGos.Count; i++)
            {
                rSelectGos[i].SetActive(!rSelectGos[i].activeSelf);
            }
        }

        [OnOpenAsset(0)]
        public static bool CreatUIPrefabInstance(int nInstanceId, int nLine)
        {
            if (Selection.activeObject == null) return false;
            if (!(Selection.activeObject is GameObject)) return false;

            //获取选定路径
            var rAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(rAssetPath)) return false;
            var rCatalog = UtilTool.GetParentPath(rAssetPath);

            //UIPrefab是否在指定路径下
            if (!UtilTool.PathIsSame(rCatalog, mUIPrefabAssetsDir))
            {
                return false;
            }

            //场景中是否存在Active的UIRoot
            GameObject rUIRoot = GameObject.Find("UIRoot");

            if (!rUIRoot)
            {
                //创建UIRoot
                rUIRoot = AssetDatabase.LoadAssetAtPath(mUIRootPath, typeof(GameObject)) as GameObject;
                PrefabUtility.InstantiatePrefab(rUIRoot);
            }

            //寻找根节点
            var rDynamicRoot = GameObject.Find("__dynamicRoot").transform;

            //获取选择的prefab
            var rUIPrefab = AssetDatabase.LoadAssetAtPath(rAssetPath, typeof(GameObject)) as GameObject;

            //获取脚本
            AddItem(rDynamicRoot, rUIPrefab);

            return true;
        }

        private static void AddItem(Transform rDynamicRoot, GameObject rUIPrefab)
        {
            //root下存在就删除该物体
            Transform rExistGo = rDynamicRoot.Find(rUIPrefab.name);
            if (rExistGo != null)
                GameObject.DestroyImmediate(rExistGo.gameObject, true);
            
            //创建
            var rInstance = PrefabUtility.InstantiatePrefab(rUIPrefab) as GameObject;
            rInstance.transform.SetParent(rDynamicRoot, false);
            
            rInstance.transform.localScale = Vector3.one;
            rInstance.transform.localPosition = Vector3.zero;
            rInstance.transform.localRotation = Quaternion.identity;

            Selection.activeObject = rInstance;
        }
    }
}