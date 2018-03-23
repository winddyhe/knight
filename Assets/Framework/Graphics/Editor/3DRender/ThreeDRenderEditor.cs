//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core;

namespace Framework.Graphics.Editor
{
    public class ThreeDRenderEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/3D UI/Text")]
        public static void CreateText3D()
        {
            GameObject rText3DGo = new GameObject("Text3D");
            rText3DGo.ReceiveComponent<Text3DRenderer>();

            GameObject rSelectGo = Selection.activeGameObject;
            if (rSelectGo != null)
                rText3DGo.transform.parent = rSelectGo.transform;

            Selection.activeGameObject = rText3DGo;

            Undo.RegisterCreatedObjectUndo(rText3DGo, "Text3D GameObject");
        }

        [MenuItem("GameObject/3D UI/Image")]
        public static void CreateImage3D()
        {
            GameObject rImage3DGo = new GameObject("Image3D");
            rImage3DGo.ReceiveComponent<Image3DRenderer>();

            GameObject rSelectGo = Selection.activeGameObject;
            if (rSelectGo != null)
                rImage3DGo.transform.parent = rSelectGo.transform;

            Selection.activeGameObject = rImage3DGo;

            Undo.RegisterCreatedObjectUndo(rImage3DGo, "Image3D GameObject");
        }
    }
}
