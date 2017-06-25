using UnityEngine;
using System.Collections;
using Core;
using System.Collections.Generic;

namespace Framework.Graphics
{
    [ExecuteInEditMode]
    public class Image3DRenderer : TEditorUpdateMB<Image3DRenderer>
    {
        public Sprite           Sprite;
        public ImageLayout      ImageLayout;

        public Material         Mat;

        [HideInInspector]
        public MeshRenderer     MeshRenderer;
        [HideInInspector]
        public MeshFilter       MeshFilter;
        [HideInInspector]
        public Mesh             Mesh;

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
            this.MeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            this.MeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            this.MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.MeshRenderer.receiveShadows = false;
            this.MeshRenderer.sharedMaterial = this.Mat;

            if (this.Mat != null)
            {
                this.Mat.SetColor("_Color", this.ImageLayout.Color);
                this.Mat.SetTexture("_MainTex", this.Sprite.texture);
            }
        }

        private void CreateGeometry()
        {
            this.MeshFilter = this.gameObject.ReceiveComponent<MeshFilter>();
            UtilTool.SafeDestroy(this.Mesh);
            this.Mesh = new Mesh();
            this.MeshFilter.sharedMesh = this.Mesh;

            this.ImageLayout.GenerateSprite(this.Sprite);

            this.ImageLayout.SetMeshVertices(this.Mesh);
            this.ImageLayout.SetMeshIndices(this.Mesh);

            this.Mesh.MarkDynamic();
        }
    }
}