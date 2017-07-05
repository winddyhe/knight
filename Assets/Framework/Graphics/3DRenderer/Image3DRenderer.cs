//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using System.Collections.Generic;

namespace Framework.Graphics
{
    [ExecuteInEditMode]
    public class Image3DRenderer : TEditorUpdateMB<Image3DRenderer>
    {
        public Sprite               Sprite;
        public ImageLayout          ImageLayout;

        public Color                ImageColor = Color.white;
        public Material             Mat;

        [HideInInspector]
        public MeshRenderer         MeshRenderer;
        [HideInInspector]
        public MeshFilter           MeshFilter;
        [HideInInspector]
        public Mesh                 Mesh;

        public bool HasBorder()
        {
            if (this.Sprite == null) return false;
            return this.Sprite.border.sqrMagnitude > 0;
        }

        public void RebuildGemotry()
        {
            this.Mesh.Clear();

            this.ImageLayout.GenerateSprite(this.Sprite);

            this.ImageLayout.SetMeshVertices(this.Mesh);
            this.ImageLayout.SetMeshIndices(this.Mesh);
        }

        public void RebuildSprite()
        {
            RebuildMaterial();
            RebuildGemotry();
        }

        public void SetSpriteNativeSize()
        {
            if (this.Sprite == null) return;
            this.ImageLayout.Width = this.Sprite.rect.width;
            this.ImageLayout.Height = this.Sprite.rect.height;
        }

        public void SetSpriteFillOrgin(int nIndex)
        {
            this.ImageLayout.FillOrigin = nIndex;
        }

        public void AspectRatioSprite_Width(float fValue)
        {
            if (this.Sprite == null) return;

            if (fValue < 1)
            {
                this.ImageLayout.Width = 1;
                this.ImageLayout.Height = 1;
                return;
            }
            if (this.ImageLayout.IsPreserveAspect)
            {
                float fRealRatio = this.Sprite.rect.height / this.Sprite.rect.width;
                this.ImageLayout.Width = fValue;
                this.ImageLayout.Height = fValue * fRealRatio;
            }
        }

        public void AspectRatioSprite_Height(float fValue)
        {
            if (this.Sprite == null) return;

            if (fValue < 1)
            {
                this.ImageLayout.Width = 1;
                this.ImageLayout.Height = 1;
                return;
            }
            if (this.ImageLayout.IsPreserveAspect)
            {
                float fRealRatio = this.Sprite.rect.width / this.Sprite.rect.height;
                this.ImageLayout.Height = fValue;
                this.ImageLayout.Width = fValue * fRealRatio;
            }
        }

        public void RebuildMaterial()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UtilTool.SafeDestroy(this.MeshRenderer.sharedMaterial);
                Material rCopyMat = new Material(this.Mat);
                this.MeshRenderer.sharedMaterial = rCopyMat;
                if (rCopyMat != null)
                {
                    rCopyMat.SetColor("_Color", this.ImageColor);
                    if (this.Sprite != null)
                        rCopyMat.SetTexture("_MainTex", this.Sprite.texture);
                }
            }
            else
            {
                this.MeshRenderer.sharedMaterial = this.Mat;
                if (this.Mat != null)
                {
                    this.Mat.SetColor("_Color", this.ImageColor);
                    if (this.Sprite != null)
                        this.Mat.SetTexture("_MainTex", this.Sprite.texture);
                }
            }
#else
            this.MeshRenderer.sharedMaterial = this.Mat;
            if (this.Mat != null)
            {
                this.Mat.SetColor("_Color", this.ImageColor);
                this.Mat.SetTexture("_MainTex", this.Sprite.texture);
            }
#endif
        }

        protected override void AwakeCustom()
        {
            this.CreateMaterial();
            this.CreateGeometry();
        }

        protected override void DestroyCustom()
        {
            UtilTool.SafeDestroy(this.Mesh);

            if (this.Mat != null)
                this.Mat.SetTexture("_MainTex", null);
        }

        private void CreateMaterial()
        {
            this.MeshRenderer = this.gameObject.ReceiveComponent<MeshRenderer>();
            this.MeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            this.MeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            this.MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.MeshRenderer.receiveShadows = false;

            RebuildMaterial();
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