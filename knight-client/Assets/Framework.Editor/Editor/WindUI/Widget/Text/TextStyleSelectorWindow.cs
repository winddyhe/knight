using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UnityEditor.UI
{
    public class TextStyleSelectorWindow : EditorWindow
    {
        private TextStyleManager mTextStyleManager;
        private Vector2 mScroll;

        private readonly int mWidth = 600;
        private readonly int mHeight = 40;

        private void OnEnable()
        {
            var rGo = AssetDatabase.LoadAssetAtPath("Assets/Game/GameAsset/GUI/Prefabs/TextStyleManager.prefab",
                typeof(GameObject)) as GameObject;

            if (rGo)
                this.mTextStyleManager = rGo.GetComponent<TextStyleManager>();
        }

        private void OnGUI()
        {
            this.mScroll = EditorGUILayout.BeginScrollView(mScroll);

            EditorGUILayout.Space();
            using (var rVertical = new EditorGUILayout.VerticalScope())
            {
                for (int i = 0; i < mTextStyleManager.TextStyles.Count; i++)
                {
                    using (var rHorizontal = new EditorGUILayout.HorizontalScope("TextField", GUILayout.Height(40)))
                    {
                        EditorGUILayout.BeginHorizontal();

                        GameObject rTextCameraInstGo = null;
                        Texture rTexture = CreateTextStyleRenderTexture(mWidth + 100, mHeight, out rTextCameraInstGo, mTextStyleManager.TextStyles[i]);

                        if (rTexture)
                        {
                            EditorGUI.DrawTextureTransparent(new Rect
                                (rHorizontal.rect.xMin, rHorizontal.rect.yMin, mWidth, mHeight), rTexture);
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("一键切换", GUILayout.Height(35), GUILayout.Width(70)))
                        {
                            var rObj = Selection.activeObject as GameObject;
                            if (rObj)
                            {
                                TextStyleSelector rTextStyle = rObj.GetComponent<TextStyleSelector>();
                                if (rTextStyle)
                                    rTextStyle.ApplyStyle(mTextStyleManager.TextStyles[i]);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        GameObject.DestroyImmediate(rTexture);
                        GameObject.DestroyImmediate(rTextCameraInstGo);
                    }
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private RenderTexture CreateTextStyleRenderTexture(int nWidth, int nHeight, out GameObject rTextCameraInstGo, TextStyle rStyle)
        {
            RenderTexture rRenderTexture = new RenderTexture(nWidth, nHeight, 24,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

            var rTextCameraPath = "Assets/Game/GameAsset/GUI/Styles/TextStyleCamera.prefab";

            var rTextCameraPrefabGo = AssetDatabase.LoadAssetAtPath(rTextCameraPath, typeof(GameObject)) as GameObject;
            rTextCameraInstGo = Instantiate(rTextCameraPrefabGo);
            rTextCameraInstGo.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideAndDontSave;
            var rTextStyleCamera = rTextCameraInstGo.GetComponent<TextStyleCamera>();

            TextStyleSelector rTextStyle = rTextStyleCamera.TextTarget.GetComponent<TextStyleSelector>();
            rTextStyle.ApplyStyle(rStyle);

            rTextStyleCamera.TextTarget.text = string.Format("{0} Font: {1}, Size: {2}", rStyle.Description, rStyle.Font.name, rStyle.Font.fontSize);
            rTextStyleCamera.TextCamera.targetTexture = rRenderTexture;
            rTextStyleCamera.TextCamera.Render();
            rTextStyleCamera.TextCamera.targetTexture = null;

            return rRenderTexture;
        }
    }
}