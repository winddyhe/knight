using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Callbacks;
using Knight.Core;

namespace UnityEditor.UI
{
    public class UIAssistEditor
    {
        private const string UI_ROOT_PATH = "Assets/Game/GameAsset/GUI/UIRoot.prefab";

        private const string UI_PREFAB_FOLDER = "Assets/Game/GameAsset/GUI/Prefabs";

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
            if (!UtilTool.PathIsSame(rCatalog, UI_PREFAB_FOLDER))
            {
                return false;
            }

            //场景中是否存在Active的UIRoot
            GameObject rUIRoot = GameObject.Find("UIRoot");

            if (!rUIRoot)
            {
                //创建UIRoot
                rUIRoot = AssetDatabase.LoadAssetAtPath(UI_ROOT_PATH, typeof(GameObject)) as GameObject;
                PrefabUtility.InstantiatePrefab(rUIRoot);
            }

            //寻找根节点
            var rPopupRoot = GameObject.Find("__popupRoot").transform;
            var rDynamicRoot = GameObject.Find("__dynamicRoot").transform;

            //获取选择的prefab
            var rUIPrefab = AssetDatabase.LoadAssetAtPath(rAssetPath, typeof(GameObject)) as GameObject;
            //获取脚本
            var rViewContainer = rUIPrefab.GetComponent<ViewContainer>();

            //if (rViewContainer)
            //{
            //    var curState = rViewContainer.CurState;
            //    var rInstance = null as GameObject;
            //    bool bIsDynamic = curState == ViewState.dispatch || curState == ViewState.overlap || curState == ViewState.fixing;
            //    AddItem(bIsDynamic, rDynamicRoot, rPopupRoot, rUIPrefab, out rInstance);
            //}

            return true;
        }

        private static void AddItem(bool bIsDynamic, Transform rDynamicRoot, Transform rPopupRoot, GameObject rUIPrefab, out GameObject rInstance)
        {
            Transform rDynamicGo = rDynamicRoot.Find(rUIPrefab.name);
            Transform rPopupGo = rPopupRoot.Find(rUIPrefab.name);

            bool bDynamicExist = (rDynamicGo != null);
            bool bPopupExist = (rPopupGo != null);

            //root下存在就删除该物体
            if (bDynamicExist)
                GameObject.DestroyImmediate(rDynamicGo.gameObject, true);
            if (bPopupExist)
                GameObject.DestroyImmediate(rPopupGo.gameObject, true);

            //创建
            if (bIsDynamic)
            {
                rInstance = PrefabUtility.InstantiatePrefab(rUIPrefab) as GameObject;
                rInstance.transform.SetParent(rDynamicRoot);
            }
            else
            {
                rInstance = PrefabUtility.InstantiatePrefab(rUIPrefab) as GameObject;
                rInstance.transform.SetParent(rPopupRoot);
            }
            rInstance.transform.localScale = Vector3.one;
            rInstance.transform.localPosition = Vector3.zero;
            rInstance.transform.localRotation = Quaternion.identity;

            Selection.activeObject = rInstance;
        }
    }
}