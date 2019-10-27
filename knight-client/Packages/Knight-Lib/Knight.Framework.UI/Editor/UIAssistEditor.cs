using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Callbacks;
using Knight.Core;
using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Text;

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
            if (!rCatalog.Contains(mUIPrefabAssetsDir))
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

        [MenuItem("Assets/AddButtonAssist")]
        [MenuItem("Tools/GUI/AddButtonAssist")]
        public static void AddButtonAssist()
        {
            var rAssetPaths = new HashSet<string>();
            rAssetPaths.Add(AssetDatabase.GetAssetPath(Selection.activeGameObject));
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                rAssetPaths.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
            }
            foreach (var rAssetPath in rAssetPaths)
            {
                AddButtonAssist(rAssetPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void AddButtonAssist(string rAssetPath)
        {
            var rUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rAssetPath) as GameObject;
            if (rUIPrefab == null) return;

            var rButtonAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Game/GameAsset/GUI/Animations/Button.controller");
            var rUIGo = GameObject.Instantiate(rUIPrefab);
            var rAllButtons = rUIGo.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < rAllButtons.Length; i++)
            {
                rAllButtons[i].transition = Selectable.Transition.Animation;
                var rButtonAssist = rAllButtons[i].gameObject.ReceiveComponent<ButtonAssist>();
                rButtonAssist.Button = rAllButtons[i];
                rButtonAssist.AudioClipName = "click";
                rButtonAssist.AudioDisableClipName = "click_invalid";

                var rAnimator = rAllButtons[i].gameObject.ReceiveComponent<Animator>();
                rAnimator.runtimeAnimatorController = rButtonAnim;

                var rEventBinding = rAllButtons[i].GetComponent<EventBinding>();
                if (rEventBinding != null)
                {
                    rEventBinding.ViewEvent = rEventBinding.ViewEvent.Replace(".Button/onClick", ".ButtonAssist/onClick");
                }
            }

            PrefabUtility.SaveAsPrefabAsset(rUIGo, rAssetPath);
            UtilTool.SafeDestroy(rUIGo);
        }

        [MenuItem("GameObject/UI/Button Extend")]
        public static void AddButton()
        {
            GameObject rSelectGo = Selection.activeGameObject;

            var rResultType = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(rAssembly => rAssembly.GetName().Name.Equals("UnityEditor.UI"))?.GetTypes()?
                .SingleOrDefault(rType => rType.FullName.Equals("UnityEditor.UI.MenuOptions"));

            if (rResultType == null)
            {
                return;
            }

            var rStandradResources = (DefaultControls.Resources)rResultType.InvokeMember(
                "GetStandardResources",
                System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null,
                null,
                new object[0]
                );

            var rGo = DefaultControls.CreateButton(rStandradResources);

            var rButtonGo = rResultType.InvokeMember(
                "PlaceUIElementRoot",
                System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null,
                null,
                new object[] { rGo, new MenuCommand(rSelectGo) });

            var rButtonAssist = rGo.ReceiveComponent<ButtonAssist>();

            rButtonAssist.Button = rGo.GetComponent<Button>();
            rButtonAssist.AudioClipName = "click";
            rButtonAssist.AudioDisableClipName = "click_invalid";

            var rButtonAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Game/GameAsset/GUI/Animations/Button.controller");
            var rAnimator = rGo.ReceiveComponent<Animator>();
            rAnimator.runtimeAnimatorController = rButtonAnim;

            Selection.activeGameObject = rGo;
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
        [MenuItem("GameObject/UI/LongClickButton")]
        public static void CreateLongClickButton()
        {
            GameObject rSelectGo = Selection.activeGameObject;
            GameObject rGo = new GameObject("LongClickButton", typeof(DelayClickEvent));
            Undo.RegisterCreatedObjectUndo(rGo, "Create LongClickButton");
            rGo.transform.SetParent(rSelectGo.transform);
            rGo.transform.localPosition = Vector3.zero;
            rGo.transform.localScale = Vector3.one;
            rGo.AddComponent<Image>();

            var rButton = rGo.GetComponent<DelayClickEvent>();
            rButton.targetGraphic = rGo.GetComponent<Image>();
            rButton.targetGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
            rButton.targetGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            Selection.activeGameObject = rGo;
        }

        [MenuItem("Tools/GUI/WText replace Text")]
        public static void WTextReplaceText()
        {
            var rTextGUID = "  m_Script: {fileID: 708705254, guid: f70555f144d8491a825f0804e09c671c, type: 3}";
            var rWTextGUID = "  m_Script: {fileID: 11500000, guid: 1d895dddb71167442a07f51c62726d1a, type: 3}";

            NewCompReplaceOldComp(rTextGUID, rWTextGUID);
        }

        private static void NewCompReplaceOldComp(string rOldGUID, string rNewGUID)
        {
            string[] rFolderSelect = Selection.assetGUIDs;
            if (rFolderSelect.Length > 1 || rFolderSelect.Length == 0)
            {
                Debug.LogError("只选一个文件夹好不好呀");
                return;
            }
            string rPath = AssetDatabase.GUIDToAssetPath(rFolderSelect[0]);
            if (!rPath.StartsWith(mUIPrefabAssetsDir))
            {
                Debug.LogError($"确认好自己选的啥{rPath}");
                return;
            }
            if (EditorUtility.DisplayDialog("提示", $"确认将{rPath}路径下的预制体替换Text为WText吗?", "确认", "取消"))
            {
                var rGUIDS = AssetDatabase.FindAssets("t:Prefab", new string[] { rPath });
                for (int i = 0; i < rGUIDS.Length; i++)
                {
                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDS[i]);
                    var rPrefabText = File.ReadAllText(rAssetPath, System.Text.Encoding.UTF8);

                    var rStringBuilder = new StringBuilder();
                    using (var sr = new StringReader(rPrefabText))
                    {
                        while (sr.Peek() > -1)
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
}