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
        public Color            Color;
        public float            Width;
        public float            Height;

        public Material         Mat;

        [HideInInspector]
        public MeshRenderer     MeshRenderer;
        [HideInInspector]
        public MeshFilter       MeshFilter;
        [HideInInspector]
        public Mesh             Mesh;

        private List<Vector3>   mVertices;
        private List<Vector2>   mUVs;
        private List<int>       mIndices;

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
                this.Mat.SetColor("_Color", this.Color);
                this.Mat.SetTexture("_MainTex", this.Sprite.texture);
            }
        }

        private void CreateGeometry()
        {
            this.MeshFilter = this.gameObject.ReceiveComponent<MeshFilter>();
            UtilTool.SafeDestroy(this.Mesh);
            this.Mesh = new Mesh();
            this.MeshFilter.sharedMesh = this.Mesh;

            float fHalfWidth = this.Width / 2;
            float fHalfHeight = this.Height / 2;
            if (this.Sprite != null)
            {
                this.mVertices = new List<Vector3>();
                for (int i = 0; i < this.Sprite.vertices.Length; i++)
                {
                    this.mVertices.Add(new Vector3(this.Sprite.vertices[i].x*this.Width, this.Sprite.vertices[i].y*this.Height, 0.0f));
                }
                this.mUVs = new List<Vector2>(this.Sprite.uv);
                this.mIndices = new List<int>();
                for (int i = 0; i < this.Sprite.triangles.Length; i++)
                {
                    this.mIndices.Add(this.Sprite.triangles[i]);
                }
            }
            else
            {
                this.mVertices = new List<Vector3>()
                {
                    new Vector3(-fHalfWidth, -fHalfHeight, 0),
                    new Vector3( fHalfWidth, -fHalfHeight, 0),
                    new Vector3( fHalfWidth,  fHalfHeight, 0),
                    new Vector3(-fHalfWidth,  fHalfHeight, 0),
                };

                this.mUVs = new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                };
                this.mIndices = new List<int>() { 0, 1, 2, 0, 2, 3 };
            }

            this.Mesh.SetVertices(this.mVertices);
            this.Mesh.SetUVs(0, this.mUVs);
            this.Mesh.SetIndices(this.mIndices.ToArray(), MeshTopology.Triangles, 0);
            this.Mesh.MarkDynamic();
        }
    }
}