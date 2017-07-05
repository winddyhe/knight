//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Framework.Graphics
{
    [ExecuteInEditMode]
    public class Text3DRenderer : TEditorUpdateMB<Text3DRenderer>
    {
        public string       Text;
        public Color        Color;

        public Font         Font;
        public FontStyle    FontStyle;
        public int          FontSize;

        public Material     Mat;

        [HideInInspector]
        public MeshRenderer MeshRenderer;
        [HideInInspector]
        public MeshFilter   MeshFilter;
        [HideInInspector]
        public Mesh         Mesh;

        protected override void AwakeCustom()
        {
            this.CreateMaterial();
            this.CreateGeometry();
        }

        protected override void DestroyCustom()
        {
            UtilTool.SafeDestroy(this.Mesh);
        }

        private void CreateMaterial()
        {
            this.MeshRenderer = this.gameObject.ReceiveComponent<MeshRenderer>();
            this.MeshRenderer.sharedMaterial = this.Mat;
            this.MeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            this.MeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            this.MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.MeshRenderer.receiveShadows = false;

            if (this.Mat != null)
            {
                this.Mat.SetColor("_Color", this.Color);
                this.Mat.SetTexture("_MainTex", this.Font.material.GetTexture("_MainTex"));
            }
        }

        private void CreateGeometry()
        {
            this.MeshFilter = this.gameObject.ReceiveComponent<MeshFilter>();
            UtilTool.SafeDestroy(this.Mesh);
            this.Mesh = new Mesh();
            this.MeshFilter.sharedMesh = this.Mesh;
        }
    }
}