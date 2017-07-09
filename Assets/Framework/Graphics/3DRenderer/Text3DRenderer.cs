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
        public TextLayout           TextLayout;
        public Material             Mat;

        [HideInInspector]
        public MeshRenderer         MeshRenderer;
        [HideInInspector]
        public MeshFilter           MeshFilter;
        [HideInInspector]
        public Mesh                 Mesh;

        public Font                 Font
        {
            get
            {
                if (this.TextLayout == null || this.TextLayout.FontData == null) return null;
                return this.TextLayout.FontData.font;
            }
        }

        public TextAnchor Alignment
        {
            get
            {
                if (this.TextLayout == null || this.TextLayout.FontData == null) return TextAnchor.MiddleCenter;
                return this.TextLayout.FontData.alignment;
            }
            set
            {
                if (this.TextLayout == null || this.TextLayout.FontData == null) return;
                this.TextLayout.FontData.alignment = value;
            }
        }
        
        public void RebuildGemotry()
        {
            this.Mesh.Clear();

            this.TextLayout.GeneratorText();

            this.TextLayout.SetMeshVertices(this.Mesh);
            this.TextLayout.SetMeshIndices(this.Mesh);
        }

        public void RebuildMaterial()
        {
            this.MeshRenderer.sharedMaterial = this.Mat;
            if (this.Mat != null)
            {
                if (this.Font != null)
                    this.Mat.SetTexture("_MainTex", this.Font.material.mainTexture);
            }
        }

        public void RebuildText()
        {
            this.RebuildMaterial();
            this.RebuildGemotry();
        }
        
        protected override void AwakeCustom()
        {
            this.CreateMaterial();
            this.CreateGeometry();
        }

        protected override void DestroyCustom()
        {
            UtilTool.SafeDestroy(this.Mesh);
        }

        void OnEnable()
        {
            Font.textureRebuilt += RebuildByFont;
        }

        void OnDisable()
        {
            Font.textureRebuilt -= RebuildByFont;
        }

#if UNITY_EDITOR
        static Vector3[] rectVertices = new Vector3[2];
        void OnDrawGizmos()
        {
            if (this.TextLayout == null) return;

            rectVertices[0] = this.transform.position + new Vector3(this.TextLayout.Extents.x / 2.0f, this.TextLayout.Extents.y / 2.0f, 0.0f);
            rectVertices[1] = new Vector3(this.TextLayout.Extents.x, this.TextLayout.Extents.y, 0.0f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(rectVertices[0], rectVertices[1]);
        }
#endif

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
                this.Mat.SetTexture("_MainTex", this.TextLayout.FontData.font.material.GetTexture("_MainTex"));
            }
        }

        private void CreateGeometry()
        {
            this.MeshFilter = this.gameObject.ReceiveComponent<MeshFilter>();
            UtilTool.SafeDestroy(this.Mesh);
            this.Mesh = new Mesh();
            this.MeshFilter.sharedMesh = this.Mesh;

            this.TextLayout = this.TextLayout ?? new TextLayout();
            this.TextLayout.GeneratorText();

            this.TextLayout.SetMeshVertices(this.Mesh);
            this.TextLayout.SetMeshIndices(this.Mesh);

            this.Mesh.MarkDynamic();
        }


        private void RebuildByFont(Font rFont)
        {
            if (this.TextLayout == null || this.TextLayout.FontData == null) return;

            Font rOriginFont = this.TextLayout.FontData.font;
            if (rOriginFont != null && rOriginFont == rFont)
            {
                this.RebuildText();
            }
        }
    }
}