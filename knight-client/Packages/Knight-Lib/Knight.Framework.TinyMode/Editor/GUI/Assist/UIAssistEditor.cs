using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Callbacks;
using Knight.Core;
using System.Collections.Generic;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;

namespace Knight.Framework.TinyMode.UI.Editor
{
    public static class UIAssistEditor
    {
        private static readonly string  mUIRootPath         = "Assets/Game/Resources/GUI/UIRoot.prefab";
        private static readonly string  mUIPrefabAssetsDir  = "Assets/Game/Resources/GUI/Prefabs";

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

                SavePrefab();
            }
        }

        public static void SavePrefab()
        {
            // 使用反射Hack 保存预制件的方法
            var rPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var rBindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod;
            if (rPrefabStage != null)
                rPrefabStage.GetType().InvokeMember("SavePrefab", rBindingFlags, null, rPrefabStage, null);
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
        
        [MenuItem("GameObject/UI/WText")]
        public static void CreateWText()
        {
            GameObject rSelectGo = Selection.activeGameObject;

            GameObject rGo = new GameObject("Text", typeof(WText));
            Undo.RegisterCreatedObjectUndo(rGo, "Create WText");
            rGo.transform.SetParent(rSelectGo.transform);
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localScale = Vector3.one;

            var rText = rGo.GetComponent<WText>();
            rText.text = "New Text";
            rText.color = Color.black;
            rText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
            rText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);

            Selection.activeGameObject = rGo;
        }

        [MenuItem("Tools/GUI/WText replace Text")]
        public static void WTextReplaceText()
        {
            var rTextGUID  = "  m_Script: {fileID: 708705254, guid: f70555f144d8491a825f0804e09c671c, type: 3}";
            var rWTextGUID = "  m_Script: {fileID: 11500000, guid: 6477893cb80bbbc4caf184a3b554a800, type: 3}";

            NewCompReplaceOldComp(rTextGUID, rWTextGUID);
        }

        private static void NewCompReplaceOldComp(string rOldGUID, string rNewGUID)
        {
            var rGUIDS = AssetDatabase.FindAssets("t:Prefab", new string[] { mUIPrefabAssetsDir });
            for (int i = 0; i < rGUIDS.Length; i++)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDS[i]);
                var rPrefabText = File.ReadAllText(rAssetPath, System.Text.Encoding.UTF8);

                var rStringBuilder = new StringBuilder();
                using (var sr = new StringReader(rPrefabText))
                {
                    while(sr.Peek() > -1)
                    {
                        var rLine = sr.ReadLine();
                        if (rLine.Contains(rOldGUID))
                        {
                            rLine = rLine.Replace(rOldGUID, rNewGUID);
                        }
                        rStringBuilder.AppendLine(rLine);
                    }
                }
                File.WriteAllText(rAssetPath, rStringBuilder.ToString());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}